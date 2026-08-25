using B2bOrder.Models.api;
using B2bOrder.Models.Common;
using B2bOrder.Services.Common;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace B2bOrder.Controllers
{
    public class BaseApiController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IEncryptionService _encryptionService;
        private string? _cachedToken;
        private JwtResult4Validate? _validationResult;

        public BaseApiController(IJwtService jwtService, IEncryptionService encryptionService)
        {
            _jwtService = jwtService;
            _encryptionService = encryptionService;
        }

        /// <summary>
        /// 從 Header 取得並快取 Token 字串。
        /// </summary>
        protected string RawToken
        {
            get
            {
                if (_cachedToken != null)
                {
                    return _cachedToken;
                }

                if (!Request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrEmpty(authHeader))
                {
                    _cachedToken = string.Empty;
                    return _cachedToken;
                }

                _cachedToken = authHeader.ToString().Replace("Bearer ", "").Trim();
                return _cachedToken;
            }
        }

        protected async Task<JwtResult?> generateToken(JwtRequest model)
        {
            return await _jwtService.encodeToken(model);
        }
        /// <summary>
        /// 驗證當前請求的 Token，並將結果快取。
        /// </summary>
        /// <returns>一個包含驗證結果的 <see cref="JwtResult4Validate"/> 物件。</returns>
        protected async Task<JwtResult4Validate> ValidateTokenOnceAsync()
        {
            // 如果已經驗證過，直接回傳快取的結果
            if (_validationResult != null)
            {
                return _validationResult;
            }

            if (string.IsNullOrEmpty(RawToken))
            {
                _validationResult = new JwtResult4Validate(false, "遺失 Token 或 Authorization Header。");
                return _validationResult;
            }

            _validationResult = await _jwtService.ValidateToken(RawToken);
            if (_validationResult.isValid && _validationResult.TokenData != null)
            {
                // 可以在這裡設定 CurrentUser，如果需要的話
                // CurrentUser = ...
            }
            return _validationResult;
        }

        /// <summary>
        /// 取得當前登入使用者的 MemberSid。如果 Token 無效或解析失敗，則回傳空字串。
        /// </summary>
        /// <returns>成功時回傳 MemberSid，失敗時回傳空字串。</returns>
        protected async Task<string> GetUserMemberSidAsync()
        {
            var validationResult = await ValidateTokenOnceAsync();
            if (!validationResult.isValid || validationResult.TokenData == null)
            {
                TokenErrorMessage = validationResult.message;
                return string.Empty;
            }

            TokenErrorMessage = string.Empty;
            return validationResult.TokenData.memberSid;
        }

        /// <summary>
        /// 存放當前請求 Token 解析或驗證失敗時的詳細異常訊息。
        /// </summary>
        protected string TokenErrorMessage { get; private set; } = string.Empty;

        // ===== 新增的加密/解密輔助方法 =====

        /// <summary>
        /// 將物件序列化為 JSON 字串後進行加密。
        /// </summary>
        /// <typeparam name="T">要加密的物件類型。</typeparam>
        /// <param name="data">要加密的物件。</param>
        /// <returns>加密後的 Base64 字串。</returns>
        protected string EncryptData<T>(T data)
        {
            if (data == null)
            {
                return string.Empty;
            }
            string jsonString = JsonSerializer.Serialize(data);
            return _encryptionService.Encrypt(jsonString);
        }

        /// <summary>
        /// 將加密的字串解密後，反序列化為指定的物件類型。
        /// </summary>
        /// <typeparam name="T">預期解密後的物件類型。</typeparam>
        /// <param name="encryptedData">加密的 Base64 字串。</param>
        /// <returns>解密並反序列化後的物件。如果解密或反序列化失敗，則回傳 default(T)。</returns>
        protected T? DecryptData<T>(string encryptedData)
        {
            if (string.IsNullOrEmpty(encryptedData))
            {
                return default;
            }
            try
            {
                string jsonString = _encryptionService.Decrypt(encryptedData);
                return JsonSerializer.Deserialize<T>(jsonString);
            }
            catch
            {
                // 記錄錯誤，或根據需求處理異常
                return default;
            }
        }
    }
}