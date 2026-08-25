using B2bOrder.Models.api;
using B2bOrder.Models.Common;
using B2bOrder.Services;
using B2bOrder.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace B2bOrder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalendarApiController : BaseApiController
    {
        private readonly ITodoService _todoService;

        public CalendarApiController(ITodoService todoService, IJwtService jwtService, IEncryptionService encryptionService) : base(jwtService, encryptionService)
        {
            _todoService = todoService;
        }

        /// <summary>
        /// 2. 取得該使用者所有待辦事項 (包含已排程與未排程)
        /// AJAX 呼叫 URL: GET /api/CalendarApi
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTodoList()
        {
            try
            {
                // 1. 調用基底類別方法，取得驗證後的 memberSid
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                // 2. 執行商業邏輯
                var list = await _todoService.GetUserTodoListAsync(memberSid);

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(list), "取得待辦事項成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(list, "取得待辦事項成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 3. 新增待辦事項 (由側欄或快速控制區觸發)
        /// AJAX 呼叫 URL: POST /api/CalendarApi
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RequestApiModel4CalendarPost input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.title))
            {
                return BadRequest(ApiResponse<object>.Fail("事項名稱不能為空", "0x0400"));
            }

            try
            {
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                var newTodo = await _todoService.CreateTodoAsync(memberSid, input.title, input.className);

                if (newTodo != null)
                {
                    if (SysDefine.EncryptData)
                    {
                        return Ok(ApiResponse<object>.Success(EncryptData(newTodo), "新增成功", "0x0200")); // 加密資料
                    }
                    else
                    {
                        return Ok(ApiResponse<object>.Success(newTodo, "新增成功", "0x0200"));
                    }
                }
                return Ok(ApiResponse<object>.Fail("新增失敗", "0x0201"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 4. 更新待辦事項基本資料
        /// AJAX 呼叫 URL: PUT /api/CalendarApi
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RequestApiModel4CalendarPut input)
        {
            try
            {
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                // 增加權限檢查，確保使用者只能更新自己的待辦事項
                bool success = await _todoService.UpdateTodoAsync(input.sid, input.title, input.className);
                if (success)
                {
                    return Ok(ApiResponse<object>.Success("更新成功", "0x0200"));
                }
                return Ok(ApiResponse<object>.Fail("更新失敗，找不到該項目或權限不足", "0x0201"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 4. 當事件在月曆上被拖曳、縮放，或從左側拉入時，同步更新資料庫時間
        /// AJAX 呼叫 URL: PUT /api/CalendarApi/timeline
        /// </summary>
        [HttpPut("timeline")]
        public async Task<IActionResult> UpdateTimeline([FromBody] RequestApiModel4CalendarTimeline input)
        {
            try
            {
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                bool success = await _todoService.UpdateEventTimelineAsync(input.sid, input.start, input.end, input.allDay);
                if (success)
                {
                    return Ok(ApiResponse<object>.Success("時間軸更新成功", "0x0200"));
                }
                return Ok(ApiResponse<object>.Fail("時間軸更新失敗，找不到該項目或權限不足", "0x0201"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 5. 切換完成狀態 (點擊左側清單或行事曆事件的 Checkbox)
        /// AJAX 呼叫 URL: PUT /api/CalendarApi/toggle-complete
        /// </summary>
        [HttpPut("toggle-complete")]
        public async Task<IActionResult> ToggleComplete([FromBody] RequestApiModel4CalendarComplete input)
        {
            try
            {
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                bool success = await _todoService.ToggleCompleteAsync(input.sid, input.isDone);
                if (success)
                {
                    return Ok(ApiResponse<object>.Success("完成狀態切換成功", "0x0200"));
                }
                return Ok(ApiResponse<object>.Fail("完成狀態切換失敗，找不到該項目或權限不足", "0x0201"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 6. 刪除待辦事項 (軟刪除)
        /// AJAX 呼叫 URL: DELETE /api/CalendarApi
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] RequestApiModel4CalendarDelete input)
        {
            try
            {
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                bool success = await _todoService.DeleteTodoAsync(input.sid);
                if (success)
                {
                    return Ok(ApiResponse<object>.Success("刪除成功", "0x0200"));
                }
                return Ok(ApiResponse<object>.Fail("刪除失敗，找不到該項目或權限不足", "0x0201"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }
    }
}