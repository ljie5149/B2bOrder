using B2bOrder.Data;
using B2bOrder.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace B2bOrder.Repositories
{
    public interface ITodoRepository
    {
        Task<IEnumerable<DataTodoCalendar>> GetListByUserAsync(string memberSid);
        Task<DataTodoCalendar?> GetBySidAsync(string sid);
        Task<bool> InsertAsync(DataTodoCalendar todo);
        Task<bool> UpdateAsync(DataTodoCalendar todo);
        Task<bool> DeleteAsync(string sid);
    }

    public class TodoRepository : ITodoRepository
    {
        private readonly ApplicationDbContext _context;

        public TodoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取得特定使用者所有未刪除的待辦事項 (未完成優先)
        /// </summary>
        public async Task<IEnumerable<DataTodoCalendar>> GetListByUserAsync(string memberSid)
        {
            // 💡 提示：使用 _context.DataTodoCalendar 比使用 .Set<T>() 更直覺 scannable
            return await _context.DataTodoCalendar
                .Where(t => t.MemberSid == memberSid && t.Avalible == "Y")
                .OrderBy(t => t.IsCompleted)      // 1. 先確保未完成 (false) 優先排在最前面
                .ThenBy(t => t.CreateDate)        // 2. 💡 關鍵：未完成與已完成各自群組內，依建立時間由舊到新 (ASC)
                .ThenByDescending(t => t.Nid)
                .ToListAsync();
        }

        /// <summary>
        /// 依據 Sid 取得單筆有效待辦事項
        /// </summary>
        public async Task<DataTodoCalendar?> GetBySidAsync(string sid)
        {
            return await _context.DataTodoCalendar
                .FirstOrDefaultAsync(t => t.Sid == sid && t.Avalible == "Y");
        }

        /// <summary>
        /// 新增一筆待辦事項
        /// </summary>
        public async Task<bool> InsertAsync(DataTodoCalendar todo)
        {
            if (string.IsNullOrEmpty(todo.Sid))
            {
                todo.Sid = Guid.NewGuid().ToString("N"); // 生成 32 碼無橫線的 UUID
            }

            // 💡 修正 1：保留前端/Service傳入的狀態，僅在未指定時給予預設值
            if (string.IsNullOrEmpty(todo.Avalible)) todo.Avalible = "Y";

            // 💡 修正 2：拿掉手動更新 CreateDate/ModifyDate，交給 DbContext 的 SaveChanges() 處理

            await _context.DataTodoCalendar.AddAsync(todo);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新待辦事項內容（拖曳、時間、狀態變更）
        /// </summary>
        public async Task<bool> UpdateAsync(DataTodoCalendar todo)
        {
            _context.DataTodoCalendar.Attach(todo);
            _context.Entry(todo).State = EntityState.Modified;

            // 確保建立時間與人員不被篡改複寫
            _context.Entry(todo).Property(x => x.CreateDate).IsModified = false;
            _context.Entry(todo).Property(x => x.CreateSid).IsModified = false;
            _context.Entry(todo).Property(x => x.MemberSid).IsModified = false;

            // 💡 修正 3：拿掉手動 todo.ModifyDate = DateTime.Now，DbContext 的 SaveChanges 攔截器會幫你處理

            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 軟刪除待辦事項 (將 Avalible 設為 D)
        /// </summary>
        public async Task<bool> DeleteAsync(string sid)
        {
            var todo = await GetBySidAsync(sid);
            if (todo == null) return false;

            // 💡 修正 4：因為實體已被 GetBySidAsync 追蹤，直接改值，EF Core 會自動抓到異動
            todo.Avalible = "D";

            // 💡 這裡同樣不需要手動指定 ModifyDate，SaveChanges 攔截器在 Modified 狀態下也會自動幫你更新它！

            return await _context.SaveChangesAsync() > 0;
        }
    }
}