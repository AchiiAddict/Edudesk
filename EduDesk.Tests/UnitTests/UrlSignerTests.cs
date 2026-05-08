using LearningService.Helper;
using Xunit;

namespace EduDesk.Tests.UnitTests
{
    public class UrlSignerTests
    {
        [Fact]
        public void CreateSecureUrl_GecerliBirUrlVeImzaUretmeli()
        {
            // Arrange
            string filePath = "/certs/test.pdf";
            int expireMins = 15;

            // Act
            string secureUrl = UrlSigner.CreateSecureUrl(filePath, expireMins);

            // Assert
            Assert.Contains("?expires=", secureUrl);
            Assert.Contains("&signature=", secureUrl);
            Assert.StartsWith(filePath, secureUrl);
        }

        [Fact]
        public void ValidateSignature_SuresiDolmusLinkIcin_FalseDonmeli()
        {
            // Arrange
            string filePath = "/certs/test.pdf";
            long pastExpiration = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds();
            string fakeSignature = "rastgele_gecersiz_imza_123";

            // Act
            bool isValid = UrlSigner.ValidateSignature(filePath, pastExpiration, fakeSignature);

            // Assert
            Assert.False(isValid);
        }
    }
}