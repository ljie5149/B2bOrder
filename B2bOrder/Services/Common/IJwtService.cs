using B2bOrder.Models.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace B2bOrder.Services.Common
{
    public interface IJwtService
    {
        Task<JwtResult?> encodeToken(JwtRequest model);
        Task<JwtResult4Decode?> decodeToken(string token);
        Task<JwtResult4Validate> ValidateToken(string token);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _offsetHours;
        private readonly SymmetricSecurityKey _securityKey;

        public JwtService(IConfiguration config)
        {
            _config = config;

            // 1. 在建構子初始化時就將設定檔讀入，避免每次呼叫方法都重複讀取
            _issuer = _config["Jwt:Issuer"] ?? "";
            _audience = _config["Jwt:Audience"] ?? "";
            _offsetHours = _config["Jwt:ExipredHours"] ?? "0";

            var secretKey = _config["Jwt:SecretKey"] ?? "";
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
            {
                throw new InvalidOperationException("JWT SecretKey 缺失或長度不足 32 個字元。");
            }

            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        /// <summary>
        /// 產生 Token
        /// </summary>
        public Task<JwtResult?> encodeToken(JwtRequest model)
        {
            try
            {
                var hours = int.Parse(_offsetHours);
                model.expires = DateTime.Now.AddHours(hours).ToString("yyyy-MM-dd HH:mm:ss");

                var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, model.account),
                    new Claim("memberSid", model.memberSid), // 注意這裡的 Key 是 memberSid
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.Parse(model.expires),
                    signingCredentials: credentials);

                var jwtRes = new JwtResult
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expires = model.expires
                };

                return Task.FromResult<JwtResult?>(jwtRes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating token: {ex.Message}");
                return Task.FromResult<JwtResult?>(null);
            }
        }

        /// <summary>
        /// 僅解密 Token (不驗證時效，只拿來讀資料)
        /// </summary>
        public Task<JwtResult4Decode?> decodeToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                // 這裡改用建構子初始化好的 _securityKey
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = false, // 僅單純 Decode 時，通常可以不阻擋過期 Token
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var ret = new JwtResult4Decode
                {
                    account = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value,
                    memberSid = jwtToken.Claims.First(x => x.Type == "memberSid").Value,
                    expires = jwtToken.ValidTo.ToString("yyyy-MM-dd HH:mm:ss")
                };
                return Task.FromResult<JwtResult4Decode?>(ret);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding token: {ex.Message}");
                return Task.FromResult<JwtResult4Decode?>(null);
            }
        }

        /// <summary>
        /// 嚴格驗證並解析 Token (推薦在 Middleware 或 Filter 使用)
        /// </summary>
        public Task<JwtResult4Validate> ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(new JwtResult4Validate(false, msg: "Token 不可為空。"));
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _securityKey,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                // 驗證演算法是否為 HmacSha256
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return Task.FromResult(new JwtResult4Validate(false, msg: "Token 使用了無效的簽章演算法。"));
                }

                // 1. 提取 sub (對應 "9015617")
                // 點網系統會自動將 jwt 中的 "sub" 對應到 ClaimTypes.NameIdentifier 或 JwtRegisteredClaimNames.Sub
                var account = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == ClaimTypes.NameIdentifier)?.Value;

                // 2. 提取 memberSid (對應 "de94dc0069a848928238824221f0d729")
                // 這裡必須是 "memberSid"，如果寫成 "member_sid" 就會找不到而回傳 null
                var memberSid = principal.Claims.FirstOrDefault(c => c.Type == "memberSid")?.Value;


                if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(memberSid))
                {
                    return Task.FromResult(new JwtResult4Validate(false, msg: "Token 中缺少必要的宣告 (sub 或 memberSid)。"));
                }

                var retToken = new JwtRequest
                {
                    account = account,
                    memberSid = memberSid,
                    expires = jwtSecurityToken.ValidTo.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
                    // 提示：ValidTo 預設是 UTC 時間，建議加上 .ToLocalTime() 轉回台灣當地時間
                };

                return Task.FromResult(new JwtResult4Validate(true, msg: "Token 驗證成功。", retToken));
            }
            catch (SecurityTokenExpiredException)
            {
                return Task.FromResult(new JwtResult4Validate(false, msg: "Token 已過期。"));
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return Task.FromResult(new JwtResult4Validate(false, msg: "Token 簽章無效。"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new JwtResult4Validate(false, msg: $"Token 驗證失敗: {ex.Message}"));
            }
        }
    }
}