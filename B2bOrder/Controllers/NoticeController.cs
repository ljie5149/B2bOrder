using B2bOrder.Models;
using B2bOrder.Models.Common;
using B2bOrder.Services;
using Microsoft.AspNetCore.Mvc;

namespace B2bOrder.Controllers
{
    public class NoticeController : Controller
    {
        private readonly INoticeService _noticeService;

        public NoticeController(INoticeService noticeService)
        {
            _noticeService = noticeService;
        }

        // --- 頁面檢視 ---

        public async Task<IActionResult> Index(
            string keyword,
            string status,
            string category,
            string sortField,
            string sortDir,
            int page = 1)
        {
            int pageSize = SysDefine.ShowRecordsPerPage;
            sortField = string.IsNullOrEmpty(sortField) ? "publishDate" : sortField;
            sortDir = string.IsNullOrEmpty(sortDir) ? "desc" : sortDir;

            var memberSid = User.FindFirst("member_sid")?.Value;
            var (items, totalCount) = await _noticeService.GetNoticeListAsync(
                    memberSid,
                    keyword, status, category, sortField, sortDir, page, pageSize);

            var viewModel = new NoticeViewModel { DataNoties = items.ToList() };

            // 將當前的搜尋/排序/分頁狀態存入 ViewBag 傳回前端 View
            ViewBag.Keyword = keyword;
            ViewBag.Status = status;
            ViewBag.Category = category;
            ViewBag.Page = page;
            ViewBag.Total = totalCount;
            ViewBag.PageSize = pageSize;
            ViewBag.SortField = sortField;
            ViewBag.SortDir = sortDir;

            ViewBag.ModalModel = new AnnouncementViewModel
            {
                CreateAnnouncement = new CreateAnnouncementModel
                {
                    PublishUnit = "系統管理部",
                    Category = "General",
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(7)
                },
                EditAnnouncement = new EditAnnouncementModel(),
                ViewAnnouncement = new ViewAnnouncementModel()
            };

            return View(viewModel);
        }

        // --- 表單/異步操作動作處理 ---

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAnnouncementModel model)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = "資料驗證失敗" });

            var result = await _noticeService.CreateNoticeAsync(model);

            return Json(new { success = result, message = result ? "公告發布成功" : "發布失敗" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] EditAnnouncementModel model)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = "資料驗證失敗" });

            var result = await _noticeService.UpdateNoticeAsync(model);

            return Json(new { success = result, message = result ? "修改成功" : "修改失敗" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] DeleteActionPayload payload)
        {
            if (payload == null || string.IsNullOrEmpty(payload.Sid))
                return Json(new { success = false, message = "無效參數" });

            bool result = false;

            if (payload.ActionType == "ARCHIVE")
                result = await _noticeService.ArchiveNoticeAsync(payload.Sid);
            else if (payload.ActionType == "PERMANENT")
                result = await _noticeService.PermanentlyDeleteNoticeAsync(payload.Sid, true);

            return Json(new { success = result, message = result ? "操作成功" : "操作失敗" });
        }

        [HttpGet]
        public async Task<IActionResult> GetDetail(string sid, bool incrementClick = false)
        {
            if (string.IsNullOrEmpty(sid))
                return Json(new { success = false, message = "無效的識別碼 (Sid)" });

            var item = await _noticeService.GetNoticeBySidAsync(sid, incrementClick);
            if (item == null)
                return Json(new { success = false, message = "找不到該筆公告資料" });

            return Json(new { success = true, data = item });
        }

        // --- 💡 新增：已讀狀態相關 API ---

        /// <summary>
        /// 標示單篇公告為已讀（供檢視詳細內容時自動觸發）
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MarkAsRead([FromBody] NoticeReadPayload payload)
        {
            if (payload == null || string.IsNullOrEmpty(payload.NoticeSid))
                return Json(new { success = false, message = "無效的參數" });

            var memberSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrEmpty(memberSid))
                return Json(new { success = false, message = "未登入會員" });

            bool result = await _noticeService.MarkNoticeAsReadAsync(payload.NoticeSid, memberSid);
            return Json(new { success = result, message = result ? "已標示為已讀" : "標示已讀失敗" });
        }

        /// <summary>
        /// 批次標示勾選的公告為已讀（供列表上方「標示為已讀」按鈕使用）
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MarkSelectedAsRead([FromBody] BatchNoticeReadPayload payload)
        {
            if (payload?.NoticeSids == null || !payload.NoticeSids.Any())
                return Json(new { success = false, message = "請至少選擇一筆公告" });

            var memberSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrEmpty(memberSid))
                return Json(new { success = false, message = "未登入會員" });

            bool allSuccess = true;
            foreach (var sid in payload.NoticeSids)
            {
                bool res = await _noticeService.MarkNoticeAsReadAsync(sid, memberSid);
                if (!res) allSuccess = false;
            }

            return Json(new { success = allSuccess, message = allSuccess ? "已成功標示為已讀" : "部分標示失敗" });
        }
    }

    // --- 封裝 Payload 結構 ---

    public class DeleteActionPayload
    {
        public string Sid { get; set; } = null!;
        public string ActionType { get; set; } = null!; // ARCHIVE 或 PERMANENT
    }

    public class NoticeReadPayload
    {
        public string NoticeSid { get; set; } = null!;
    }

    public class BatchNoticeReadPayload
    {
        public List<string> NoticeSids { get; set; } = new();
    }
}