using B2bOrder.Models;
using B2bOrder.Models.Common;
using B2bOrder.Models.Db;
using B2bOrder.Responsitories;
using B2bOrder.Services.Common; // 引用 EmailService[cite: 1]
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System.Diagnostics;
using System.Text;

namespace B2bOrder.Services
{
    // Routine 使用的模型
    // 1. 下線會員與狀態模型
    public class DownlineTargetItem
    {
        public dynamic Member { get; set; } = default!;
        public bool IsExpired { get; set; }
    }

    // 2. 祖先與其下線清單模型
    public class AncestorGroup
    {
        public dynamic Parent { get; set; } = default!;
        public List<DownlineTargetItem> Downlines { get; set; } = new();
    }

    public class NotificationHostedService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<NotificationHostedService> _logger;
        private readonly IConfiguration _configuration;
        private ILogPushService _logPushService;

        private bool _firebaseInitialized;

        public NotificationHostedService(
            IServiceProvider provider,
            ILogger<NotificationHostedService> logger,
            IConfiguration configuration)
        {
            _provider = provider;
            _logger = logger;
            _configuration = configuration;
            _firebaseInitialized = false;

            using var scope = _provider.CreateScope();
            _logPushService = scope.ServiceProvider.GetRequiredService<ILogPushService>();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationHostedService started.");

            // 初始化 FirebaseApp
            try
            {
                InitializeFirebaseIfNeeded();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firebase 初始化失敗，推播功能將不可用。");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var Log_Message = "";
                try
                {
                    // 從 appsettings.json 讀取發送時間 (預設值: 10:00)
                    int resendIntervalDays = _configuration.GetValue<int>("Notification:ResendIntervalDays", 10);
                    int targetHour   = _configuration.GetValue<int>("Notification:DailyReminderHour", 10);
                    int targetMinute = _configuration.GetValue<int>("Notification:DailyReminderMinute", 0);

                    // 計算距離下一次目標時間的延遲
                    var delay = GetDelayUntilTargetTime(targetHour, targetMinute);

                    _logger.LogInformation("等待下一次發送時間，剩餘：{Delay}", delay);
                    await Task.Delay(delay, stoppingToken);

                    using var scope = _provider.CreateScope();

                    var memberRepo = scope.ServiceProvider.GetRequiredService<IDataMemberRepository>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    // 1. 取得所有到期會員
                    var expiringMembers = await memberRepo.GetExpiringWithinDaysAsync(30);
                    var overExpiredMembers = await memberRepo.GetOverExpiringAsync();

                    // 2. 統一包裝成帶有狀態 (IsExpired) 的物件清單
                    var allTargetMembers = new List<(dynamic Member, bool IsExpired)>();

                    if (expiringMembers != null)
                    {
                        allTargetMembers.AddRange(expiringMembers.Select(m => ((dynamic)m, IsExpired: false)));
                    }

                    if (overExpiredMembers != null)
                    {
                        allTargetMembers.AddRange(overExpiredMembers.Select(m => ((dynamic)m, IsExpired: true)));
                    }

                    // 快取已查過的 Member 物件 (Key: Sid, Value: Member)
                    var memberCache = new Dictionary<string, dynamic>(StringComparer.OrdinalIgnoreCase);

                    // 彙整字典：Key 為祖先/父會員的 Sid (字串)
                    // Value 為 Tuple：(父會員物件, 該父會員對應到的所有到期下線列表)
                    // ✅ 新寫法：改用 AncestorGroup 類別
                    var ancestorToDownlinesMap = new Dictionary<string, AncestorGroup>(StringComparer.OrdinalIgnoreCase);

                    foreach (var item in allTargetMembers)
                    {
                        var m = item.Member;
                        bool isExpired = item.IsExpired;
                        if (m == null) continue;


                        string memberSid = ((object)m.Sid)?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(memberSid))
                        {
                            memberCache[memberSid] = m;
                        }

                        // -------------------------------------------------------------
                        // [A] 發送 FCM 推播給當前到期會員本人
                        // -------------------------------------------------------------
                        if (_firebaseInitialized && !string.IsNullOrWhiteSpace((string)m.FcmToken))
                        {
                            try
                            {
                                var message = new Message
                                {
                                    Token = m.FcmToken,
                                    Notification = new Notification
                                    {
                                        Title = "會員續約提醒",
                                        Body = $"親愛的 {m.Name ?? m.Mid}, 您的合約即將到期，請盡速續約。"
                                    },
                                    Data = new Dictionary<string, string>
                                    {
                                        { "nid", m.Nid.ToString() },
                                        { "type", "expiry_reminder" }
                                    }
                                };

                                var resp = await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken: stoppingToken);
                                _logger.LogInformation($"Sent FCM to {m.Mid} token (response: {resp})");
                                await WritePushLog(m, "EXPIRE_NOTICE", "FCM", message.Notification.Title, message.Notification.Body, "SUCCESS");
                            }
                            catch (Exception sendEx)
                            {
                                _logger.LogError($"發送 FCM 給會員 {m.Mid} 失敗 Exception :{sendEx}");
                                await WritePushLog(m, "EXPIRE_NOTICE", "FCM", "發送 FCM 推播", $"發送 FCM 給會員 {m.Mid} 失敗 Exception :{sendEx}", "FAILURE");
                            }
                        }

                        // -------------------------------------------------------------
                        // [B] 發送 Email 給會員「本人」(區分：即將到期 vs 已逾期)
                        // -------------------------------------------------------------
                        if (!string.IsNullOrWhiteSpace((string)m.Email))
                        {
                            try
                            {
                                string subject = isExpired ? "【續約通知】您的會員合約已經逾期" : "【續約提醒】您的會員合約即將到期";
                                string body = isExpired
                                    ? $@"<h2>親愛的 {m.Name ?? m.Mid} 您好：</h2>
                                         <p>提醒您，您的會員合約<strong style='color:red;'>已經逾期</strong>，請盡速辦理續約手續以保障您的權益與相關服務。</p>
                                         <br/><p>系統自動發送，請勿直接回覆。</p>"
                                    : $@"<h2>親愛的 {m.Name ?? m.Mid} 您好：</h2>
                                         <p>提醒您，您的會員合約即將到期，請盡速辦理續約以維護權益。</p>
                                         <br/><p>系統自動發送，請勿直接回覆。</p>";

                                var resp = await emailService.SendAsync(m.Email, subject, body);
                                Log_Message = $"成功發送個人續約/逾期 Email 給本人 Mid: {m.Mid}, Email: {m.Email}, response :{resp}";
                                _logger.LogInformation(Log_Message);
                            }
                            catch (Exception emailEx)
                            {
                                Log_Message = $"發送 Email 給本人 {m.Mid} 時發生異常! Exception :{emailEx}";
                                _logger.LogError(Log_Message);
                            }
                        }

                        // -------------------------------------------------------------
                        // [C] 向上追溯父/祖先節點，並在記憶體中進行分組彙整
                        // -------------------------------------------------------------
                        var visitedSids = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { memberSid };
                        string? currentParentSid = ((object)m.ParentSid)?.ToString();

                        while (!string.IsNullOrEmpty(currentParentSid))
                        {
                            string pSid = currentParentSid.Trim();

                            if (!visitedSids.Add(pSid))
                            {
                                Log_Message = $"檢測到會員 ParentSid 循環鏈結，停止追溯，Sid: {pSid}, Mid :{m.Mid}";
                                _logger.LogInformation(Log_Message);
                                break;
                            }

                            if (!memberCache.TryGetValue(pSid, out dynamic parentMember))
                            {
                                parentMember = await memberRepo.GetBySidAsync(pSid);
                                if (parentMember != null)
                                {
                                    memberCache[pSid] = parentMember;
                                }
                            }

                            if (parentMember == null) break;

                            // ✅ 修正 2：使用類別初始化，完全避免 Value Tuple 型態不齊問題
                            if (!ancestorToDownlinesMap.TryGetValue(pSid, out var ancestorData))
                            {
                                ancestorData = new AncestorGroup { Parent = parentMember };
                                ancestorToDownlinesMap[pSid] = ancestorData;
                            }

                            // ✅ 修正 3：防止重複加入下線，判定簡單清晰
                            if (!ancestorData.Downlines.Any(x => string.Equals(((object)x.Member.Sid)?.ToString(), memberSid, StringComparison.OrdinalIgnoreCase)))
                            {
                                ancestorData.Downlines.Add(new DownlineTargetItem { Member = m, IsExpired = isExpired });
                            }

                            currentParentSid = ((object)parentMember.ParentSid)?.ToString();
                        }
                    }

                    // -------------------------------------------------------------
                    // [D] 統一寄送彙整後的 HTML 信件給各個父/祖先會員
                    // -------------------------------------------------------------
                    foreach (var kvp in ancestorToDownlinesMap)
                    {
                        var ancestor = kvp.Value.Parent;
                        var downlineList = kvp.Value.Downlines;

                        if (string.IsNullOrWhiteSpace((string)ancestor.Email))
                        {
                            Log_Message = $"上線/祖先會員 Mid: {ancestor.Mid} 之 Email 為空或 null，跳過發送彙整Email";
                            _logger.LogInformation(Log_Message, (string)ancestor.Mid);
                            continue;
                        }

                        // 【冷卻檢查】判斷 10 天內上線是否已收到過組織彙整信
                        bool hasSentAncestorRecently = await _logPushService.HasSentRecentlyAsync(ancestor.Sid, "DOWNLINE_SUMMARY_NOTICE", 10);
                        if (hasSentAncestorRecently)
                        {
                            Log_Message = $"上線/祖先會員 Mid: {ancestor.Mid} 在 10 天內已收到過組織彙整信，本次跳過。";
                            _logger.LogInformation(Log_Message);
                            continue;
                        }

                        try
                        {
                            string subject = $"【組織管理提醒】您有 {downlineList.Count} 位下線會員合約即將到期/已逾期";
                            string htmlBody = BuildAggregatedHtmlEmail(ancestor, downlineList, memberCache);

                            bool isSent = await emailService.SendAsync(ancestor.Email, subject, htmlBody);
                            if (isSent)
                            {
                                await WritePushLog(ancestor, "EXPIRE_NOTICE", "EMAIL", subject, htmlBody, "SUCCESS");
                                Log_Message = $"成功發送下線到期彙整信件給上線 Mid: {ancestor.Mid}, Email: {ancestor.Email}, 包含 {downlineList.Count} 位下線";
                                _logger.LogInformation(Log_Message, (string)ancestor.Mid);
                            }
                            else
                            {
                                await WritePushLog(ancestor, "EXPIRE_NOTICE", "EMAIL", subject, htmlBody, "FAILURE");
                                Log_Message = $"發送下線到期彙整信件給上線 Mid: {ancestor.Mid} 失敗";
                                _logger.LogInformation(Log_Message, (string)ancestor.Mid);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log_Message = $"發送下線到期彙整信件給上線 Mid: {ancestor.Mid} 時發生異常";
                            await WritePushLog(ancestor, "EXPIRE_NOTICE", "EMAIL", "發送下線到期彙整信件", Log_Message, "FAIL", ex.Message);
                            _logger.LogError(Log_Message, (string)ancestor.Mid, ex);
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Log_Message = $"Error in NotificationHostedService main loop:{ex.Message.ToString()}";
                    _logger.LogError(Log_Message, ex);
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        /// <summary>
        /// 生成寄給上線/祖先的 HTML 彙整格式信件
        /// </summary>
        private string BuildAggregatedHtmlEmail(
            dynamic ancestor,
            List<DownlineTargetItem> downlines,
            Dictionary<string, dynamic> memberCache)
        {
            string ancestorName = string.IsNullOrWhiteSpace((string)ancestor.Name) ? "未填寫" : (string)ancestor.Name;

            var sb = new StringBuilder();
            sb.Append($@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h2>親愛的 {ancestorName} 您好：</h2>
                    <p>以下為您組織旗下合約即將到期（30天內）或已經逾期的下線會員清單，請協助提醒並輔導續約：</p>
            
                    <table style='border-collapse: collapse; width: 100%; max-width: 800px; margin-top: 15px;'>
                        <thead>
                            <tr style='background-color: #4CAF50; color: white;'>
                                <th style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>會員編號</th>
                                <th style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>會員姓名</th>
                                <th style='border: 1px solid #dddddd; text-align: center; padding: 8px;'>狀態</th>
                                <th style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>聯絡 Email</th>
                                <th style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>直屬上線</th>
                            </tr>
                        </thead>
                        <tbody>");

            foreach (var item in downlines)
            {
                try
                {
                    var m = item.Member;
                    bool isExpired = item.IsExpired;

                    // -------------------------------------------------------------
                    // 從 ParentSid 向 memberCache 查詢直屬上線資料
                    // -------------------------------------------------------------
                    string parentSid = ((object)m.ParentSid)?.ToString() ?? "";
                    string parentMid = "無";
                    string parentName = "無";

                    if (!string.IsNullOrEmpty(parentSid) && memberCache.TryGetValue(parentSid, out dynamic directParent))
                    {
                        if (directParent != null)
                        {
                            parentMid = ((object)directParent.Mid)?.ToString() ?? "無";
                            parentName = string.IsNullOrWhiteSpace((string)directParent.Name) ? "未填寫" : (string)directParent.Name;
                        }
                    }

                    // 狀態標籤
                    string statusBadge = isExpired
                        ? "<span style='color: #d9534f; font-weight: bold;'>已逾期</span>"
                        : "<span style='color: #f0ad4e; font-weight: bold;'>即將到期</span>";

                    string emailStr = string.IsNullOrWhiteSpace((string)m.Email) ? "無" : (string)m.Email;
                    string nameStr = string.IsNullOrWhiteSpace((string)m.Name) ? "未填寫" : (string)m.Name;

                    sb.Append($@"
                    <tr>
                        <td style='border: 1px solid #dddddd; padding: 8px;'>{m.Mid}</td>
                        <td style='border: 1px solid #dddddd; padding: 8px;'>{nameStr}</td>
                        <td style='border: 1px solid #dddddd; padding: 8px; text-align: center;'>{statusBadge}</td>
                        <td style='border: 1px solid #dddddd; padding: 8px;'>{emailStr}</td>
                        <td style='border: 1px solid #dddddd; padding: 8px;'>{parentMid}({parentName})</td>
                    </tr>");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"生成下線會員清單時發生異常，會員 MID: {((object)item.Member.Mid)?.ToString() ?? "未知"}");
                }
            }

            sb.Append($@"
                    </tbody>
                </table>
            
                <br/>
                <p style='color: #777; font-size: 12px;'>此信件為系統自動發送，請勿直接回覆。</p>
            </div>");

            return sb.ToString();
        }

        private TimeSpan GetDelayUntilTargetTime(int targetHour, int targetMinute)
        {
            TimeZoneInfo tzi;
            try
            {
                tzi = TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei"); // Linux / Docker
            }
            catch
            {
                tzi = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time"); // Windows
            }

            var now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tzi);
            var nextRun = new DateTime(now.Year, now.Month, now.Day, targetHour, targetMinute, 0);

            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            return nextRun - now;
        }
        private async Task WritePushLog(
            DataMember member,
            string notifyType,
            string pushType,
            string title,
            string message,
            string result,
            string responseMessage = "")
        {

            await _logPushService.CreateAsync(
                new LogPush
                {
                    MemberSid = member.Sid,
                    MemberMid = member.Mid,
                    NotifyType = notifyType,
                    PushType = pushType,
                    Title = title,
                    Message = message,
                    SendDate = DateTime.Now,
                    Result = result,
                    ResponseMessage = responseMessage,
                    CreateSid = "B2bOrder背景作業"
                });
        }
        private void InitializeFirebaseIfNeeded()
        {
            if (_firebaseInitialized) return;

            var credPath = _configuration["Firebase:CredentialsPath"];
            var credJson = _configuration["Firebase:CredentialsJson"];

            if (string.IsNullOrWhiteSpace(credPath) && string.IsNullOrWhiteSpace(credJson))
            {
                _logger.LogWarning("未設定 Firebase credentials。無法初始化 Firebase.");
                return;
            }

            if (FirebaseApp.DefaultInstance != null)
            {
                _firebaseInitialized = true;
                return;
            }

            GoogleCredential credential = !string.IsNullOrWhiteSpace(credPath)
                ? GoogleCredential.FromFile(credPath)
                : GoogleCredential.FromJson(credJson);

            FirebaseApp.Create(new AppOptions { Credential = credential });
            _firebaseInitialized = true;
        }
    }
}