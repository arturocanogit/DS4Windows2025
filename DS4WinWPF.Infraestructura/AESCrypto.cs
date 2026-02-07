using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DS4WinWPF.Infraestructura
{
    public class AESCrypto
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AESCrypto()
        {
            // Key de 32 bytes (256 bits) hardcodeada
            _key = Encoding.UTF8.GetBytes("DS4Windows2025SecretKey123456789");
            // IV de 16 bytes hardcodeado
            _iv = Encoding.UTF8.GetBytes("InitVector123456");
        }

        public string Encrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] data = Encoding.UTF8.GetBytes(plainText);
                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public string Decrypt(string encryptedText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] encryptedData = Convert.FromBase64String(encryptedText);
                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(encryptedData))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var result = new MemoryStream())
                {
                    cs.CopyTo(result);
                    return Encoding.UTF8.GetString(result.ToArray());
                }
            }
        }
    }
}