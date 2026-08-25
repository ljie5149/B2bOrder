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
    public class DataMemberFileApiController : BaseApiController
    {
        private readonly IDataMemberFileService _service;
        public DataMemberFileApiController(IDataMemberFileService service, IJwtService jwtService, IEncryptionService encryptionService) : base(jwtService, encryptionService)
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
                return Ok(ApiResponse<object>.Success(list, "取得所有會員檔案成功。", "0x0200"));
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
                if (item is null)
                {
                    return NotFound(ApiResponse<object>.Fail($"找不到 ID 為 {nid} 的會員檔案。", "0x0404"));
                }

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(item), "取得會員檔案成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(item, "取得會員檔案成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DataMemberFile model)
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
                    return BadRequest(ApiResponse<object>.Fail("檔案資料不能為空。", "0x0400"));
                }

                await _service.CreateAsync(model);

                var en = EncryptData(model); // 加密資料
                var result = ApiResponse<object>.Success(model, "建立會員檔案成功。", "0x0201");
                return CreatedAtAction(nameof(Get), new { nid = model.Nid }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        [HttpPut("{nid:int}")]
        public async Task<IActionResult> Update(int nid, [FromBody] DataMemberFile model)
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
                    return NotFound(ApiResponse<object>.Fail($"找不到 ID 為 {nid} 的會員檔案，無法更新。", "0x0404"));
                }

                model.Nid = nid;
                await _service.UpdateAsync(model);

                return Ok(ApiResponse<object>.Success("更新會員檔案成功。", "0x0200"));
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
                    return NotFound(ApiResponse<object>.Fail($"找不到 ID 為 {nid} 的會員檔案，無法刪除。", "0x0404"));
                }

                await _service.DeleteAsync(nid);
                return Ok(ApiResponse<object>.Success("刪除會員檔案成功。", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }
    }
}