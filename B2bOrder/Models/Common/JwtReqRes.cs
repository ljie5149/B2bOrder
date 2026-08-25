namespace B2bOrder.Models.Common
{
    public class JwtRequest
    {
        public string account { get; set; } = string.Empty;
        public string memberSid { get; set; } = string.Empty;
        public string expires { get; set; } = string.Empty;
    }
    public class JwtResult4Decode: JwtRequest
    {
    }
    public class JwtResult4ValidateStandard
    {
        public bool isValid { get; set; } = false;
        public string message { get; set; } = string.Empty;
    }
    public class JwtResult4Validate : JwtResult4ValidateStandard
    {
        public JwtResult4Validate(bool valid, string msg, JwtRequest? tokenData = null)
        {
            isValid     = valid;
            message     = msg;
            TokenData   = tokenData;
        }
        // 將 JwtRequest 變成內部的一個成員屬性
        public JwtRequest? TokenData { get; set; } = new JwtRequest();
    }
    public class JwtResult4Valid : JwtResult4ValidateStandard
    {
        public string memberSid { get; set; } = string.Empty;
    }
    public class JwtResult
    {
        public string token { get; set; } = string.Empty;
        public string expires { get; set; } = string.Empty;
    }
    public class JwtParam
    {
        public string token { get; set; } = string.Empty;
        public string memberSid { get; set; } = string.Empty;
    }
}