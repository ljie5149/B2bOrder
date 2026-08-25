using B2bOrder.Models.api;
using B2bOrder.Models.Common;
using B2bOrder.Services;
using B2bOrder.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace B2bOrder.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SysUserApiController : BaseApiController
    {
        private readonly ISysUserService _service;
        private readonly IMemberService _memberService;

        public SysUserApiController(ISysUserService service, IMemberService memberService, IJwtService jwtService, IEncryptionService encryptionService) : base(jwtService, encryptionService)
        {
            _service = service;
            _memberService = memberService;
        }

        // ==================== 🟢 公開 API：登入與註冊 ====================
        [HttpGet("login")]
        public async Task<IActionResult> Login([FromBody] RequestApiModel4Login model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Mid) || string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest(ApiResponse<object>.Fail("請輸入帳號與密碼。", "0x0400"));
                }

                model.isApi = true; // 標記為 API 登入
                var jwt = await _service.Login(model);

                if (jwt == null)
                {
                    return Unauthorized(ApiResponse<object>.Fail("帳號或密碼錯誤。", "0x0401"));
                }

                var input = new JwtRequest
                {
                    account     = jwt.Account,
                    memberSid   = jwt.MemberSid,
                    expires     = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") // 設定 JWT 過期時間為 1 小時後
                };
                var ret = await generateToken(input);

                // 回傳 JSON 結果給 SwiftUI 
                // 攜帶 account 與 memberSid，讓 App 接下來能去撈取會員詳細資料
                var loginResult = ret;
                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(loginResult), "登入成功", "0x0200")); // 加密資料
                } else
                {
                    return Ok(ApiResponse<object>.Success(loginResult, "登入成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RequestApiModel4Register model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(ApiResponse<object>.Fail("註冊資料不能為空。", "0x0400"));
                }

                model.isApi = true; // 標記為 API 註冊
                var res = await _service.Register(model);

                // 💡 修正：將 null 檢查移至最上方，防止後續讀取 .Remark 時發生 NullReferenceException
                if (res == null)
                {
                    return Unauthorized(ApiResponse<object>.Fail("此會員編號已被註冊過!", "0x0401"));
                }

                var registerResult = new
                {
                    account = res.Account,
                    memberSid = res.MemberSid ?? string.Empty
                };

                // 情況1：會員存在但登入帳號不存在
                if (res.Remark == "1")
                {
                    if (SysDefine.EncryptData)
                    {
                        return Ok(ApiResponse<object>.Success(EncryptData(registerResult), $"已是組織一員，註冊成為會員：{res.Account} 帳號建立成功", "0x0200")); // 加密資料
                    }
                    else
                    {
                        return Ok(ApiResponse<object>.Success(registerResult, $"已是組織一員，註冊成為會員：{res.Account} 帳號建立成功", "0x0200"));
                    }
                }

                // 情況2：新建立帳號成功
                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(registerResult), $"建立帳號成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(registerResult, $"建立帳號成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        // ==================== 🔒 需授權的後台管理 API ====================

        [HttpGet]
        public async Task<IActionResult> GetOwner()
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                // 權限檢查：服務層應根據 memberSid 驗證是否有權限獲取所有使用者列表
                var list = await _service.GetByMemberSidAsync(memberSid);

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(list), "取得所有使用者成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(list, "取得所有使用者成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        [HttpGet("All")]
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

                // 權限檢查：服務層應根據 memberSid 驗證是否有權限獲取所有使用者列表
                var list = await _service.GetAllAsync();

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(list), "取得所有使用者成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(list, "取得所有使用者成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        [HttpGet("One/{nid:int}")]
        public async Task<IActionResult> Get(int nid)
        {
            try
            {
                // 1. 驗證 Token
                var tokenMemberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(tokenMemberSid))
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
                    return NotFound(ApiResponse<object>.Fail($"找不到 ID 為 {nid} 的系統使用者。", "0x0404"));
                }

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(item), "取得系統使用者成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(item, "取得系統使用者成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RequestApiModel4SysUser model)
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
                var nid = model.nid;
                var existing = await _service.GetByIdAsync(nid);
                if (existing == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"找不到 ID 為 {nid} 的系統使用者，無法更新。", "0x0404"));
                }

                // 將傳入的 model.sysUser 的屬性值，更新到從資料庫取出的 existing 實體上
                // 這裡假設 SysUser 是一個 POCO 物件，您可以手動或使用 AutoMapper 等工具來進行屬性對應
                existing.Avalible            = model.sysUser.Avalible;
                existing.Remark              = model.sysUser.Remark;

                await _service.UpdateAsync(existing);
                return Ok(ApiResponse<object>.Success("更新系統使用者成功。", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] RequestApiModel4SysUserNid model)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                var nid = model.nid;
                var existing = await _service.GetByIdAsync(nid);
                if (existing == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"找不到 ID 為 {nid} 的系統使用者，無法刪除。", "0x0404"));
                }

                await _service.DeleteAsync(nid);
                return Ok(ApiResponse<object>.Success("刪除系統使用者成功。", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }
    }
}