using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SupportService.Models;
using Xunit;

namespace EduDesk.Tests.IntegrationTests
{
    public class SupportApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public SupportApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateTicket_IdempotencyKeyYoksa_BadRequest400Donmeli()
        {
            // Arrange
            var newTicket = new Ticket { Subject = "Test Bilet", Urgency = 1, PlanMultiplier = 1 };

            // Act
            var response = await _client.PostAsJsonAsync("/api/ticket", newTicket);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateTicket_GecerliVeriVeHeaderIle_Ok200Donmeli()
        {
            // Arrange
            var newTicket = new Ticket { Subject = "Sistem Çöktü", Urgency = 5, PlanMultiplier = 3 };
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/ticket");
            request.Content = JsonContent.Create(newTicket);
            request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}