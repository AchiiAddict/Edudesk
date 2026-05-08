using Microsoft.AspNetCore.Mvc;
using LearningService.Helper;

namespace LearningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        // Frontend bir sınav görseli veya sertifika istediğinde bu endpoint'i çağıracak
        [HttpGet("view")]
        public IActionResult GetSecureMedia([FromQuery] string path, [FromQuery] long expires, [FromQuery] string signature)
        {
            //HMAC
            bool isValid = UrlSigner.ValidateSignature(path, expires, signature);

            if (!isValid)
            {
                return Unauthorized(new { Mesaj = "Üzgünüm, bu linkin süresi dolmuş veya geçersiz bir imza!" });
            }

            
            return Ok(new { Mesaj = "Güvenli medya erişimi onaylandı.", FilePath = path });
        }

        // Test
        [HttpGet("generate-test-link")]
        public IActionResult GenerateLink(string fileName)
        {
            // 15 dakika geçerli süreli URL
            var secureUrl = UrlSigner.CreateSecureUrl($"/storage/certs/{fileName}", 15);
            return Ok(new { SecureUrl = secureUrl });
        }
    }
}