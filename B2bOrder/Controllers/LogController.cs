using B2bOrder.Models.Common;
using B2bOrder.Models.Db;
using B2bOrder.Services.Common;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace B2bOrder.Controllers
{
    [Authorize]
    public class LogController : Controller
    {
        private readonly ILogActionService _logService;

        public LogController(ILogActionService logService)
        {
            _logService = logService;
        }

        // 1. 負責進入頁面，動態撈取現存有操作紀錄的不重複 UserAccount
        public async Task<IActionResult> Index()
        {
            var userSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrWhiteSpace(userSid))
            {
                return View();
            }

            var logs = await _logService.GetByUserSidAsync(userSid);

            // 修正：完全使用 UserAccount 來抓取操作者清單
            var accounts = logs.Where(l => !string.IsNullOrEmpty(l.UserAccount))
                               .Select(l => l.UserAccount!)
                               .Distinct()
                               .OrderBy(a => a)
                               .ToList();

            ViewBag.UserAccounts = accounts;

            return View();
        }

        // 2. 負責處理前端 AJAX 的非同步篩選與分頁 API
        [HttpGet]
        public async Task<IActionResult> GetLogs(
            string startDate, string endDate,
            string pageName, string actionType, string userAccount,
            string keyword, int page = 1, int pageSize = SysDefine.ShowRecordsPerPage)
        {
            var userSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrWhiteSpace(userSid))
            {
                return Json(new { success = false, message = "未登入或權限不足" });
            }

            // 直接呼叫經優化過、在資料庫端過濾分頁的 Service
            var result = await _logService.GetPagedLogsAsync(
                userSid, startDate, endDate, pageName, actionType, userAccount, keyword, page, pageSize);

            return Json(result, new JsonSerializerOptions { PropertyNamingPolicy = null });
        }

        #region 匯出操作紀錄（CSV 或 Excel）
        [HttpGet]
        public async Task<IActionResult> Export(
            string type,
            string startDate,
            string endDate,
            string pageName,
            string actionType,
            string userAccount,
            string keyword = "")
        {
            var userSid = User.FindFirst("member_sid")?.Value;
            if (string.IsNullOrWhiteSpace(userSid))
            {
                return Json(new { success = false, message = "未登入或權限不足" });
            }

            // 匯出通常為符合該篩選條件下的「全部資料」，因此 page=1, pageSize 傳入極大值 999999
            var result = await _logService.GetPagedLogsAsync(
                userSid, startDate, endDate, pageName, actionType, userAccount, keyword, page: 1, pageSize: 999999);

            // 若查無資料，直接返回阻擋
            if (result == null || result.Rows == null || result.Rows.Count == 0)
            {
                return Content("<script>alert('查無符合當前篩選條件的操作紀錄，無法匯出。');window.history.back();</script>", "text/html", Encoding.UTF8);
            }

            // 比照會員匯出邏輯，依據前端 type 參數決定導向
            if (type == "csv")
            {
                return ExportCsv(result.Rows);
            }

            return ExportExcel(result.Rows);
        }

        private IActionResult ExportCsv(List<LogAction> data)
        {
            var sb = new StringBuilder();

            // 1. 寫入欄位表頭
            sb.AppendLine("流水號,時間,使用者,模組,操作,目標對象,IP 位址,結果,操作描述");

            // 2. 寫入內容資料
            int index = 1;
            foreach (var log in data)
            {
                // 阻擋可能破壞 CSV 結構的逗號與換行符號
                string cleanDesc = (log.Description ?? "").Replace(",", "，").Replace("\r", "").Replace("\n", " ");
                string cleanAction = (log.ActionName ?? "").Replace(",", "，");
                string cleanModule = (log.ModuleName ?? "一般").Replace(",", "，");
                string targetDisplay = (log.TargetMid ?? "-").Replace(",", "，");

                sb.AppendLine(
                    $"{index}," +
                    $"{log.CreateDate:yyyy-MM-dd HH:mm:ss}," +
                    $"{log.UserAccount ?? "Unknown"}," +
                    $"{cleanModule}," +
                    $"{cleanAction}," +
                    $"{targetDisplay}," +
                    $"{log.Ip ?? "127.0.0.1"}," +
                    $"{log.Result ?? "成功"}," +
                    $"{cleanDesc}");

                index++;
            }

            // 加上 UTF8 Preamble (BOM) 防止 Excel 開啟 CSV 時中文變成亂碼
            return File(
                Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray(),
                "text/csv",
                $"操作紀錄_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }

        private IActionResult ExportExcel(List<LogAction> data)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("操作紀錄");

            // 1. 設定表頭名稱
            ws.Cell(1, 1).Value = "流水號";
            ws.Cell(1, 2).Value = "時間";
            ws.Cell(1, 3).Value = "使用者";
            ws.Cell(1, 4).Value = "模組";
            ws.Cell(1, 5).Value = "操作";
            ws.Cell(1, 6).Value = "目標對象";
            ws.Cell(1, 7).Value = "IP 位址";
            ws.Cell(1, 8).Value = "結果";
            ws.Cell(1, 9).Value = "操作描述";

            // 2. 填入內容資料
            int row = 2;
            foreach (var log in data)
            {
                ws.Cell(row, 1).Value = row - 1; // 流水號自增
                ws.Cell(row, 2).Value = log.CreateDate.ToString("yyyy-MM-dd HH:mm:ss") ?? "-";
                ws.Cell(row, 3).Value = log.UserAccount ?? "Unknown";
                ws.Cell(row, 4).Value = log.ModuleName ?? "一般";
                ws.Cell(row, 5).Value = log.ActionName ?? "-";
                ws.Cell(row, 6).Value = log.TargetMid ?? "-";
                ws.Cell(row, 7).Value = log.Ip ?? "127.0.0.1";
                ws.Cell(row, 8).Value = log.Result ?? "成功";
                ws.Cell(row, 9).Value = log.Description ?? "-";

                row++;
            }

            // 自動調整欄寬
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"操作紀錄_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
        #endregion
    }
}