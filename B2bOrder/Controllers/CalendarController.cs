using B2bOrder.Services;
using Microsoft.AspNetCore.Mvc;

namespace B2bOrder.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ITodoService _todoService;

        private readonly string _module_name = "個人化行事曆";

        public CalendarController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        /// <summary>
        /// 1. 渲染待辦行事曆主頁面
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 2. 取得該使用者所有待辦事項 (包含已排程與未排程)
        /// Ajax 呼叫 URL: /Calendar/GetTodoList
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTodoList()
        {
            try
            {
                var memberSid = User.FindFirst("member_sid")?.Value;

                if (string.IsNullOrEmpty(memberSid))
                {
                    return RedirectToAction("Logout", "Member");
                }

                var list = await _todoService.GetUserTodoListAsync(memberSid);
                return Json(new { success = true, data = list });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 3. 新增待辦事項 (畫面上方彈出視窗點選「新增項目」)
        /// Ajax 呼叫 URL: /Calendar/Create
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(string title, string className)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return Json(new { success = false, message = "事項名稱不能為空" });
            }

            try
            {
                var memberSid = User.FindFirst("member_sid")?.Value;

                if (string.IsNullOrEmpty(memberSid))
                {
                    return RedirectToAction("Logout", "Member");
                }
                
                var newTodo = await _todoService.CreateTodoAsync(memberSid, title, className);

                if (newTodo != null)
                {
                    return Json(new { success = true, data = newTodo });
                }
                return Json(new { success = false, message = "新增失敗" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTodoDetail(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) 
                return Json(new { success = false, message = "識別碼不正確" });

            var todoDto = await _todoService.GetTodoBySidAsync(id);
            if (todoDto == null) 
                return Json(new { success = false, message = "找不到該筆待辦事項" });

            return Json(new { success = true, data = todoDto });
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id, string title, string className)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(title))
                return Json(new { success = false, message = "資料必填欄位驗證失敗" });

            bool result = await _todoService.UpdateTodoAsync(id, title, className);
            if (!result) 
                return Json(new { success = false, message = "儲存變更失敗" });

            return Json(new { success = true });
        }

        /// <summary>
        /// 4. 當事件在月曆上被拖曳、縮放，或從左側拉入時，同步更新資料庫時間
        /// Ajax 呼叫 URL: /Calendar/UpdateTimeline
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateTimeline(string id, string? start, string? end, bool allDay)
        {
            try
            {
                // 呼叫 Service 解析 ISO 時間並存入 Db
                bool success = await _todoService.UpdateEventTimelineAsync(id, start, end, allDay);
                return Json(new { success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 5. 切換完成狀態 (點擊左側清單的 Checkbox)
        /// Ajax 呼叫 URL: /Calendar/ToggleComplete
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleComplete(string id, bool isDone)
        {
            try
            {
                bool success = await _todoService.ToggleCompleteAsync(id, isDone);
                return Json(new { success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 6. 刪除待辦事項 (軟刪除)
        /// Ajax 呼叫 URL: /Calendar/Delete
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                bool success = await _todoService.DeleteTodoAsync(id);
                return Json(new { success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}