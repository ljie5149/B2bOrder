using B2bOrder.Models.api;
using B2bOrder.Models.Common;
using B2bOrder.Services;
using B2bOrder.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace B2bOrder.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SysConfigApiController : BaseApiController
    {
        private readonly ISysConfigService _configService;

        public SysConfigApiController(ISysConfigService configService, IJwtService jwtService, IEncryptionService encryptionService) : base(jwtService, encryptionService)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        /// <summary>
        /// 獲取特定 Key 的參數詳細資料
        /// GET: api/SysConfigApi/key/SYSTEM_NAME
        /// </summary>
        [HttpGet("key/{configKey}")]
        public async Task<IActionResult> GetConfigByKey(string configKey)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (string.IsNullOrWhiteSpace(configKey))
                {
                    return BadRequest(ApiResponse<object>.Fail("設定鍵值不能為空。", "0x0400"));
                }

                var config = await _configService.GetConfigByKeyAsync(configKey);
                if (config == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"找不到設定鍵值: {configKey}", "0x0404"));
                }

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(config), "取得參數詳細資料成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(config, "取得參數詳細資料成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 獲取特定 Key 的參數值 (包裝為統一格式)
        /// GET: api/SysConfigApi/value/SYSTEM_NAME
        /// </summary>
        [HttpGet("value/{configKey}")]
        public async Task<IActionResult> GetValueByKey(string configKey)
        {
            try
            {
                // 根據需求，此端點可能允許未登入訪問，但為保持一致性，仍建議加入驗證
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (string.IsNullOrWhiteSpace(configKey))
                {
                    return BadRequest(ApiResponse<object>.Fail("設定鍵值不能為空。", "0x0400"));
                }

                var value = await _configService.GetStringValueAsync(configKey, null!);
                if (value == null)
                {
                    return NotFound(ApiResponse<object>.Fail($"找不到設定鍵值: {configKey}", "0x0404"));
                }

                // 💡 優化：原本直接塞字串，現在包裝成匿名物件對齊 data 屬性，方便前端讀取
                var result = new { configValue = value };
                var en = EncryptData(result); // 加密資料

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(result), "取得參數值成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(result, "取得參數值成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 依據分類分類群組獲取設定清單
        /// GET: api/SysConfigApi/category/BASIC
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetConfigsByCategory(string category)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                var configs = await _configService.GetConfigsByCategoryAsync(category);

                if (SysDefine.EncryptData)
                {
                    return Ok(ApiResponse<object>.Success(EncryptData(configs), "取得分類設定清單成功", "0x0200")); // 加密資料
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(configs, "取得分類設定清單成功", "0x0200"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }

        /// <summary>
        /// 更新特定 Key 的參數值
        /// PUT: api/SysConfigApi/update
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateConfigValue([FromBody] UpdateConfigDto model)
        {
            try
            {
                // 1. 驗證 Token
                var memberSid = await GetUserMemberSidAsync();
                if (string.IsNullOrEmpty(memberSid))
                {
                    return Unauthorized(ApiResponse<object>.Fail(TokenErrorMessage, "0x0401"));
                }

                if (model == null || string.IsNullOrWhiteSpace(model.ConfigKey))
                {
                    return BadRequest(ApiResponse<object>.Fail("請求參數錯誤，鍵值不可為空。", "0x0400"));
                }

                // 權限檢查：服務層應根據 memberSid 驗證是否有權限更新系統參數
                var result = await _configService.UpdateConfigValueAsync(model.ConfigKey, model.ConfigValue);
                if (!result)
                {
                    return Ok(ApiResponse<object>.Fail("更新失敗，請確認鍵值是否存在。", "0x0201"));
                }

                return Ok(ApiResponse<object>.Success(null, "參數更新成功", "0x0200"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Fail(ex.Message, "0x0500"));
            }
        }
    }

    /// <summary>
    /// 用於接收更新參數的傳輸物件 (DTO)
    /// </summary>
    public class UpdateConfigDto
    {
        public string ConfigKey { get; set; } = string.Empty;
        public string? ConfigValue { get; set; }
    }
}