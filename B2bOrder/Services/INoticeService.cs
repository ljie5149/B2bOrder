using B2bOrder.Extensions;
using B2bOrder.Models;
using B2bOrder.Models.Db;
using B2bOrder.Repositories;

namespace B2bOrder.Services
{
    public interface INoticeService
    {
        // 💡 查詢列表時帶入 memberSid 以判斷每筆公告的 IsRead (已讀/未讀)
        Task<(IEnumerable<DataNotice> Items, int TotalCount)> GetNoticeListAsync(
            string? memberSid, string? keyword, string? status, string? category, string? sortField, string? sortDir, int page, int pageSize);

        Task<DataNotice?> GetNoticeBySidAsync(string sid, bool incrementClick);

        // 💡 單篇公告標示為已讀
        Task<bool> MarkNoticeAsReadAsync(string noticeSid, string memberSid);

        // 💡 將該會員的所有公告標示為全部已讀
        Task<bool> MarkAllNoticesAsReadAsync(string memberSid);

        // 管理端 API
        Task<bool> CreateNoticeAsync(CreateAnnouncementModel model);
        Task<bool> UpdateNoticeAsync(EditAnnouncementModel model);
        Task<bool> ArchiveNoticeAsync(string sid);
        Task<bool> PermanentlyDeleteNoticeAsync(string sid, bool realDelete = false);
    }

    public class NoticeService : BaseService, INoticeService
    {
        private readonly ISysUserService _sysUserService;
        private readonly INoticeRepository _noticeRepository;
        private readonly IDataNoticeReadRepository _noticeReadRepository; // 💡 補上 Repository 注入
        private readonly string _moduleName = "系統公告管理";

        public NoticeService(
            IServiceProvider serviceProvider,
            INoticeRepository repo,
            IDataNoticeReadRepository noticeReadRepository,
            ISysUserService sysUserService) : base(serviceProvider)
        {
            _noticeRepository = repo;
            _noticeReadRepository = noticeReadRepository;
            _sysUserService = sysUserService;
        }

        /// <summary>
        /// 輔助方法：統一取得操作者辨識 Sid（如果取得不到則預設系統管理）
        /// </summary>
        private string GetCurrentUserSid()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("member_sid")?.Value ?? "SYS_ADMIN";
        }

        private string MapCategoryFromValue(string? categoryValue)
        {
            return categoryValue switch
            {
                "General" => "一般公告",
                "Activity" => "活動通知",
                "System" => "系統維護",
                "Urgent" => "緊急通報",
                _ => string.IsNullOrEmpty(categoryValue) ? "一般公告" : categoryValue
            };
        }

        /// <summary>
        /// 取得公告列表，並透過 last_read_all_notices_time 與 data_notice_read 比對出每筆公告的已讀狀態
        /// </summary>
        public async Task<(IEnumerable<DataNotice> Items, int TotalCount)> GetNoticeListAsync(
            string? memberSid, string? keyword, string? status, string? category, string? sortField, string? sortDir, int page, int pageSize)
        {
            int safePage = page <= 0 ? 1 : page;
            int safePageSize = pageSize <= 0 ? 10 : pageSize;

            string? dbCategory = !string.IsNullOrEmpty(category) ? MapCategoryFromValue(category) : null;

            // 1. 取得分頁公告資料
            var (notices, totalCount) = await _noticeRepository.GetPagedAndSortedAsync(keyword, status, dbCategory, sortField, sortDir, safePage, safePageSize);

            // 2. 若有傳入會員 Sid，撈取該會員的已讀判斷基準
            DateTime lastReadAllTime = DateTime.MinValue;
            HashSet<string> readNoticeSids = new HashSet<string>();

            if (!string.IsNullOrEmpty(memberSid))
            {
                var user = await _sysUserService.GetByMemberSidAsync(memberSid);
                if (user?.LastReadAllNoticesTime != null)
                {
                    lastReadAllTime = user.LastReadAllNoticesTime.Value;
                }

                // 💡 修正：使用 _noticeReadRepository 撈取已讀紀錄
                var sids = await _noticeReadRepository.GetReadNoticeSidsByMemberAsync(memberSid);
                readNoticeSids = sids.ToHashSet();
            }

            // 3. 組合已讀/未讀狀態判斷
            foreach (var notice in notices)
            {
                // 已讀條件：
                // A. 公告發布時間 <= 會員「最後全部標為已讀」的時間
                // B. 或者，該公告 sid 存在於 data_notice_read 關聯表內
                notice.IsRead = (notice.PublishDate <= lastReadAllTime) || readNoticeSids.Contains(notice.Sid);
            }

            return (notices, totalCount);
        }

        /// <summary>
        /// 會員點開單篇公告時呼叫：寫入已讀紀錄表
        /// </summary>
        public async Task<bool> MarkNoticeAsReadAsync(string noticeSid, string memberSid)
        {
            if (string.IsNullOrEmpty(noticeSid) || string.IsNullOrEmpty(memberSid)) return false;

            // 1. 檢查是否已經讀過（避免 Unique Constraint 衝突）
            bool alreadyRead = await _noticeReadRepository.ExistsAsync(noticeSid, memberSid);
            if (alreadyRead) return true;

            // 2. 寫入 data_notice_read 表
            var readRecord = new DataNoticeRead
            {
                NoticeSid = noticeSid,
                MemberSid = memberSid,
                ReadTime = DateTime.Now
            };

            return await _noticeReadRepository.AddAsync(readRecord);
        }

