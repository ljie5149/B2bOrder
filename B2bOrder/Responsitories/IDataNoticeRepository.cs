using B2bOrder.Data;
using B2bOrder.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Repositories
{
    public interface INoticeRepository
    {
        Task<(IEnumerable<DataNotice> Items, int TotalCount)> GetPagedAndSortedAsync(
            string? keyword, string? status, string? category, string? sortField, string? sortDir, int pageIndex, int pageSize);

        Task<DataNotice?> GetBySidAsync(string sid);
        Task<DataNotice?> GetByIdAsync(uint nid);
        Task<bool> AddAsync(DataNotice notice);
        Task<bool> UpdateAsync(DataNotice notice);

        // 💡 新增：真實刪除（物理刪除）介面方法
        Task<bool> DeleteAsync(DataNotice notice);
    }

    public class NoticeRepository : INoticeRepository
    {
        private readonly ApplicationDbContext _context;

        public NoticeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<DataNotice> Items, int TotalCount)> GetPagedAndSortedAsync(
            string? keyword, string? status, string? category, string? sortField, string? sortDir, int pageIndex, int pageSize)
        {
            // 1. 初始化查詢，過濾掉已刪除 (Avalible = 'D') 的資料
            var query = _context.DataNotices
                                .Where(x => x.Avalible != "D") // #1 軟刪除規格
                                .AsQueryable();

            // 2. 狀態篩選：啟用/下架（對齊 #2 前端篩選選單）
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.Status == status);
            }

            // 3. 分類篩選（對齊 #1 & #2 的一般公告、活動通知等中文分類）
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category == category);
            }

            // 4. 關鍵字模糊搜尋（擴充：同時搜尋標題、內文、發布單位與分類）
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Title.Contains(keyword) ||
                                         x.Content.Contains(keyword) ||
                                         x.PublishUnit.Contains(keyword) ||
                                         x.Category.Contains(keyword));
            }

            // 總筆數計算
            int totalCount = await query.CountAsync();
            // 💡 修正防呆點 A：如果總筆數本來就是 0，直接回傳空列表，不執行後續的 EF 轉譯與 Skip/Take
            if (totalCount == 0)
            {
                return (new List<DataNotice>(), 0);
            }

            // 5. 動態排序邏輯調整（結合置頂優先權，並對齊前端欄位代碼）
            bool isDesc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

            // 💡 核心優化：不論依據何種欄位排序，皆維持「置頂公告 (IsTop = 1)」永遠在最上方
            var orderedQuery = query.OrderByDescending(x => x.IsTop);

            // 針對指定欄位進行次要排序（轉換小寫避免大小寫不一致問題）
            string field = sortField?.ToLower() ?? "publishdate";
            switch (field)
            {
                case "title": // 對齊 #2 前端 asp-route-sortField="title"
                    orderedQuery = isDesc ? orderedQuery.ThenByDescending(x => x.Title) : orderedQuery.ThenBy(x => x.Title);
                    break;
                case "clickcount": // 對齊 #1 新增的點閱次數欄位
                    orderedQuery = isDesc ? orderedQuery.ThenByDescending(x => x.ClickCount) : orderedQuery.ThenBy(x => x.ClickCount);
                    break;
                case "category": // 對齊 #1 新增的公告分類欄位
                    orderedQuery = isDesc ? orderedQuery.ThenByDescending(x => x.Category) : orderedQuery.ThenBy(x => x.Category);
                    break;
                case "publishunit": // 對齊 #1 新增的發布單位欄位
                    orderedQuery = isDesc ? orderedQuery.ThenByDescending(x => x.PublishUnit) : orderedQuery.ThenBy(x => x.PublishUnit);
                    break;
                case "publishdate": // 對齊 #2 前端 asp-route-sortField="publishDate"
                default:
                    orderedQuery = isDesc ? orderedQuery.ThenByDescending(x => x.PublishDate) : orderedQuery.ThenBy(x => x.PublishDate);
                    break;
            }

            // 💡 修正防呆點 B：確保頁碼與每頁筆數絕對不會小於 1
            int safePageIndex = pageIndex <= 0 ? 1 : pageIndex;
            int safePageSize = pageSize <= 0 ? 10 : pageSize;

            // 計算跳過筆數，確保絕對不為負數
            int skipCount = Math.Max(0, (safePageIndex - 1) * safePageSize);

            // 6. 安全分頁切片
            var items = await orderedQuery
                .Skip(skipCount)
                .Take(safePageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<DataNotice?> GetBySidAsync(string sid)
        {
            if (string.IsNullOrEmpty(sid)) return null; //
            return await _context.DataNotices
                                 .FirstOrDefaultAsync(x => x.Sid == sid && x.Avalible != "D"); //
        }

        public async Task<DataNotice?> GetByIdAsync(uint nid)
        {
            return await _context.DataNotices
                                 .FirstOrDefaultAsync(x => x.Nid == nid && x.Avalible != "D");
        }
        public async Task<bool> AddAsync(DataNotice notice)
        {
            try
            {
                // 明確將實體加入集合
                await _context.DataNotices.AddAsync(notice);

                // 1. 強制檢查狀態（預期應為 Added）
                var entryState = _context.Entry(notice).State;
                System.Diagnostics.Debug.WriteLine($"[EF Debug] 準備寫入資料，實體當前狀態: {entryState}");

                // 2. 執行存檔
                int rowsAffected = await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine($"[EF Debug] SaveChanges 執行完畢，影響筆數: {rowsAffected}");

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // 如果是因為資料表欄位長度（例如 sid 超過 32 碼、publish_unit 超過 50 碼）或型態不符，這裡一定會噴出
                System.Diagnostics.Debug.WriteLine($"[EF 錯誤] 資料庫寫入崩潰: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[EF 內部 SQL 錯誤] : {ex.InnerException.Message}");
                }
                return false;
            }
        }
        //public async Task<bool> AddAsync(DataNotice notice)
        //{
        //    await _context.DataNotices.AddAsync(notice);
        //    return await _context.SaveChangesAsync() > 0;
        //}

        public async Task<bool> UpdateAsync(DataNotice notice)
        {
            _context.DataNotices.Update(notice);
            return await _context.SaveChangesAsync() > 0;
        }

        // 💡 💡 💡 新增：真實刪除實作（物理刪除） 💡 💡 💡
        public async Task<bool> DeleteAsync(DataNotice notice)
        {
            if (notice == null) return false;

            try
            {
                // 從 EF Tracker 中移除該實體（標記狀態為 Deleted）
                _context.DataNotices.Remove(notice);

                System.Diagnostics.Debug.WriteLine($"[EF Debug] 準備從資料庫物理刪除，SID: {notice.Sid}");

                // 執行 SaveChangesAsync，底層會發送 DELETE FROM data_notices WHERE... 語法
                int rowsAffected = await _context.SaveChangesAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EF 錯誤] 真實刪除物理資料崩潰: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[EF 內部 SQL 錯誤] : {ex.InnerException.Message}");
                }
                return false;
            }
        }
    }
}