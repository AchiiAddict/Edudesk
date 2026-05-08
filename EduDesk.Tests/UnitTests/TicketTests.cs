using SupportService.Models;
using Xunit;

namespace EduDesk.Tests.UnitTests
{
    public class TicketTests
    {
        [Fact]
        public void BracketScore_SlaGecikmisse_MaksimumAciliyetIcin_DogruSkoruUretmeli()
        {
            // Arrange
            var ticket = new Ticket
            {
                Urgency = 5,
                PlanMultiplier = 3, 
                SlaDeadline = DateTime.UtcNow.AddHours(-2) 
            };

            // Act
            var score = ticket.BracketScore;

            // Assert
            Assert.Equal(1.5, score);
        }

        [Fact]
        public void BracketScore_CozulmusBiletlerIcin_SifirDonmeli()
        {
            // Arrange
            var ticket = new Ticket { IsResolved = true, Urgency = 5, PlanMultiplier = 2 };

            // Act
            var score = ticket.BracketScore;

            // Assert
            Assert.Equal(0, score);
        }

        [Fact]
        public void BracketScore_NormalSartlarda_DogruMatematikselHesabiYapmali()
        {
            // Arrange
            var ticket = new Ticket
            {
                Urgency = 2,
                PlanMultiplier = 2,
                SlaDeadline = DateTime.UtcNow.AddHours(5)
            };

            // Act
            var score = ticket.BracketScore;

            // Assert
            Assert.InRange(score, 19.5, 20.5);
        }
    }
}