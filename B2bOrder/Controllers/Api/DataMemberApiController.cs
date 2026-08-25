using B2bOrder.Models;
using B2bOrder.Models.api;
using B2bOrder.Models.Common;
using B2bOrder.Models.Db;
using B2bOrder.Models.Tree;
using B2bOrder.Services;
using B2bOrder.Services.Common;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static B2bOrder.Controllers.MemberController;

namespace B2bOrder.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataMemberApiController : BaseApiController
    {
        private readonly ISysUserService _sysuserService;
        private readonly IMemberService _memberService;
        private readonly IPasswordHasher<SysUser> _passwordHasher;

        public DataMemberApiController(
            ISysUserService sysuserService,
            IMemberService memberService,
            IPasswordHasher<SysUser> passwordHasher,
            IEncryptionService encryptionService,
            IJwtService jwtService) : base(jwtService, encryptionService)
        {
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _sysuserService = sysuserService ?? throw new ArgumentNullException(nameof(sysuserService));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        }

        // GET: api/DataMemberApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DataMember>>> GetAll()
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                // 假設此 API 需要特定權限，服務層應根據 memberSid 進行驗證
                var members = await _memberService.GetAllAsync();

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(members), "取得所有會員成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(members, "取得所有會員成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        // GET: api/DataMemberApi/5
        [HttpGet("{nid:int}")]
        public async Task<ActionResult<DataMember>> GetById(int nid)
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

                var member = await _memberService.GetByIdAsync(nid);
                if (member == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"找不到 ID 為 {nid} 的會員資料。", "0x0404"));
                }

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(member), "取得會員資料成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(member, "取得會員資料成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        // GET: api/DataMemberApi/sid/S123456
        [HttpGet("sid/{sid}")]
        public async Task<ActionResult<DataMember>> GetBySid(string sid)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }


                if (string.IsNullOrEmpty(sid))
                {
                    return BadRequest(ApiResponse<object>.Fail("SID 不能為空。", "0x0400"));
                }

                var member = await _memberService.GetBySidAsync( sid);
                if (member == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"找不到 SID 為 {sid} 的會員資料。", "0x0404"));
                }

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(member), "取得會員資料成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(member, "取得會員資料成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        // GET: api/DataMemberApi/mid/M123456
        [HttpGet("mid/{mid}")]
        public async Task<ActionResult<DataMember>> GetByMid(string mid)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (string.IsNullOrEmpty(mid))
                {
                    return BadRequest(ApiResponse<object>.Fail("MID 不能為空。", "0x0400"));
                }
                var member = await _memberService.GetByMidAsync( mid);
                if (member == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"找不到 MID 為 {mid} 的會員資料。", "0x0404"));
                }

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(member), "取得會員資料成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(member, "取得會員資料成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        // GET: api/DataMemberApi/children/S123456
        [HttpGet("children/{parentSid}")]
        public async Task<ActionResult<IEnumerable<DataMember>>> GetChildren(string parentSid)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                var children = await _memberService.GetChildrenAsync( parentSid);

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(children), "取得下線成員成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(children, "取得下線成員成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        // GET: api/DataMemberApi/tree/S123456
        [HttpGet("tree/{sid}")]
        public async Task<ActionResult<MemberTreeData>> GetMemberTree(string sid)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                var treeData = await _memberService.GetMemberTreeDataAsync( sid);
                if (treeData == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"無法取得 SID 為 {sid} 的會員樹狀資料。", "0x0404"));
                }

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(treeData), "取得樹狀資料成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(treeData, "取得樹狀資料成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        // GET: api/DataMemberApi/organization/S123456
        [HttpGet("organization/{sid}")]
        public async Task<ActionResult<MemberTreeData>> GetMemberOrganization(string sid)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                var organizationData = await _memberService.GetOrganizationTreeAsync( sid);
                if (organizationData == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"無法取得 SID 為 {sid} 的會員樹狀資料。", "0x0404"));
                }

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(organizationData), "取得組織網資料成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(organizationData, "取得組織網資料成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        // POST: api/DataMemberApi
        [HttpPost]
        public async Task<ActionResult<DataMember>> Create([FromBody] DataMember member)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (member == null)
                {
                    return BadRequest(ApiResponse<object>.Fail("會員資料不能為空。", "0x0400"));
                }

                // 對齊功能 #2 邏輯：寫入前若有續約日期，統一轉為 UTC 時間
                if (member.ContinueDate.HasValue)
                {
                    member.ContinueDate = member.ContinueDate.Value.ToUniversalTime();
                }

                await _memberService.CreateAsync(member);

                // 保持 CreatedAtAction 的 201 狀態碼，但包裝內容物

                var result = ApiResponse<object>.Success(member, "建立會員成功。", "0x0200");
                if (SysDefine.EncryptData)
                {
                    result = ApiResponse<object>.Success(EncryptData(member), "建立會員成功。", "0x0200");// 加密資料
                }
                else
                {
                    result = ApiResponse<object>.Success(member, "建立會員成功。", "0x0200");
                }
                return CreatedAtAction(nameof(GetById), new { nid = member.Nid }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        // PUT: api/DataMemberApi/5
        [HttpPut("{nid:int}")]
        public async Task<IActionResult> Update(int nid, [FromBody] DataMember member)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (member == null)
                {
                    return BadRequest(ApiResponse<object>.Fail("更新資料不能為空。", "0x0400"));
                }

                if (nid != member.Nid)
                {
                    return BadRequest(ApiResponse<object>.Fail("URL 的 ID 與請求內容中的會員 ID 不符。", "0x0400"));
                }

                // ✨ 依據功能 #2 核心邏輯：更新個人資料時，自動將續約日期轉換為 UTC 時間
                if (member.ContinueDate.HasValue)
                {
                    member.ContinueDate = member.ContinueDate.Value.ToUniversalTime();
                }

                // 權限檢查：確保使用者只能更新自己的資料，或管理者有權限更新
                // 這裡假設服務層會處理權限驗證
                await _memberService.UpdateAsync( member);
                return Ok(ApiResponse<object>.Success("更新會員資料成功。", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail($"更新失敗: {ex.Message}", "0x0500"));
            }
        }

        // PUT: api/DataMemberApi/change-password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] RequestApiModel4ChangePassword model)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                var changePwdData = model.changePasswordData;
                if (changePwdData == null || string.IsNullOrEmpty(changePwdData.OldPassword) || string.IsNullOrEmpty(changePwdData.NewPassword))
                {
                    return BadRequest(ApiResponse<object>.Fail("密碼欄位不可為空。", "0x0400"));
                }

                // 2. 使用來自 Token 的 SID，而不是 model 中的
                var sysUser = await _sysuserService.GetByMemberSidAsync( memberSid);
                if (sysUser == null)
                {
                    // 此處不應發生，因為 Token 有效就該有使用者
                    return NotFound(ApiResponse<object>.Fail("找不到對應的使用者帳號。", "0x0404"));
                }

                // 3. 傳遞 memberSid 以進行驗證
                model.memberSid = memberSid;
                await _sysuserService.UpdateAsync( sysUser, model, true);
                return Ok(ApiResponse<object>.Success("密碼已成功變更。", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail($"變更密碼失敗: {ex.Message}", "0x0500"));
            }
        }

        // PUT: api/DataMemberApi/renewal-request
        [HttpPut("renewal-request")]
        public async Task<IActionResult> SubmitRenewalRequest([FromBody] RenewMemberViewModel model)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                // 2. 使用來自 Token 的 SID 進行操作
                var member = await _memberService.GetBySidAsync( memberSid);

                if (member == null)
                {
                    return NotFound(ApiResponse<object>.Fail("找不到您的會員資料。", "0x0404"));
                }

                await _memberService.UpdateAsync( member, true);
                return Ok(ApiResponse<object>.Success("續約申請已成功提交。", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail($"提交續約申請失敗: {ex.Message}", "0x0500"));
            }
        }

        // DELETE: api/DataMemberApi/5
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

                var member = await _memberService.GetByIdAsync( nid);
                if (member == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"找不到 ID 為 {nid} 的會員，無法刪除。", "0x0404"));
                }

                // 權限檢查：服務層應驗證 memberSid 是否有權限刪除此 nid
                await _memberService.DeleteAsync( nid);
                return Ok(ApiResponse<object>.Success("刪除成功。", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail($"刪除失敗: {ex.Message}", "0x0500"));
            }
        }
    }

    #region 功能 #2 資料傳輸物件 (DTOs)

    public class ChangePasswordRequestDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class RenewalRequestDto
    {
        public string RenewalPlanId { get; set; }
        public string Notes { get; set; }
        public decimal Amount { get; set; }
    }

    #endregion
}