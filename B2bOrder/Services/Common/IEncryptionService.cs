using System.Security.Cryptography;
using System.Text;

namespace B2bOrder.Services.Common
{
    /// <summary>
    /// 提供字串加密和解密功能的服務。
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        /// 將指定的純文字加密。
        /// </summary>
        /// <param name="plainText">要加密的純文字。</param>
        /// <returns>加密後的 Base64 字串。</returns>
        string Encrypt(string plainText);

        /// <summary>
        /// 將指定的加密字串解密。
        /// </summary>
        /// <param name="cipherText">要解密的 Base64 字串。</param>
        /// <returns>解密後的純文字。</returns>
        string Decrypt(string cipherText);
    }
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionService(IConfiguration configuration)
        {
            var section = configuration.GetSection("Encryption");
            var keyString = section["Key"];
            var ivString = section["IV"];

            if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(ivString))
            {
                throw new ArgumentException("Encryption Key 或 IV 未在設定檔中設定。");
            }

            _key = Encoding.UTF8.GetBytes(keyString);
            _iv = Encoding.UTF8.GetBytes(ivString);

            if (_key.Length != 32)
            {
                throw new ArgumentException("Encryption Key 必須是 32 個位元組。");
            }
            if (_iv.Length != 16)
            {
                throw new ArgumentException("Encryption IV 必須是 16 個位元組。");
            }
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return string.Empty;
            }

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using var swEncrypt = new StreamWriter(csEncrypt);
                swEncrypt.Write(plainText);
            }
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return string.Empty;
            }

            byte[] buffer = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var msDecrypt = new MemoryStream(buffer);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
}