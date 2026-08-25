using B2bOrder.Models;
using B2bOrder.Models.Db;
using B2bOrder.Repositories;

namespace B2bOrder.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoDto>> GetUserTodoListAsync(string userSid);
        Task<TodoDto?> CreateTodoAsync(string userSid, string title, string className);
        Task<TodoDto?> GetTodoBySidAsync(string sid);
        Task<bool> UpdateTodoAsync(string sid, string title, string className);
        Task<bool> UpdateEventTimelineAsync(string sid, string? startIso, string? endIso, bool allDay);
        Task<bool> ToggleCompleteAsync(string sid, bool isDone);
        Task<bool> DeleteTodoAsync(string sid);
    }

    public class TodoService : BaseService, ITodoService
    {
        private readonly ITodoRepository _todoRepository;
        private readonly string _moduleName = "個人化行事曆"; // 💡 從原本的 Controller 移入

        public TodoService(IServiceProvider serviceProvider, ITodoRepository repo) : base(serviceProvider)
        {
            _todoRepository = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// 取得使用者所有代辦清單並轉換為 DTO 格式
        /// </summary>
        public async Task<IEnumerable<TodoDto>> GetUserTodoListAsync(string userSid)
        {
            var todos = await _todoRepository.GetListByUserAsync(userSid);

            return todos.Select(t => new TodoDto
            {
                Id = t.Sid,
                Title = t.Title,
                Category = t.Category,
                ClassName = t.ClassName,
                Start = t.StartDate,
                End = t.EndDate,
                AllDay = t.IsAllDay,
                IsDone = t.IsCompleted,
                IsScheduled = t.IsScheduled // 💡 同步更新：對齊新版 DTO 欄位
            });
        }

        /// <summary>
        /// 新增一筆待辦事項
        /// </summary>
        public async Task<TodoDto?> CreateTodoAsync(string userSid, string title, string className)
        {
            string category = "work";
            if (className.Contains("life")) category = "life";
            if (className.Contains("personal")) category = "personal";

            var entity = new DataTodoCalendar
            {
                Sid = Guid.NewGuid().ToString("N"),
                MemberSid = userSid,  // 💡 修正 1：必須補上，否則資料庫必填限制會報錯
                CreateSid = userSid,
                Title = title,
                Category = category,
                ClassName = className,
                StartDate = null,
                EndDate = null,
                IsAllDay = false,
                IsCompleted = false,
                IsScheduled = false,  // 💡 修正 2-A：明確宣告為未排程狀態
                Avalible = "Y"
            };

            bool success = await _todoRepository.InsertAsync(entity);
            if (!success) return null;

            // 💡 新增：寫入操作日誌
            await WriteLog(
                actionType: "Create",
                targetSid: entity.Sid,
                moduleName: _moduleName,
                actionName: "新增待辦事項",
                description: $"新增待辦事項：{title}",
                result: "成功"
            );

            return new TodoDto
            {
                Id = entity.Sid,
                Title = entity.Title,
                Category = entity.Category,
                ClassName = entity.ClassName,
                IsDone = entity.IsCompleted,
                IsScheduled = entity.IsScheduled
            };
        }

        /// <summary>
        /// 💡 關鍵新增：依據識別碼撈取單筆資料並回傳對齊前端規格的 DTO
        /// </summary>
        public async Task<TodoDto?> GetTodoBySidAsync(string sid)
        {
            var t = await _todoRepository.GetBySidAsync(sid);
            if (t == null) return null;

            return new TodoDto
            {
                Id = t.Sid,
                Title = t.Title,
                Category = t.Category,
                ClassName = t.ClassName,
                Start = t.StartDate,
                End = t.EndDate,
                AllDay = t.IsAllDay,
                IsDone = t.IsCompleted,
                IsScheduled = t.IsScheduled
            };
        }

        /// <summary>
        /// 💡 關鍵新增：更新待辦事項主體名稱與標籤樣式
        /// </summary>
        public async Task<bool> UpdateTodoAsync(string sid, string title, string className)
        {
            var todo = await _todoRepository.GetBySidAsync(sid);
            if (todo == null) return false;

            string category = "work";
            if (className.Contains("life")) category = "life";
            if (className.Contains("personal")) category = "personal";

            string oldTitle = todo.Title;

            todo.Title = title;
            todo.ClassName = className;
            todo.Category = category;

            bool success = await _todoRepository.UpdateAsync(todo);

            if (success)
            {
                // 💡 新增：寫入操作日誌
                await WriteLog(
                    actionType: "Update",
                    targetSid: sid,
                    moduleName: _moduleName,
                    actionName: "更新待辦事項",
                    description: $"更新待辦資料 (原標題: {oldTitle} -> 新標題: {title})",
                    result: "成功"
                );
            }

            return success;
        }

        /// <summary>
        /// 當事件拖曳變更時間或從左側拉入行事曆時觸發
        /// </summary>
        public async Task<bool> UpdateEventTimelineAsync(string sid, string? startIso, string? endIso, bool allDay)
        {
            var todo = await _todoRepository.GetBySidAsync(sid);
            if (todo == null) return false;

            todo.StartDate = string.IsNullOrEmpty(startIso) ? null : (DateTime?)DateTime.Parse(startIso);
            todo.EndDate = string.IsNullOrEmpty(endIso) ? null : (DateTime?)DateTime.Parse(endIso);
            todo.IsAllDay = allDay;
            todo.IsScheduled = todo.StartDate.HasValue;

            bool success = await _todoRepository.UpdateAsync(todo);

            if (success)
            {
                // 💡 新增：寫入操作日誌
                await WriteLog(
                    actionType: "Update",
                    targetSid: sid,
                    moduleName: _moduleName,
                    actionName: "排程異動",
                    description: $"更動了行事曆排程時間: {todo.Title}",
                    result: "成功"
                );
            }

            return success;
        }

        /// <summary>
        /// 勾選完成狀態
        /// </summary>
        public async Task<bool> ToggleCompleteAsync(string sid, bool isDone)
        {
            var todo = await _todoRepository.GetBySidAsync(sid);
            if (todo == null) return false;

            todo.IsCompleted = isDone;
            bool success = await _todoRepository.UpdateAsync(todo);

            if (success)
            {
                // 💡 新增：寫入操作日誌
                await WriteLog(
                    actionType: "Update",
                    targetSid: sid,
                    moduleName: _moduleName,
                    actionName: "切換完成狀態",
                    description: $"切換狀態為 {(isDone ? "已完成" : "未完成")}: {todo.Title}",
                    result: "成功"
                );
            }

            return success;
        }

        /// <summary>
        /// 刪除項目
        /// </summary>
        public async Task<bool> DeleteTodoAsync(string sid)
        {
            // 將軟刪除的實作透過 Repository 處理。這裡僅紀錄
            bool success = await _todoRepository.DeleteAsync(sid);

            if (success)
            {
                // 💡 新增：寫入操作日誌
                await WriteLog(
                    actionType: "Delete",
                    targetSid: sid,
                    moduleName: _moduleName,
                    actionName: "刪除待辦事項",
                    description: $"刪除了指定的待辦事項 (SID: {sid})",
                    result: "成功"
                );
            }

            return success;
        }
    }
}