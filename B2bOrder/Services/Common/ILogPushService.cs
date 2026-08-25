using B2bOrder.Data;
using B2bOrder.Models;
using B2bOrder.Models.Common;
using B2bOrder.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Services.Common
{
    public interface ILogPushService
    {
        Task<Dictionary<string, DateTime>> GetLastNotifyMapAsync();
        Task<List<PushHistoryViewModel>> GetExpireNotifyLogsAsync(string memberSid, string keyword = "");
        Task<int> GetMonthNotifyCountAsync();
        Task<decimal> GetMonthSuccessRateAsync();
        Task<DateTime?> GetLastNotifyDateAsync(string memberSid);
        Task<IEnumerable<LogPush>> GetAllAsync();
        Task<LogPush?> GetByIdAsync(int nid);
        Task CreateAsync(LogPush entity);
        Task UpdateAsync(LogPush entity);
        Task DeleteAsync(int nid);

        /// <summary>
        /// 檢查指定會員在指定時間範圍內是否已發送過通知
        /// </summary>
        /// <param name="memberSid">會員識別碼</param>
        /// <param name="notifyType">通知類型（可選，若為 null 則檢查所有類型）</param>
        /// <param name="withinMinutes">檢查最近幾分鐘內（預設 30 分鐘）</param>
        /// <returns>若最近有發送過回傳 true，否則回傳 false</returns>
        Task<bool> HasSentRecentlyAsync(string memberSid, string? notifyType = null, int cooldownDays = 10);
    }
    public class LogPushService : ILogPushService
    {
        private readonly ApplicationDbContext _db;
        public LogPushService(ApplicationDbContext db) => _db = db;
        public async Task<List<PushHistoryViewModel>> GetExpireNotifyLogsAsync(string memberSid, string keyword = "")
        {
            var query =
                _db.LogPushes
                .Where(x =>
                    x.CreateSid == memberSid)
                   .AsQueryable();

            // 只顯示到期通知
            query = query.Where(x =>
                x.NotifyType ==
                NotifyTypes.ExpireNotice);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    (x.MemberMid ?? "")
                        .Contains(keyword) ||

                    x.Title
                        .Contains(keyword) ||

                    x.Message
                        .Contains(keyword));
            }

            return await query
                .OrderByDescending(x =>
                    x.SendDate)
                .Select(x =>
                    new PushHistoryViewModel
                    {
                        Nid =
                            x.Nid,

                        MemberSid =
                            x.MemberSid,

                        MemberMid =
                            x.MemberMid ?? "",

                        NotifyType =
                            x.NotifyType,

                        PushType =
                            x.PushType,

                        Title =
                            x.Title,

                        Message =
                            x.Message,

                        Result =
                            x.Result ?? "",

                        ResponseMessage =
                            x.ResponseMessage,

                        SendDate =
                            x.SendDate
                    })
                .ToListAsync();
        }
        public async Task<Dictionary<string, DateTime>> GetLastNotifyMapAsync()
        {
            return await _db.LogPushes
                .Where(x =>
                    x.NotifyType ==
                    NotifyTypes.ExpireNotice)

                .GroupBy(x =>
                    x.MemberSid)

                .Select(x =>
                    new
                    {
                        x.Key,

                        LastDate =
                            x.Max(y =>
                                y.SendDate)
                    })

                .ToDictionaryAsync(
                    x => x.Key,
                    x => x.LastDate);
        }
        public async Task<int> GetMonthNotifyCountAsync()
        {
            var now = DateTime.Now;

            return await _db.LogPushes
                .CountAsync(x =>
                    x.SendDate.Year == now.Year &&
                    x.SendDate.Month == now.Month);
        }
        public async Task<decimal> GetMonthSuccessRateAsync()
        {
            var now = DateTime.Now;

            var total =
                await _db.LogPushes
                    .CountAsync(x =>
                        x.SendDate.Year == now.Year &&
                        x.SendDate.Month == now.Month);

            if (total == 0)
                return 100;

            var success =
                await _db.LogPushes
                    .CountAsync(x =>
                        x.SendDate.Year == now.Year &&
                        x.SendDate.Month == now.Month &&
                        x.Result == "SUCCESS");

            return Math.Round(
                success * 100m / total,
                1);
        }

        public async Task<DateTime?> GetLastNotifyDateAsync(string memberSid)
        {
            return await _db.LogPushes
                .Where(x =>
                    x.MemberSid == memberSid)
                .OrderByDescending(x =>
                    x.SendDate)
                .Select(x =>
                    (DateTime?)x.SendDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<LogPush>> GetAllAsync() => await _db.LogPushes.AsNoTracking().ToListAsync();

        public async Task<LogPush?> GetByIdAsync(int nid) => await _db.LogPushes.FindAsync(nid);

        public async Task CreateAsync(LogPush entity)
        {
            await _db.LogPushes.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(LogPush entity)
        {
            _db.LogPushes.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int nid)
        {
            var e = await _db.LogPushes.FindAsync(nid);
            if (e is null) return;
            _db.LogPushes.Remove(e);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// 判斷指定會員在指定天數內是否已經發送過通知（預設 10 天內不重複發）
        /// </summary>
        public async Task<bool> HasSentRecentlyAsync(string memberSid, string? notifyType = null, int cooldownDays = 10)
        {
            // 計算門檻時間：當前時間往回推 10 天
            // 例如：今日為 8/10，thresholdTime 即為 8/1 的同一時間
            // 只要 SendDate >= 8/1，代表還在 10 天的冷卻期內
            var thresholdTime = DateTime.Now.AddDays(-cooldownDays);

            var query = _db.LogPushes
                .AsNoTracking()
                .Where(x => x.MemberSid == memberSid && x.SendDate >= thresholdTime);

            // 若有指定通知類型（例如到期通知），則再做類型篩選
            if (!string.IsNullOrEmpty(notifyType))
            {
                query = query.Where(x => x.NotifyType == notifyType);
            }

            // 使用 AnyAsync 轉譯成 EXISTS 查詢，資料庫找到第一筆資料就停止掃描，效能最佳
            return await query.AnyAsync();
        }
    }
}