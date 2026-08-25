using B2bOrder.Models.api;
using B2bOrder.Models.Common;
using B2bOrder.Models.Db;
using B2bOrder.Services;
using B2bOrder.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace B2bOrder.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogImportApiController : BaseApiController
    {
        private readonly ILogImportService _service;
        public LogImportApiController(ILogImportService service, IJwtService jwtService, IEncryptionService encryptionService) : base(jwtService, encryptionService)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                var list = await _service.GetAllAsync();
                var en = EncryptData(list); // 加密資料
                return Ok(ApiResponse<object>.Success(en, "取得所有匯入日誌成功。", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        [HttpGet("{nid:int}")]
        public async Task<IActionResult> Get(int nid)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (nid <= 0)
                {
                    return BadRequest(ApiResponse<object>.Fail($"不合法的 ID: {nid}。", "0x0400"));
                }

                var item = await _service.GetByIdAsync(nid);
                if (item == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"找不到 ID 為 {nid} 的匯入日誌。", "0x0404"));
                }

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(item), "取得匯入日誌成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(item, "取得匯入日誌成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LogImport model)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (model == null)
                {
                    return BadRequest(ApiResponse<object>.Fail("日誌資料不能為空。", "0x0400"));
                }

                // 安全性增強：應使用 Token 中的 memberSid 覆寫或設定 model 的操作者欄位
                // model.MemberSid = memberSid;

                await _service.CreateAsync(model);

                var result = ApiResponse<object>.Success(model, "建立匯入日誌成功。", "0x0200");
                if (SysDefine.EncryptData)
                {
                    result = ApiResponse<object>.Success(EncryptData(model), "建立匯入日誌成功", "0x0200"); // 加密資料
                }
                else
                {
                    result = ApiResponse<object>.Success(model, "建立匯入日誌成功", "0x0200");
                }
                return CreatedAtAction(nameof(Get), new { nid = model.Nid }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        [HttpPut("{nid:int}")]
        public async Task<IActionResult> Update(int nid, [FromBody] LogImport model)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (model == null)
                {
                    return BadRequest(ApiResponse<object>.Fail("更新資料不能為空。", "0x0400"));
                }

                var existing = await _service.GetByIdAsync(nid);
                if (existing == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"找不到 ID 為 {nid} 的匯入日誌，無法更新。", "0x0404"));
                }

                model.Nid = nid;
                await _service.UpdateAsync(model);

                return Ok(ApiResponse<object>.Success("更新匯入日誌成功。", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        [HttpDelete("{nid:int}")]
        public async Task<IActionResult> Delete(int nid)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                var existing = await _service.GetByIdAsync(nid);
                if (existing == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"找不到 ID 為 {nid} 的匯入日誌，無法刪除。", "0x0404"));
                }

                await _service.DeleteAsync(nid);
                return Ok(ApiResponse<object>.Success("刪除匯入日誌成功。", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }
    }
}