using B2bOrder.Models;
using B2bOrder.Models.Common;
using B2bOrder.Models.Db;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;

namespace B2bOrder.Services
{
    public interface IMemberExpireNoticeService : IBaseService
    {
        Task SendExpireNoticeAsync();

        /// <summary>
        /// 取得完整到期通知頁面的統計看板、分流篩選、排序與分頁後的 ViewModel 資料
        /// </summary>
        Task<ExpireNoticePageViewModel?> GetExpireNoticePageDataAsync(
            string memberSid, string tab, string keyword, string sortField, string sortDir, int page, int pageSize);

        /// <summary>
        /// 手動發送 LINE 到期通知並記錄相關 Log
        /// </summary>
        /// <returns>回傳發送結果 (是否成功, 錯誤訊息)</returns>
        Task<(bool Success, string Message)> SendManualLineNoticeAsync(string sid);

        /// <summary>
        /// 手動發送 FCM 到期通知並記錄相關 Log（新加入）
        /// </summary>
        /// <param name="sid">會員 SID</param>
        /// <returns>回傳發送結果 (是否成功, 錯誤訊息)</returns>
        Task<(bool Success, string Message)> SendManualFcmNoticeAsync(string sid);

        /// <summary>
        /// 手動發送 EMAIL 到期通知並記錄相關 Log
        /// </summary>
        /// <param name="sid">會員 SID</param>
        /// <returns>回傳發送結果 (是否成功, 錯誤訊息)</returns>
        Task<(bool Success, string Message)> SendManualEmailNoticeAsync(string sid);

        /// <summary>
        /// 批次發送 LINE 到期通知並記錄相關 Log
        /// </summary>
        /// <param name="sids">會員 SID 列表</param>
        /// <returns>回傳一個自訂物件，包含成功狀態、成功件數與失敗件數</returns>
        Task<(bool Success, int SuccessCount, int FailureCount)> SendBatchLineNoticeAsync(List<string> sids);

        /// <summary>
        /// 批次發送 FCM 到期通知並記錄相關 Log
        /// </summary>
        /// <param name="sids">會員 SID 列表</param>
        /// <returns>回傳一個自訂物件，包含成功狀態、成功件數與失敗件數</returns>
        Task<(bool Success, int SuccessCount, int FailureCount)> SendBatchFcmNoticeAsync(List<string> sids);

        /// <summary>
        /// 批次發送 EMAIL 到期通知並記錄相關 Log
        /// </summary>
        /// <param name="sids">會員 SID 列表</param>
        /// <returns>回傳一個自訂物件，包含成功狀態、成功件數與失敗件數</returns>
        Task<(bool Success, int SuccessCount, int FailureCount)> SendBatchEmailNoticeAsync(List<string> sids);

        /// <summary>
        /// 產生到期會員通知的匯出檔案 (Excel / CSV)
        /// </summary>
        /// <param name="type">檔案類型 ("excel" 或 "csv")</param>
        /// <param name="keyword">搜尋關鍵字</param>
        Task<FileDownloadModel> ExportExpireNoticeFileAsync(string memberSid, string type, string keyword);

        /// <summary>
        /// 產生到期通知歷史紀錄的匯出檔案 (Excel / CSV)
        /// </summary>
        /// <param name="memberSid">操作者會員 SID</param>
        /// <param name="type">檔案類型 ("excel" 或 "csv")</param>
        /// <param name="keyword">搜尋關鍵字</param>
        Task<FileDownloadModel> ExportExpireHistoryFileAsync(string memberSid, string type, string keyword);
    }

    public class MemberExpireNoticeService : BaseService, IMemberExpireNoticeService
    {
        private readonly IMemberService _memberService;

        public MemberExpireNoticeService(IServiceProvider serviceProvider, IMemberService memberService)
            : base(serviceProvider)
        {
            _memberService = memberService;
        }

        #region 私有方法：獨立推播通知紀錄 (LogPush)

        /// <summary>
        /// 寫入推播明細紀錄，原名 WriteLog，現更名為 WritePushLog 以防與基底系統日誌衝突
        /// </summary>
        private async Task WritePushLog(DataMember member, string pushType, string title, string message, string result, string response = "")
        {
            await _logPushService.CreateAsync(new LogPush
            {
                MemberSid = member.Sid,
                MemberMid = member.Mid,
                NotifyType = "EXPIRE_NOTICE",
                PushType = pushType,
                Title = title,
                Message = message,
                SendDate = DateTime.Now,
                Result = result,
                ResponseMessage = response
            });
        }

        #endregion

        #region 1. 定期自動發送通知邏輯 (排程用)

        public async Task SendExpireNoticeAsync()
        {
            var members = await _memberService.GetAllAsync();

            foreach (var member in members)
            {
                if (member.Avalible != "Y")
                    continue;

                if (member.NoticeEnable != "Y")
                    continue;

                if (!member.ContinueDate.HasValue)
                    continue;

                var remainDays = (member.ContinueDate.Value.Date - DateTime.Today).Days;

                if (remainDays < 0)
                    continue;

                if (remainDays > member.HintDays)
                    continue;

                await SendMemberNotice(member, remainDays);
            }
        }