        /// <summary>
        /// 會員點擊「全部標為已讀」：更新 sys_user 時間戳記並清空該會員的舊單篇紀錄（釋放資料庫空間）
        /// </summary>
        public async Task<bool> MarkAllNoticesAsReadAsync(string memberSid)
        {
            if (string.IsNullOrEmpty(memberSid)) return false;

            // 1. 更新 sys_user.last_read_all_notices_time = NOW()
            bool updatedUser = await _sysUserService.UpdateLastReadAllNoticesTimeAsync(memberSid, DateTime.Now);

            if (updatedUser)
            {
                // 2. 一併清空該會員在 data_notice_read 裡的舊紀錄，避免資料表膨脹
                await _noticeReadRepository.DeleteByMemberSidAsync(memberSid);
            }

            return updatedUser;
        }

        public async Task<DataNotice?> GetNoticeBySidAsync(string sid, bool incrementClick)
        {
            if (string.IsNullOrEmpty(sid)) return null;

            var notice = await _noticeRepository.GetBySidAsync(sid);
            if (notice == null) return null;

            if (incrementClick)
            {
                notice.ClickCount++;
                await _noticeRepository.UpdateAsync(notice);
            }

            return notice;
        }

        public async Task<bool> CreateNoticeAsync(CreateAnnouncementModel model)
        {
            if (model == null) return false;

            DateTime? startDate = DateTimeExtensions.ToStartOfDay(model.StartDate);
            DateTime? endDate = DateTimeExtensions.ToEndOfDay(model.EndDate);
            string userSid = GetCurrentUserSid();

            var notice = new DataNotice
            {
                Sid = Guid.NewGuid().ToString("N"),
                Title = model.Title,
                Content = model.Content,
                IsTop = (sbyte)(model.IsTop ? 1 : 0),
                PublishUnit = string.IsNullOrEmpty(model.PublishUnit) ? "系統管理部" : model.PublishUnit,
                Category = MapCategoryFromValue(model.Category),
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                ClickCount = 0,
                PublishDate = DateTime.Today,
                StartTime = startDate,
                EndTime = endDate,
                CreateUserSid = userSid,
                CreateDate = DateTime.Now,
                Avalible = "Y",
                Status = "啟用"
            };

            bool success = await _noticeRepository.AddAsync(notice);

            if (success)
            {
                await WriteLog(
                    actionType: "Create",
                    targetSid: notice.Sid,
                    moduleName: _moduleName,
                    actionName: "發布系統公告",
                    description: $"新增了一筆公告：{model.Title}",
                    result: "成功"
                );
            }

            return success;
        }

        public async Task<bool> UpdateNoticeAsync(EditAnnouncementModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Sid)) return false;

            var existing = await _noticeRepository.GetBySidAsync(model.Sid);
            if (existing == null) return false;

            existing.Title = model.Title;
            existing.Content = model.Content;
            existing.IsTop = (sbyte)(model.IsTop ? 1 : 0);
            existing.PublishUnit = string.IsNullOrEmpty(model.PublishUnit) ? "系統管理部" : model.PublishUnit;
            existing.Category = MapCategoryFromValue(model.Category);
            existing.StartDate = model.StartDate;
            existing.EndDate = model.EndDate;
            existing.StartTime = DateTimeExtensions.ToStartOfDay(model.StartDate);
            existing.EndTime = DateTimeExtensions.ToEndOfDay(model.EndDate);

            existing.ModifyUserSid = GetCurrentUserSid();
            existing.ModifyDate = DateTime.Now;

            bool success = await _noticeRepository.UpdateAsync(existing);

            if (success)
            {
                await WriteLog(
                    actionType: "Update",
                    targetSid: existing.Sid,
                    moduleName: _moduleName,
                    actionName: "修改系統公告",
                    description: $"更新公告內容：{model.Title}",
                    result: "成功"
                );
            }

            return success;
        }

        public async Task<bool> ArchiveNoticeAsync(string sid)
        {
            var notice = await _noticeRepository.GetBySidAsync(sid);
            if (notice == null) return false;

            notice.Status = "下架";
            notice.ModifyUserSid = GetCurrentUserSid();
            notice.ModifyDate = DateTime.Now;

            bool success = await _noticeRepository.UpdateAsync(notice);

            if (success)
            {
                await WriteLog(
                    actionType: "Update",
                    targetSid: notice.Sid,
                    moduleName: _moduleName,
                    actionName: "下架系統公告",
                    description: $"將公告設為下架：{notice.Title}",
                    result: "成功"
                );
            }

            return success;
        }

        public async Task<bool> PermanentlyDeleteNoticeAsync(string sid, bool realDelete = false)
        {
            var notice = await _noticeRepository.GetBySidAsync(sid);
            if (notice == null) return false;

            bool success = false;
            string userSid = GetCurrentUserSid();

            if (realDelete)
            {
                success = await _noticeRepository.DeleteAsync(notice);
            }
            else
            {
                notice.Avalible = "D";
                notice.ModifyUserSid = userSid;
                notice.ModifyDate = DateTime.Now;
                success = await _noticeRepository.UpdateAsync(notice);
            }

            if (success)
            {
                await WriteLog(
                    actionType: "Delete",
                    targetSid: notice.Sid,
                    moduleName: _moduleName,
                    actionName: realDelete ? "實體刪除系統公告" : "軟刪除系統公告",
                    description: $"刪除了公告：{notice.Title}",
                    result: "成功"
                );
            }

            return success;
        }
    }
}