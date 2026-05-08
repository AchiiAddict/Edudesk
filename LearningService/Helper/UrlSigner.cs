using System.Security.Cryptography;
using System.Text;

namespace LearningService.Helper
{
    public static class UrlSigner
    {
        // Bu anahtar normalde appsettings.json'da saklanmalıdır.
        private static readonly string SecretKey = "EduDeskSuperSecretHmacKey123!";

        public static string CreateSecureUrl(string filePath, int expirationMinutes)
        {
            //Link ne zamana kadar geçerli
            var expiration = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes).ToUnixTimeSeconds();

            //İmzalanacak metni oluşturma
            string rawData = $"{filePath}{expiration}";

            //HMACSHA256 ile kilit
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            string signature = Convert.ToBase64String(hashBytes);

            // Fronthand için
            return $"{filePath}?expires={expiration}&signature={Uri.EscapeDataString(signature)}";
        }

        public static bool ValidateSignature(string filePath, long expires, string signature)
        {
            // Zaman kontrolü
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expires) return false;

            // İmza kontrolü
            string rawData = $"{filePath}{expires}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            string expectedSignature = Convert.ToBase64String(hashBytes);

            return signature == expectedSignature;
        }
    }
}