        private async Task SendMemberNotice(DataMember member, int remainDays)
        {
            var title = "會員到期通知";
            var message = $"親愛的 {member.Name}，您的會員資格將於 {remainDays} 天後到期。";

            try
            {
                // LINE
                if (!string.IsNullOrWhiteSpace(member.LineUserId))
                {
                    await _lineService.PushAsync(member.LineUserId, message);
                    // 💡 調整點：改呼叫 WritePushLog
                    await WritePushLog(member, "LINE", title, message, "SUCCESS");
                }

                // EMAIL
                if (!string.IsNullOrWhiteSpace(member.Email))
                {
                    await _emailService.SendAsync(member.Email, title, message);
                    // 💡 調整點：改呼叫 WritePushLog
                    await WritePushLog(member, "EMAIL", title, message, "SUCCESS");
                }

                // FCM
                if (!string.IsNullOrWhiteSpace(member.FcmToken))
                {
                    await _fcmService.SendAsync(member.FcmToken, title, message);
                    // 💡 調整點：改呼叫 WritePushLog
                    await WritePushLog(member, "FCM", title, message, "SUCCESS");
                }
            }
            catch (Exception ex)
            {
                // 💡 調整點：改呼叫 WritePushLog
                await WritePushLog(member, "SYSTEM", title, message, "FAIL", ex.Message);
            }
        }
        #endregion

        #region 2. 後台頁面 View 用的核心資料搜集與篩選分頁邏輯

