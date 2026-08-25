using B2bOrder.Data;
using B2bOrder.Models.Common;
using B2bOrder.Models.Db;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace B2bOrder.Services.Common
{
    public interface ILogActionService
    {
        #region CRUD
        Task<IEnumerable<LogAction>> GetAllAsync();

        Task<LogAction?> GetByIdAsync(int nid);

        Task CreateAsync(LogAction entity);

        Task UpdateAsync(LogAction entity);

        Task DeleteAsync(int nid);
        #endregion

        #region Query
        Task<IEnumerable<LogAction>> GetByUserSidAsync(string userSid);

        Task<IEnumerable<LogAction>> GetByModuleAsync(string moduleName);

        Task<IEnumerable<LogAction>> GetByActionTypeAsync(string actionType);

        Task<IEnumerable<LogAction>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate);
        #endregion

        #region Write Log
        Task WriteAsync(
            HttpContext httpContext,
            string actionType,
            string? targetSid = null,
            string? targetMid = null,
            string? moduleName = null,
            string? actionName = null,
            string? description = null,
            string? moreDescription = null,
            string result = "成功"
        );
        #endregion

        #region 新增：支援完整搜尋條件的資料庫端分頁方法
        Task<PagedResult<LogAction>> GetPagedLogsAsync(
            string userSid,
            string startDate,
            string endDate,
            string pageName,
            string actionType,
            string userAccount,
            string keyword,
            int page,
            int pageSize);
        #endregion
    }
    public class LogActionService : ILogActionService
    {
        private readonly ApplicationDbContext _db;

        public LogActionService(ApplicationDbContext db)
        {
            _db = db;
        }

        #region CRUD

        public async Task<IEnumerable<LogAction>> GetAllAsync()
        {
            return await _db.LogActions
                .AsNoTracking()
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        public async Task<LogAction?> GetByIdAsync(int nid)
        {
            return await _db.LogActions.FindAsync(nid);
        }

        public async Task CreateAsync(LogAction entity)
        {
            await _db.LogActions.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(LogAction entity)
        {
            _db.LogActions.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int nid)
        {
            var entity = await _db.LogActions.FindAsync(nid);

            if (entity == null)
                return;

            _db.LogActions.Remove(entity);
            await _db.SaveChangesAsync();
        }

        #endregion

        #region Query

        public async Task<IEnumerable<LogAction>> GetByUserSidAsync(
            string userSid)
        {
            return await _db.LogActions
                .AsNoTracking()
                .Where(x => x.UserSid == userSid)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogAction>> GetByModuleAsync(
            string moduleName)
        {
            return await _db.LogActions
                .AsNoTracking()
                .Where(x => x.ModuleName == moduleName)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogAction>> GetByActionTypeAsync(
            string actionType)
        {
            return await _db.LogActions
                .AsNoTracking()
                .Where(x => x.ActionType == actionType)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogAction>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate)
        {
            return await _db.LogActions
                .AsNoTracking()
                .Where(x =>
                    x.CreateDate >= startDate &&
                    x.CreateDate <= endDate)
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();
        }

        #endregion

        #region Write Log

        public async Task WriteAsync(
            HttpContext httpContext,
            string actionType,
            string? targetSid = null,
            string? targetMid = null,
            string? moduleName = null,
            string? actionName = null,
            string? description = null,
            string? moreDescription = null,
            string result = "成功")
        {
            var user = httpContext.User;

            var userSid = user.FindFirst("member_sid")?.Value;
            var userAccount = user.FindFirst("account")?.Value;
            if (actionType == ActionTypes.Login || actionType == ActionTypes.Register)
            {
                if (string.IsNullOrEmpty(userSid)) userSid = targetSid;
                if (string.IsNullOrEmpty(userAccount)) userAccount = targetMid;
            }

            var log = new LogAction
            {
                CreateDate = DateTime.Now,

                UserSid = userSid,

                UserAccount = userAccount,

                UserName =
                    user.FindFirst(ClaimTypes.Name)?.Value
                    ?? user.FindFirst("name")?.Value,

                ModuleName = moduleName,

                ActionName = actionName,

                ActionType = actionType,

                TargetSid = targetSid,

                TargetMid = targetMid,

                Result = result,

                PageName =
                    httpContext.Request.Path,

                Ip =
                    httpContext.Connection
                        .RemoteIpAddress?
                        .ToString(),

                UserAgent =
                    httpContext.Request.Headers["User-Agent"]
                        .ToString(),

                Description = description,

                MoreDescription = moreDescription
            };

            await CreateAsync(log);
        }

        #endregion

        #region 新增：支援完整搜尋條件的資料庫端分頁方法
        public async Task<PagedResult<LogAction>> GetPagedLogsAsync(
            string userSid, string startDate, string endDate,
            string moduleName, string actionName, string userAccount,
            string keyword, int page, int pageSize)
        {
            var result = new PagedResult<LogAction> { Page = page, PageSize = pageSize };

            // 1. 基礎查詢：限定範圍
            var query = _db.LogActions.Where(l => l.UserSid == userSid);

            // 2. 條件篩選 - 日期區間（確保有值才過濾）
            if (!string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, out DateTime sDate))
            {
                query = query.Where(l => l.CreateDate >= sDate.Date);
            }

            if (!string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, out DateTime eDate))
            {
                // 結束日期調整為當天的 23:59:59
                var endDateTime = eDate.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(l => l.CreateDate <= endDateTime);
            }

            // 3. 條件篩選 - 模組名稱（「全部模組」傳來空字串時跳過）
            if (!string.IsNullOrWhiteSpace(moduleName))
            {
                query = query.Where(l => l.ModuleName == moduleName);
            }

            // 4. 條件篩選 - 操作類型（「全部操作」傳來空字串時跳過）
            if (!string.IsNullOrWhiteSpace(actionName))
            {
                query = query.Where(l => l.ActionName == actionName);
            }

            // 5. 條件篩選 - 使用者帳號（「全部使用者」傳來空字串時跳過）
            if (!string.IsNullOrWhiteSpace(userAccount))
            {
                query = query.Where(l => l.UserAccount == userAccount);
            }

            // 6. 條件篩選 - 關鍵字模糊搜尋
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(l => l.UserAccount != null && l.UserAccount.Contains(keyword) ||
                                         l.Description != null && l.Description.Contains(keyword) ||
                                         l.ActionName != null && l.ActionName.Contains(keyword) ||
                                         l.TargetMid != null && l.TargetMid.Contains(keyword));
            }

            // 7. 計算總筆數
            result.Total = await query.CountAsync();

            // 8. 分頁與排序
            result.Rows = await query.OrderByDescending(l => l.CreateDate)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

            return result;
        }
        #endregion
    }
}