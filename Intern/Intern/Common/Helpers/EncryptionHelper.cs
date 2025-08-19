using System.Security.Cryptography;
using System.Text.Json;
using System.Text;

namespace Intern.Common.Helpers
{
    public class EncryptionHelper
    {
        private readonly string _encryptionKey;

        public EncryptionHelper(string encryptionKey)
        {
            _encryptionKey = encryptionKey;
        }

        public string Encrypt(object data)
        {
            var json = JsonSerializer.Serialize(data);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
            aes.GenerateIV();
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(json);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public T Decrypt<T>(string cipherText)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                throw new ArgumentNullException(nameof(cipherText));

            // Fix: Replace spaces with '+' (URL sometimes replaces + with space)
            cipherText = cipherText.Replace(" ", "+");

            // First URL-decode (handles %2F, %2B, etc.)
            var urlDecoded = Uri.UnescapeDataString(cipherText);

            // Fix: Add missing Base64 padding if necessary
            int mod4 = urlDecoded.Length % 4;
            if (mod4 > 0)
            {
                urlDecoded += new string('=', 4 - mod4);
            }

            // Base64 decode
            var fullCipher = Convert.FromBase64String(urlDecoded);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);

            var iv = new byte[aes.BlockSize / 8];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            var json = sr.ReadToEnd();

            return JsonSerializer.Deserialize<T>(json);
        }
    }
}