        public async Task<ExpireNoticePageViewModel?> GetExpireNoticePageDataAsync(
            string memberSid, string tab, string keyword, string sortField, string sortDir, int page, int pageSize)
        {
            var loginMember = await _memberService.GetBySidAsync(memberSid);
            if (loginMember == null) return null;

            var model = new ExpireNoticePageViewModel();
            var today = DateTime.Today;

            // =========================
            // 統計資料計算
            // =========================
            model.NotifyCount = await _logPushService.GetMonthNotifyCountAsync();
            model.SuccessRate = await _logPushService.GetMonthSuccessRateAsync();

            // 1. 取得登入者所有層級 (1~n層) 的下屬成員
            // 若採方式一：_memberRepository.GetAllDescendantsAsync(memberSid)
            // 若採方式二：GetAllDescendantsRecursiveAsync(memberSid)
            var allDescendants = await GetAllDescendantsRecursiveAsync(memberSid);

            // 2. 將「登入者本身」加入查詢清單的最前面
            var allMembers = new List<DataMember> { loginMember };
            allMembers.AddRange(allDescendants);

            // 3. 建立快取字典 (包含登入者與所有下線)
            var memberLookup = allMembers.ToDictionary(m => m.Sid, m => m);
            memberLookup[loginMember.Sid] = loginMember;

            // 3. 統計轄下所有 1~n 層會員的到期數量
            model.ExpireSoonCount = allDescendants.Count(x =>
                x.Avalible == "Y" &&
                x.ContinueDate.HasValue &&
                (x.ContinueDate.Value.Date - today).Days >= 0 &&
                (x.ContinueDate.Value.Date - today).Days <= x.HintDays);

            model.ExpiredCount = allDescendants.Count(x =>
                x.Avalible == "Y" &&
                x.ContinueDate.HasValue &&
                (x.ContinueDate.Value.Date - today).Days < 0);

            // =========================
            // 分流 Tab 處理明細與分頁排序
            // =========================
            if (tab == "member")
            {
                var notifyLogs = await _logPushService.GetLastNotifyMapAsync();
                var tempMembers = new List<ExpireNoticeViewModel>();

                foreach (var member in allMembers)
                {
                    if (member.Avalible != "Y" || !member.ContinueDate.HasValue)
                        continue;

                    // 計算剩餘天數 (負數代表已到期)
                    var remainDays = (member.ContinueDate.Value.Date - today).Days;

                    // 只過濾出「已到期 (remainDays < 0)」與「即將到期 (0 <= remainDays <= HintDays)」
                    if (remainDays > member.HintDays)
                        continue;

                    // 取得該成員的直屬上線 (Parent) 資訊
                    string parentMid = "";
                    string parentName = "";
                    if (!string.IsNullOrEmpty(member.ParentSid) && memberLookup.TryGetValue(member.ParentSid, out var parentObj))
                    {
                        parentMid = parentObj.Mid ?? "";
                        parentName = parentObj.Name ?? "";
                    }

                    // 關鍵字模糊搜尋 (支援：會員編號、姓名、Email、直屬上線帳號、直屬上線姓名)
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        bool match = (member.Mid ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                     (member.Name ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                     (member.Email ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                     parentMid.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                     parentName.Contains(keyword, StringComparison.OrdinalIgnoreCase);
                        if (!match) continue;
                    }

                    tempMembers.Add(new ExpireNoticeViewModel
                    {
                        Sid = member.Sid,
                        Mid = member.Mid,
                        Name = member.Name ?? "",
                        Mobile = member.Mobile ?? "",
                        Email = member.Email ?? "",
                        FcmToken = member.FcmToken ?? "",
                        Role = MemberRoles.GetName(member.Role),
                        ContinueDate = member.ContinueDate,
                        RemainDays = remainDays,
                        HintDays = member.HintDays,
                        // 直屬上線資訊
                        ParentMid = parentMid,
                        ParentName = parentName,
                        Status = remainDays < 0 ? NoticeRemainStatus.EXPIRED : NoticeRemainStatus.EXPIRING,
                        LastNotifyDate = notifyLogs.TryGetValue(member.Sid, out var lastNotify) ? lastNotify : null
                    });
                }

                // 動態排序 (LINQ)
                var isDesc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
                var sortedQuery = sortField?.ToLower() switch
                {
                    "mid" => isDesc ? tempMembers.OrderByDescending(x => x.Mid) : tempMembers.OrderBy(x => x.Mid),
                    "name" => isDesc ? tempMembers.OrderByDescending(x => x.Name) : tempMembers.OrderBy(x => x.Name),
                    "parentmid" => isDesc ? tempMembers.OrderByDescending(x => x.ParentMid) : tempMembers.OrderBy(x => x.ParentMid),
                    "role" => isDesc ? tempMembers.OrderByDescending(x => x.Role) : tempMembers.OrderBy(x => x.Role),
                    "mobile" => isDesc ? tempMembers.OrderByDescending(x => x.Mobile) : tempMembers.OrderBy(x => x.Mobile),
                    "email" => isDesc ? tempMembers.OrderByDescending(x => x.Email) : tempMembers.OrderBy(x => x.Email),
                    "fcm_token" => isDesc ? tempMembers.OrderByDescending(x => x.FcmToken) : tempMembers.OrderBy(x => x.FcmToken),
                    "continuedate" => isDesc ? tempMembers.OrderByDescending(x => x.ContinueDate) : tempMembers.OrderBy(x => x.ContinueDate),
                    "remaindays" => isDesc ? tempMembers.OrderByDescending(x => x.RemainDays) : tempMembers.OrderBy(x => x.RemainDays),
                    "lastnotify" => isDesc ? tempMembers.OrderByDescending(x => x.LastNotifyDate) : tempMembers.OrderBy(x => x.LastNotifyDate),
                    _ => tempMembers.OrderBy(x => x.ContinueDate).ThenBy(x => x.Mid)
                };

                var finalForm = sortedQuery.ToList();

                model.TotalCount = finalForm.Count;
                model.Members = finalForm.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                var logs = await _logPushService.GetExpireNotifyLogsAsync(memberSid);

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    logs = logs.Where(x =>
                        (x.MemberMid ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        (x.NotifyType ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        (x.PushType ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        (x.Title ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                var isDesc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
                var sortedLogs = sortField switch
                {
                    "member_mid" => isDesc ? logs.OrderByDescending(x => x.MemberMid) : logs.OrderBy(x => x.MemberMid),
                    "notify_type" => isDesc ? logs.OrderByDescending(x => x.NotifyType) : logs.OrderBy(x => x.NotifyType),
                    "push_type" => isDesc ? logs.OrderByDescending(x => x.PushType) : logs.OrderBy(x => x.PushType),
                    "title" => isDesc ? logs.OrderByDescending(x => x.Title) : logs.OrderBy(x => x.Title),
                    "result" => isDesc ? logs.OrderByDescending(x => x.Result) : logs.OrderBy(x => x.Result),
                    "SendDate" => isDesc ? logs.OrderByDescending(x => x.SendDate) : logs.OrderBy(x => x.SendDate),
                    _ => logs.OrderByDescending(x => x.SendDate)
                };

                var finalLogs = sortedLogs.ToList();
                model.TotalCount = finalLogs.Count;
                model.Logs = finalLogs.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }

            return model;
        }
        // 遞迴輔助函式：取得指定 parentSid 轄下的所有層級下屬
        private async Task<List<DataMember>> GetAllDescendantsRecursiveAsync(string parentSid)
        {
            var result = new List<DataMember>();
            var directChildren = await _memberService.GetChildrenAsync(parentSid); // 撈第一層

            foreach (var child in directChildren)
            {
                result.Add(child);
                // 遞迴撈取第 2, 3... n 層
                var grandChildren = await GetAllDescendantsRecursiveAsync(child.Sid);
                result.AddRange(grandChildren);
            }

            return result;
        }
        #endregion

        #region 3. 手動發送 LINE 通知

        public async Task<(bool Success, string Message)> SendManualLineNoticeAsync(string sid)
        {
            var member = await _memberService.GetBySidAsync(sid);

            if (member == null)
            {
                // 💡 呼叫基底 BaseService 的 WriteLog (系統軌跡)
                await base.WriteLog(
                    actionType: ActionTypes.EXPIRE_LINE,
                    targetSid: sid,
                    targetMid: "UNKNOWN",
                    moduleName: ModuleNames.ExpireNotice,
                    actionName: ActionNames.SendExipredNotice4Line,
                    description: $"[FAIL]發送通知失敗! 原因：會員不存在 (Sid: {sid})",
                    result: "失敗"
                );
                return (false, "會員不存在");
            }

            var title = "會員到期通知";
            var message = $"親愛的 {member.Name}，您的會員即將到期，請儘速完成續約。";

            try
            {
                var success = await _lineService.PushAsync(member.LineUserId ?? "", message);

                if (!success)
                {
                    // 1. 寫入操作軌跡日誌 (基底 Service)
                    await base.WriteLog(
                        actionType: ActionTypes.EXPIRE_LINE,
                        targetSid: member.Sid,
                        targetMid: member.Mid,
                        moduleName: ModuleNames.ExpireNotice,
                        actionName: ActionNames.SendExipredNotice4Line,
                        description: $"[FAIL]手動發送 LINE 通知至 ({member.Mid}) 失敗!",
                        result: "失敗"
                    );

                    // 2. 寫入發送明細日誌 (本類別私有方法)
                    await WritePushLog(member, "LINE", title, message, "FAIL", "手動發送失敗");
                    return (false, "手動發送失敗");
                }

                // 發送成功：操作軌跡
                await base.WriteLog(
                    actionType: ActionTypes.EXPIRE_LINE,
                    targetSid: member.Sid,
                    targetMid: member.Mid,
                    moduleName: ModuleNames.ExpireNotice,
                    actionName: ActionNames.SendExipredNotice4Line,
                    description: $"[SUCCESS]發送通知至 ({member.Mid}) 成功: {message}",
                    result: "成功"
                );

                // 發送成功：明細紀錄
                await WritePushLog(member, "LINE", title, message, "SUCCESS", "手動發送成功");
                return (true, "發送成功");
            }
            catch (Exception ex)
            {
                // 異常：操作軌跡
                await base.WriteLog(
                    actionType: ActionTypes.EXPIRE_LINE,
                    targetSid: member.Sid,
                    targetMid: member.Mid,
                    moduleName: ModuleNames.ExpireNotice,
                    actionName: ActionNames.SendExipredNotice4Line,
                    description: $"[FAIL]發送通知至 ({member.Mid}) 異常: {ex.Message}",
                    result: "失敗"
                );

                // 異常：明細紀錄
                await WritePushLog(member, "LINE", title, message, "FAIL", ex.Message);
                return (false, ex.Message);
            }
        }

        #endregion

        #region 4. 手動發送 FCM 通知
        public async Task<(bool Success, string Message)> SendManualFcmNoticeAsync(string sid)
        {
            var member = await _memberService.GetBySidAsync(sid);

            if (member == null)
            {
                // 💡 修正盲點：當 member 為 null 時，改用傳入的 sid，防禦 NullReferenceException
                await base.WriteLog(
                    actionType: ActionTypes.EXPIRE_FCM,
                    targetSid: sid,
                    targetMid: "UNKNOWN",
                    moduleName: ModuleNames.ExpireNotice,
                    actionName: ActionNames.SendExipredNotice4FCM,
                    description: $"[FAIL]發送通知失敗! 原因：會員不存在 (Sid: {sid})",
                    result: "失敗"
                );
                return (false, "會員不存在");
            }

            var title = "會員到期通知";
            var message = $"親愛的 {member.Name}，您的會員即將到期，請儘速完成續約。";

            try
            {
                // 呼叫底層 FCM 服務（請確保 _fcmService 已在 BaseService 或本類別中宣告並注入）
                var success = await _fcmService.SendAsync(member.FcmToken ?? "", title, message);

                if (!success)
                {
                    // 1. 寫入操作軌跡日誌 (基底 Service)
                    await base.WriteLog(
                        actionType: ActionTypes.EXPIRE_FCM,
                        targetSid: member.Sid,
                        targetMid: member.Mid,
                        moduleName: ModuleNames.ExpireNotice,
                        actionName: ActionNames.SendExipredNotice4FCM,
                        description: $"[FAIL]手動發送 FCM 通知至 ({member.Mid}) 失敗!",
                        result: "失敗"
                    );

                    // 2. 寫入發送明細日誌 (本類別私有方法，修正原本寫成 LINE 的管道 Bug)
                    await WritePushLog(member, "FCM", title, message, "FAIL", "手動發送失敗");
                    return (false, "手動發送失敗");
                }

                // 發送成功：操作軌跡
                await base.WriteLog(
                    actionType: ActionTypes.EXPIRE_FCM,
                    targetSid: member.Sid,
                    targetMid: member.Mid,
                    moduleName: ModuleNames.ExpireNotice,
                    actionName: ActionNames.SendExipredNotice4FCM,
                    description: $"[SUCCESS]發送通知至 ({member.Mid}) 成功: {message}",
                    result: "成功"
                );

                // 發送成功：明細紀錄
                await WritePushLog(member, "FCM", title, message, "SUCCESS", "手動發送成功");
                return (true, "發送成功");
            }
            catch (Exception ex)
            {
                // 異常：操作軌跡
                await base.WriteLog(
                    actionType: ActionTypes.EXPIRE_FCM,
                    targetSid: member.Sid,
                    targetMid: member.Mid,
                    moduleName: ModuleNames.ExpireNotice,
                    actionName: ActionNames.SendExipredNotice4FCM,
                    description: $"[FAIL]發送通知至 ({member.Mid}) 異常: {ex.Message}",
                    result: "失敗"
                );

                // 異常：明細紀錄
                await WritePushLog(member, "FCM", title, message, "FAIL", ex.Message);
                return (false, ex.Message);
            }
        }
        #endregion

        #region 5. 手動發送 EMAIL 通知
        public async Task<(bool Success, string Message)> SendManualEmailNoticeAsync(string sid)
        {
            var member = await _memberService.GetBySidAsync(sid);

            if (member == null)
            {
                // 💡 修正盲點：當 member 為 null 時，改用傳入的 sid，防禦 NullReferenceException
                await base.WriteLog(
                    actionType: ActionTypes.EXPIRE_EMAIL,
                    targetSid: sid,
                    targetMid: "UNKNOWN",
                    moduleName: ModuleNames.ExpireNotice,
                    actionName: ActionNames.SendExipredNotice4Email,
                    description: $"[FAIL]發送通知失敗! 原因：會員不存在 (Sid: {sid})",
                    result: "失敗"
                );
                return (false, "會員不存在");
            }

            var title = "會員到期通知";
            var message = $"親愛的 {member.Name}，您的會員即將到期，請儘速完成續約。";

            try
            {
                // 呼叫底層 Email 服務
                var success = await _emailService.SendAsync(member.Email ?? "", title, message);

                if (!success)
                {
                    // 1. 寫入操作軌跡日誌 (基底 Service)
                    await base.WriteLog(
                        actionType: ActionTypes.EXPIRE_EMAIL,
                        targetSid: member.Sid,
                        targetMid: member.Mid,
                        moduleName: ModuleNames.ExpireNotice,
                        actionName: ActionNames.SendExipredNotice4Email,
                        description: $"[FAIL]手動發送 EMAIL 通知至 ({member.Mid}) 失敗!",
                        result: "失敗"
                    );

                    // 2. 寫入發送明細日誌 (修正原代碼寫成 LINE 的 Bug，統一改為 EMAIL)
                    await WritePushLog(member, "EMAIL", title, message, "FAIL", "手動發送失敗");
                    return (false, "手動發送失敗");
                }

                // 發送成功：操作軌跡
                await base.WriteLog(
                    actionType: ActionTypes.EXPIRE_EMAIL,
                    targetSid: member.Sid,
                    targetMid: member.Mid,
                    moduleName: ModuleNames.ExpireNotice,
                    actionName: ActionNames.SendExipredNotice4Email,
                    description: $"[SUCCESS]發送通知至 ({member.Mid}) 成功: {message}",
                    result: "成功"
                );

                // 發送成功：明細紀錄
                await WritePushLog(member, "EMAIL", title, message, "SUCCESS", "手動發送成功");
                return (true, "發送成功");
            }
            catch (Exception ex)
            {
                // 異常：操作軌跡
                await base.WriteLog(
                    actionType: ActionTypes.EXPIRE_EMAIL,
                    targetSid: member.Sid,
                    targetMid: member.Mid,
                    moduleName: ModuleNames.ExpireNotice,
                    actionName: ActionNames.SendExipredNotice4Email,
                    description: $"[FAIL]發送通知至 ({member.Mid}) 異常: {ex.Message}",
                    result: "失敗"
                );

                // 異常：明細紀錄
                await WritePushLog(member, "EMAIL", title, message, "FAIL", ex.Message);
                return (false, ex.Message);
            }
        }
        #endregion

        #region 6. 批次發送 LINE 通知
        public async Task<(bool Success, int SuccessCount, int FailureCount)> SendBatchLineNoticeAsync(List<string> sids)
        {
            bool overallSuccess = true;
            int successCount = 0;
            int failureCount = 0;

            if (sids == null || !sids.Any())
            {
                return (true, 0, 0);
            }

            foreach (var sid in sids)
            {
                DataMember? member = null;
                var title = "會員到期通知";
                var message = "親愛的 {0}，會員即將到期";

                try
                {
                    // 1. 取得會員資料
                    member = await _memberService.GetBySidAsync(sid);

                    if (member == null)
                    {
                        // 查無會員則跳過，不計入成功或失敗件數（或依商業邏輯計入失敗）
                        continue;
                    }

                    string formattedMessage = string.Format(message, member.Name);

                    // 2. 呼叫底層 LINE 推播服務（請確保 _lineService 已在類別中宣告並注入）
                    var success = await _lineService.PushAsync(member.LineUserId ?? "", formattedMessage);

                    if (!success)
                    {
                        // 發送失敗：操作軌跡日誌
                        await base.WriteLog(
                            actionType: ActionTypes.EXPIRE_LINE,
                            targetSid: member.Sid,
                            targetMid: member.Mid,
                            moduleName: ModuleNames.ExpireNotice,
                            actionName: ActionNames.SendExipredNotice4BatchLine,
                            description: $"[FAIL]批次發送通知至 ({member.Mid})失敗!",
                            result: "失敗"
                        );

                        // 發送失敗：明細紀錄
                        await WritePushLog(member, "BATCH_LINE", title, formattedMessage, "FAIL", "批次發送失敗");

                        failureCount++;
                        overallSuccess = false;
                    }
                    else
                    {
                        // 發送成功：操作軌跡日誌
                        await base.WriteLog(
                            actionType: ActionTypes.EXPIRE_LINE,
                            targetSid: member.Sid,
                            targetMid: member.Mid,
                            moduleName: ModuleNames.ExpireNotice,
                            actionName: ActionNames.SendExipredNotice4BatchLine,
                            description: $"[SUCCESS]批次發送通知至 ({member.Mid})成功: {formattedMessage}",
                            result: "成功"
                        );

                        // 發送成功：明細紀錄
                        await WritePushLog(member, "BATCH_LINE", title, formattedMessage, "SUCCESS", "批次發送成功");

                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    overallSuccess = false;
                    failureCount++;

                    // 💡 優化：不再重複進行 _memberService.GetBySidAsync(sid) 查詢，防禦二次崩潰
                    if (member != null)
                    {
                        string formattedMessage = string.Format(message, member.Name);

                        await base.WriteLog(
                            actionType: ActionTypes.EXPIRE_LINE,
                            targetSid: member.Sid,
                            targetMid: member.Mid,
                            moduleName: ModuleNames.ExpireNotice,
                            actionName: ActionNames.SendExipredNotice4BatchLine,
                            description: $"[FAIL]批次發送通知至 ({member.Mid})失敗: {ex.Message}",
                            result: "失敗"
                        );

                        await WritePushLog(member, "BATCH_LINE", title, formattedMessage, "FAIL", ex.Message);
                    }
                    else
                    {
                        // 連 member 都取不出來或為 null 時的極端異常防禦日誌
                        await base.WriteLog(
                            actionType: ActionTypes.EXPIRE_LINE,
                            targetSid: sid,
                            targetMid: "UNKNOWN",
                            moduleName: ModuleNames.ExpireNotice,
                            actionName: ActionNames.SendExipredNotice4BatchLine,
                            description: $"[FAIL]批次發送處理異常 (Sid: {sid}): {ex.Message}",
                            result: "失敗"
                        );
                    }
                }
            }

            return (overallSuccess, successCount, failureCount);
        }
        #endregion

        #region 7. 批次發送 FCM 通知
        public async Task<(bool Success, int SuccessCount, int FailureCount)> SendBatchFcmNoticeAsync(List<string> sids)
        {
            bool overallSuccess = true;
            int successCount = 0;
            int failureCount = 0;

            if (sids == null || !sids.Any())
            {
                return (true, 0, 0);
            }

            foreach (var sid in sids)
            {
                DataMember? member = null;
                var title = "會員到期通知";
                var message = "親愛的 {0}，會員即將到期";

                try
                {
                    // 1. 取得會員資料
                    member = await _memberService.GetBySidAsync(sid);

                    if (member == null)
                    {
                        // 查無會員則直接跳過，不中斷其他會員的發送
                        continue;
                    }

                    string formattedMessage = string.Format(message, member.Name);

                    // 2. 呼叫底層 FCM 推播服務（請確保 _fcmService 已在類別中宣告並注入）
                    var success = await _fcmService.SendAsync(member.FcmToken ?? "", title, formattedMessage);

                    if (!success)
                    {
                        // 發送失敗：操作軌跡日誌
                        await base.WriteLog(
                            actionType: ActionTypes.EXPIRE_FCM,
                            targetSid: member.Sid,
                            targetMid: member.Mid,
                            moduleName: ModuleNames.ExpireNotice,
                            actionName: ActionNames.SendExipredNotice4BatchFCM,
                            description: $"[FAIL]批次發送通知至 ({member.Mid})失敗!",
                            result: "失敗"
                        );

                        // 發送失敗：明細紀錄
                        await WritePushLog(member, "BATCH_FCM", title, formattedMessage, "FAIL", "批次發送失敗");

                        failureCount++;
                        overallSuccess = false;
                    }
                    else
                    {
                        // 發送成功：操作軌跡日誌
                        await base.WriteLog(
                            actionType: ActionTypes.EXPIRE_FCM,
                            targetSid: member.Sid,
                            targetMid: member.Mid,
                            moduleName: ModuleNames.ExpireNotice,
                            actionName: ActionNames.SendExipredNotice4BatchFCM,
                            description: $"[SUCCESS]批次發送通知至 ({member.Mid})成功: {formattedMessage}",
                            result: "成功"
                        );

                        // 發送成功：明細紀錄
                        await WritePushLog(member, "BATCH_FCM", title, formattedMessage, "SUCCESS", "批次發送成功");

                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    overallSuccess = false;
                    failureCount++;

                    // 💡 優化：移除 catch 內重複的查詢，防禦二次崩潰
                    if (member != null)
                    {
                        string formattedMessage = string.Format(message, member.Name);

                        await base.WriteLog(
                            actionType: ActionTypes.EXPIRE_FCM,
                            targetSid: member.Sid,
                            targetMid: member.Mid,
                            moduleName: ModuleNames.ExpireNotice,
                            actionName: ActionNames.SendExipredNotice4BatchFCM,
                            description: $"[FAIL]批次發送通知至 ({member.Mid})失敗: {ex.Message}",
                            result: "失敗"
                        );

                        await WritePushLog(member, "BATCH_FCM", title, formattedMessage, "FAIL", ex.Message);
                    }
                    else
                    {
                        // member 本身查不到或於讀取階段即崩潰時的極端防禦
                        await base.WriteLog(
                            actionType: ActionTypes.EXPIRE_FCM,
                            targetSid: sid,
                            targetMid: "UNKNOWN",
                            moduleName: ModuleNames.ExpireNotice,
                            actionName: ActionNames.SendExipredNotice4BatchFCM,
                            description: $"[FAIL]批次發送處理異常 (Sid: {sid}): {ex.Message}",
                            result: "失敗"
                        );
                    }
                }
            }

            return (overallSuccess, successCount, failureCount);
        }
        #endregion

        #region 8. 批次發送 EMAIL 通知
        public async Task<(bool Success, int SuccessCount, int FailureCount)> SendBatchEmailNoticeAsync(List<string> sids)
        {
            bool overallSuccess = true;
            int successCount = 0;
            int failureCount = 0;

            if (sids == null || !sids.Any())
            {
                return (true, 0, 0);
            }

            foreach (var sid in sids)
            {
                DataMember? member = null;
                var title = "會員到期通知";
                var message = "親愛的 {0}，會員即將到期";

                try
                {
                    // 1. 取得會員資料
                    member = await _memberService.GetBySidAsync(sid);

                    if (member == null)
                    {
                        // 查無會員則跳過，不干擾其他會員的批次發送流程
                        continue;
                    }

                    string formattedMessage = string.Format(message, member.Name);

                    // 2. 呼叫底層 Email 服務（請確保 _emailService 已在類別中宣告並注入）
                    var success = await _emailService.SendAsync(member.Email ?? "", title, formattedMessage);

                    if (!success)
                    {
                        // 發送失敗：操作軌跡日誌
                        await base.WriteLog(
                            actionType: ActionTypes.EXPIRE_EMAIL,
                            targetSid: member.Sid,
                            targetMid: member.Mid,
                            moduleName: ModuleNames.ExpireNotice,
                            actionName: ActionNames.SendExipredNotice4BatchEmail,
                            description: $"[FAIL]批次發送通知至 ({member.Mid})失敗!",
                            result: "失敗"
                        );

                        // 發送失敗：明細紀錄
                        await WritePushLog(member, "BATCH_EMAIL", title, formattedMessage, "FAIL", "批次發送失敗");

                        failureCount++;
                        overallSuccess = false;
                    }
                    else
                    {
                        // 發送成功：操作軌跡日誌
                        await base.WriteLog(
                            actionType: ActionTypes.EXPIRE_EMAIL,
                            targetSid: member.Sid,
                            targetMid: member.Mid,
                            moduleName: ModuleNames.ExpireNotice,
                            actionName: ActionNames.SendExipredNotice4BatchEmail,
                            description: $"[SUCCESS]批次發送通知至 ({member.Mid})成功: {formattedMessage}",
                            result: "成功"
                        );

                        // 發送成功：明細紀錄
                        await WritePushLog(member, "BATCH_EMAIL", title, formattedMessage, "SUCCESS", "批次發送成功");

                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    overallSuccess = false;
                    failureCount++;

                    // 💡 優化：移除 catch 區塊內重複查詢資料庫的設計，阻斷二次潰堤
                    if (member != null)
                    {
                        string formattedMessage = string.Format(message, member.Name);

                        await base.WriteLog(
                            actionType: ActionTypes.EXPIRE_EMAIL,
                            targetSid: member.Sid,
                            targetMid: member.Mid,
                            moduleName: ModuleNames.ExpireNotice,
                            actionName: ActionNames.SendExipredNotice4BatchEmail,
                            description: $"[FAIL]批次發送通知至 ({member.Mid})失敗: {ex.Message}",
                            result: "失敗"
                        );

                        await WritePushLog(member, "BATCH_EMAIL", title, formattedMessage, "FAIL", ex.Message);
                    }
                    else
                    {
                        // 連 member 基本資料讀取時就出現異常的防禦紀錄
                        await base.WriteLog(
                            actionType: ActionTypes.EXPIRE_EMAIL,
                            targetSid: sid,
                            targetMid: "UNKNOWN",
                            moduleName: ModuleNames.ExpireNotice,
                            actionName: ActionNames.SendExipredNotice4BatchEmail,
                            description: $"[FAIL]批次發送處理異常 (Sid: {sid}): {ex.Message}",
                            result: "失敗"
                        );
                    }
                }
            }

            return (overallSuccess, successCount, failureCount);
        }
        #endregion

        #region 9. 匯出到期會員通知檔案

        public async Task<FileDownloadModel> ExportExpireNoticeFileAsync(string memberSid, string type, string keyword)
        {
            // 1. 取得會員本人與所有下線資料
            var loginMember = await _memberService.GetBySidAsync(memberSid);
            var allChildrens = await _memberService.GetFullChildrenAsync(memberSid);

            // 2. 合併會員本人與下線
            var allMembers = new List<DataMember>();
            if (loginMember != null)
            {
                allMembers.Add(loginMember);
            }
            allMembers.AddRange(allChildrens);

            // 2. 建立 ParentSid -> 子會員清單 的 Lookup，以及 Sid -> Member 的字典快取
            var parentLookup = allMembers
                .Where(m => !string.IsNullOrEmpty(m.ParentSid))
                .ToLookup(m => m.ParentSid!);

            var memberDict = allMembers.ToDictionary(m => m.Sid, m => m);

            // 3. 遞迴撈取「本人」與「所有第 N 層下線」
            var allTargetMembers = new List<DataMember>();
            if (memberDict.TryGetValue(memberSid, out var currentMember))
            {
                allTargetMembers.Add(currentMember);
                CollectDescendantsFromMemory(memberSid, parentLookup, allTargetMembers);
            }

            // 若完全查無資料直接產生空檔
            if (!allTargetMembers.Any())
            {
                return BuildDownloadModel(new List<DataMember>(), type, memberDict);
            }

            var today = DateTime.Today;
            var hasKeyword = !string.IsNullOrWhiteSpace(keyword);

            // 3. 使用 LINQ 進行邏輯篩選
            var filteredData = allTargetMembers
                .Where(m => m.Avalible == "Y" && m.ContinueDate.HasValue)
                .Where(m =>
                {
                    // 計算剩餘天數 (今日天數差)
                    var remainDays = (m.ContinueDate!.Value.Date - today).Days;

                    // 條件修改：只要剩餘天數 <= HintDays 即符合 (包含 remainDays < 0 的已逾期會員)
                    return remainDays <= m.HintDays;
                })
                .Where(m => !hasKeyword ||
                    (m.Mid ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (m.Name ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (m.Email ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // 2. 根據類型分流至對應的二進位資料產生器
            return BuildDownloadModel(filteredData, type, memberDict);
        }
        private FileDownloadModel BuildDownloadModel(List<DataMember> data, string type, Dictionary<string, DataMember> memberDict)
        {
            if (string.Equals(type, "csv", StringComparison.OrdinalIgnoreCase))
            {
                return GenerateExpireCsv(data, memberDict);
            }

            return GenerateExpireExcel(data, memberDict);
        }
        /// <summary>
        /// 記憶體內部快速展開所有 N 層下線（無需連線 DB）
        /// </summary>
        private void CollectDescendantsFromMemory(string parentSid, ILookup<string, DataMember> lookup, List<DataMember> resultList)
        {
            foreach (var child in lookup[parentSid])
            {
                resultList.Add(child);
                CollectDescendantsFromMemory(child.Sid, lookup, resultList);
            }
        }
        /// <summary>
        /// 透過 ParentSid 動態查找上線會員的 姓名 (MID)
        /// </summary>
        private string GetParentInfoText(string? parentSid, Dictionary<string, DataMember> memberDict)
        {
            if (string.IsNullOrEmpty(parentSid)) return "";

            if (memberDict.TryGetValue(parentSid, out var parent))
            {
                return $"{parent.Name} ({parent.Mid})".Trim();
            }

            return "";
        }
        private FileDownloadModel GenerateExpireCsv(List<DataMember> data, Dictionary<string, DataMember> memberDict)
        {
            var sb = new StringBuilder();
            //sb.AppendLine("編號,會員編號,姓名,手機,Email,角色,到期日,到期狀態,上線");
            sb.AppendLine("編號,會員編號,姓名,角色,到期日,到期狀態,上線");

            var today = DateTime.Today;
            int idx = 0;

            foreach (var m in data)
            {
                var remainDays = m.ContinueDate.HasValue
                    ? (m.ContinueDate.Value.Date - today).Days
                    : 0;
                var remainDaysStr = (remainDays > 0) ? $"即將到期({remainDays}後到期)" : "已逾期";

                // 從字典查找 Parent 資料
                string parentInfo = GetParentInfoText(m.ParentSid, memberDict);

                idx++;
                sb.AppendLine($"{idx}," +
                              $"\"{m.Mid}\"," +
                              $"\"{m.Name}\"," +
                              //$"\"{m.Mobile}\"," +
                              //$"\"{m.Email}\"," +
                              $"\"{MemberRoles.GetName(m.Role)}\"," +
                              $"{m.ContinueDate:yyyy-MM-dd}," +
                              $"{remainDaysStr}," +
                              $"\"{parentInfo}\"");
            }

            var fileBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();

            return new FileDownloadModel
            {
                FileContents = fileBytes,
                ContentType = "text/csv",
                FileName = $"到期會員清單_{DateTime.Now:yyyyMMddHHmmss}.csv"
            };
        }

        private FileDownloadModel GenerateExpireExcel(List<DataMember> data, Dictionary<string, DataMember> memberDict)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("到期會員");

            //string[] headers = { "編號", "會員編號", "姓名", "手機", "Email", "角色", "到期日", "到期狀態", "上線" };
            string[] headers = { "編號", "會員編號", "姓名", "角色", "到期日", "到期狀態", "上線" };
            for (int col = 0; col < headers.Length; col++)
            {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            var today = DateTime.Today;
            var row = 2;
            int idx = 0;

            foreach (var m in data)
            {
                var remainDays = m.ContinueDate.HasValue
                    ? (m.ContinueDate.Value.Date - today).Days
                    : 0;
                var remainDaysStr = (remainDays > 0) ? $"即將到期({remainDays}後到期)" : "已逾期";

                int iColumn = 1; // 每列重設欄位索引

                ws.Cell(row, iColumn++).Value = ++idx;
                ws.Cell(row, iColumn++).Value = m.Mid;
                ws.Cell(row, iColumn++).Value = m.Name;
                //ws.Cell(row, iColumn++).Value = m.Mobile;
                //ws.Cell(row, iColumn++).Value = m.Email;
                ws.Cell(row, iColumn++).Value = MemberRoles.GetName(m.Role);
                ws.Cell(row, iColumn++).Value = m.ContinueDate?.ToString("yyyy-MM-dd");
                ws.Cell(row, iColumn++).Value = remainDaysStr;
                ws.Cell(row, iColumn++).Value = GetParentInfoText(m.ParentSid, memberDict);

                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);

            return new FileDownloadModel
            {
                FileContents = stream.ToArray(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"到期會員清單_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            };
        }
        #endregion

        #region 10. 匯出到期通知歷史紀錄檔案
        public async Task<FileDownloadModel> ExportExpireHistoryFileAsync(string memberSid, string type, string keyword)
        {
            // 1. 取得該管理員權限下的所有推播歷史紀錄
            var logs = await _logPushService.GetExpireNotifyLogsAsync(memberSid);
            var logList = logs.ToList();

            // 2. 關鍵字過濾
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                logList = logList.Where(x =>
                    (x.MemberMid ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (x.NotifyType ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (x.PushType ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (x.Title ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (x.Result ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // 3. 根據類型分流產生檔案二進位資料
            if (type?.ToLower() == "csv")
            {
                return GenerateHistoryCsv(logList);
            }

            return GenerateHistoryExcel(logList);
        }

        private FileDownloadModel GenerateHistoryCsv(List<PushHistoryViewModel> data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("時間,MID,通知類型,發送方式,標題,結果");

            foreach (var item in data)
            {
                sb.AppendLine(
                    $"{item.SendDate:yyyy-MM-dd HH:mm:ss}," +
                    $"{item.MemberMid}," +
                    $"{item.NotifyType}," +
                    $"{item.PushType}," +
                    $"{item.Title}," +
                    $"{item.Result}");
            }

            var fileBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();

            return new FileDownloadModel
            {
                FileContents = fileBytes,
                ContentType = "text/csv",
                FileName = $"通知紀錄_{DateTime.Now:yyyyMMddHHmmss}.csv"
            };
        }

        private FileDownloadModel GenerateHistoryExcel(List<PushHistoryViewModel> data)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("通知紀錄");

            ws.Cell(1, 1).Value = "時間";
            ws.Cell(1, 2).Value = "MID";
            ws.Cell(1, 3).Value = "通知類型";
            ws.Cell(1, 4).Value = "發送方式";
            ws.Cell(1, 5).Value = "標題";
            ws.Cell(1, 6).Value = "結果";

            var row = 2;
            foreach (var item in data)
            {
                ws.Cell(row, 1).Value = item.SendDate;
                ws.Cell(row, 2).Value = item.MemberMid;
                ws.Cell(row, 3).Value = item.NotifyType;
                ws.Cell(row, 4).Value = item.PushType;
                ws.Cell(row, 5).Value = item.Title;
                ws.Cell(row, 6).Value = item.Result;
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);

            return new FileDownloadModel
            {
                FileContents = stream.ToArray(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"通知紀錄_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            };
        }
        #endregion
    }
}