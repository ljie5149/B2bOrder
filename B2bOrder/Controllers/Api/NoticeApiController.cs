using B2bOrder.Models.api;
using B2bOrder.Models.Common;
using B2bOrder.Services;
using B2bOrder.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace B2bOrder.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoticeApiController : BaseApiController
    {
        private readonly INoticeService _noticeService;

        public NoticeApiController(INoticeService noticeService, IJwtService jwtService, IEncryptionService encryptionService) : base(jwtService, encryptionService)
        {
            _noticeService = noticeService;
        }

        /// <summary>
        /// 獲取公告分頁列表（對齊 #2 前端條件篩選與分頁）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? keyword,
            [FromQuery] string? status = "啟用",
            [FromQuery] string? category = null,
            [FromQuery] string? sortField = "publishDate",
            [FromQuery] string? sortDir = "desc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = SysDefine.ShowRecordsPerPage)
        {
            try
            {
                // 1. 調用基底類別方法，取得驗證後的 memberSid
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                var (items, totalCount) = await _noticeService.GetNoticeListAsync(
                    memberSid,
                    keyword, status, category, sortField, sortDir, page, pageSize);

                var pagedResult = new
                {
                    totalCount = totalCount,
                    list = items
                };

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(pagedResult), "取得公告分頁清單成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(pagedResult, "取得公告分頁清單成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 依據 SID 獲取單筆公告明細（對齊 #2 檢視明細：觸發時自動增加點閱數與標記已讀）
        /// </summary>
        [HttpGet("{sid}")]
        public async Task<IActionResult> GetBySid(string sid)
        {
            try
            {
                // 1. 調用基底類別方法，取得驗證後的 memberSid
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (string.IsNullOrEmpty(sid))
                {
                    return BadRequest(ApiResponse<object>.Fail("SID 不能為空。", "0x0400"));
                }

                // 2. 傳入 incrementClick: true，點開明細時 ClickCount 自動 +1
                var item = await _noticeService.GetNoticeBySidAsync(sid, incrementClick: true);
                if (item == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"找不到該筆公告資料: {sid}。", "0x0404"));
                }

                // 💡 點開明細時自動標示單篇已讀
                await _noticeService.MarkNoticeAsReadAsync(sid, memberSid);
                item.IsRead = true;

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(item), "取得公告明細成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(item, "取得公告明細成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 💡 新增：標記單篇公告為已讀
        /// </summary>
        [HttpPost("read/{sid}")]
        public async Task<IActionResult> MarkAsRead(string sid)
        {
            try
            {
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (string.IsNullOrEmpty(sid))
                {
                    return BadRequest(ApiResponse<object>.Fail("SID 不能為空。", "0x0400"));
                }

                bool result = await _noticeService.MarkNoticeAsReadAsync(sid, memberSid);
                if (!result)
                {
                    return Ok(ApiResponse<object>.Fail("標記已讀失敗", "0x0201"));
                }

                return Ok(ApiResponse<object>.Success(null, "已成功標記為已讀", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 💡 新增：標記所有公告為已讀
        /// </summary>
        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                bool result = await _noticeService.MarkAllNoticesAsReadAsync(memberSid);
                if (!result)
                {
                    return Ok(ApiResponse<object>.Fail("全部標記已讀失敗", "0x0201"));
                }

                return Ok(ApiResponse<object>.Success(null, "所有公告已成功標記為已讀", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 新增系統公告（對齊 #1 新增欄位規格 與 #2 新增 Modal）
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RequestApiModel4CreateAnnouncement model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("欄位驗證失敗。", "0x0400"));
            }

            try
            {
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (model == null)
                {
                    return BadRequest(ApiResponse<object>.Fail("公告資料不能為空。", "0x0400"));
                }

                model.CreatorSid = memberSid;

                var result = await _noticeService.CreateNoticeAsync(model);
                if (!result)
                {
                    return Ok(ApiResponse<object>.Fail("系統公告新增失敗", "0x0201"));
                }

                return Ok(ApiResponse<object>.Success(null, "公告新增成功", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 修改系統公告（對齊 #1 新增欄位規格 與 #2 編輯 Modal）
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RequestApiModel4EditAnnouncement model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("欄位驗證失敗。", "0x0400"));
            }

            try
            {
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (model == null)
                {
                    return BadRequest(ApiResponse<object>.Fail("更新資料不能為空。", "0x0400"));
                }

                var result = await _noticeService.UpdateNoticeAsync(model);
                if (!result)
                {
                    return Ok(ApiResponse<object>.Fail("系統公告更新失敗，可能資料已被移除", "0x0201"));
                }

                return Ok(ApiResponse<object>.Success(null, "公告更新成功", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 刪除或下架公告（對齊 #2 刪除確認 Modal 的兩種行為機制）
        /// </summary>
        [HttpDelete("{sid}")]
        public async Task<IActionResult> Delete(string sid, [FromQuery] string deleteType = "ARCHIVE")
        {
            try
            {
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (string.IsNullOrEmpty(sid))
                {
                    return BadRequest(ApiResponse<object>.Fail("遺失關鍵識別碼 Sid", "0x0400"));
                }

                bool result;

                if (string.Equals(deleteType, "PERMANENT", StringComparison.OrdinalIgnoreCase))
                {
                    result = await _noticeService.PermanentlyDeleteNoticeAsync(sid);
                }
                else
                {
                    result = await _noticeService.ArchiveNoticeAsync(sid);
                }

                if (!result)
                {
                    return Ok(ApiResponse<object>.Fail("操作執行失敗，請檢查該公告是否存在", "0x0201"));
                }

                string successMessage = string.Equals(deleteType, "PERMANENT", StringComparison.OrdinalIgnoreCase)
                    ? "該公告已從系統永久刪除。"
                    : "該公告已成功提前下架並封存。";

                return Ok(ApiResponse<object>.Success(null, successMessage, "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }
    }
}