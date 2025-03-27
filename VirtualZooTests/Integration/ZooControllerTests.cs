using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using VirtualZooAPI;
using Xunit;
using System.IO;

namespace VirtualZooTests.Integration
{
    /// <summary>
    /// Integration tests voor de ZooController API endpoints.
    /// </summary>
    public class ZooControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initialisatie van de testclient met aangepaste content root.
        /// </summary>
        public ZooControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                var apiPath = Path.GetFullPath("../../../../VirtualZooAPI");
                builder.UseContentRoot(apiPath);
            }).CreateClient();
        }

        /// <summary>
        /// Test of Sunrise een lijst van dierreacties retourneert.
        /// </summary>
        [Fact]
        public async Task Sunrise_ShouldReturnAnimalActivityMessages()
        {
            var response = await _client.GetAsync("/api/zoo/sunrise");
            response.EnsureSuccessStatusCode();

            var raw = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonSerializer.Deserialize<JsonElement>(raw);
            var messages = JsonSerializer.Deserialize<List<string>>(jsonObject.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });

            Assert.NotNull(messages);
            Assert.All(messages, msg =>
                Assert.True(
                    msg.Contains("wordt wakker", System.StringComparison.OrdinalIgnoreCase) ||
                    msg.Contains("gaat slapen", System.StringComparison.OrdinalIgnoreCase) ||
                    msg.Contains("actief", System.StringComparison.OrdinalIgnoreCase),
                    $"Onverwachte boodschap: {msg}"));
        }

        /// <summary>
        /// Test of Sunset een lijst van dierreacties retourneert.
        /// </summary>
        [Fact]
        public async Task Sunset_ShouldReturnAnimalActivityMessages()
        {
            var response = await _client.GetAsync("/api/zoo/sunset");
            response.EnsureSuccessStatusCode();

            var raw = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonSerializer.Deserialize<JsonElement>(raw);
            var messages = JsonSerializer.Deserialize<List<string>>(jsonObject.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });

            Assert.NotNull(messages);
            Assert.All(messages, msg =>
                Assert.True(
                    msg.Contains("wordt wakker", System.StringComparison.OrdinalIgnoreCase) ||
                    msg.Contains("gaat slapen", System.StringComparison.OrdinalIgnoreCase) ||
                    msg.Contains("actief", System.StringComparison.OrdinalIgnoreCase),
                    $"Onverwachte boodschap: {msg}"));
        }

        /// <summary>
        /// Test of FeedingTime de juiste eetinformatie retourneert.
        /// </summary>
        [Fact]
        public async Task FeedingTime_ShouldReturnFeedingInfo()
        {
            var response = await _client.GetAsync("/api/zoo/feedingtime");
            response.EnsureSuccessStatusCode();

            var raw = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonSerializer.Deserialize<JsonElement>(raw);
            var messages = JsonSerializer.Deserialize<List<string>>(jsonObject.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });

            Assert.NotNull(messages);
            Assert.All(messages, msg =>
                Assert.Contains("eet", msg, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Test of CheckConstraints teruggeeft of dieren voldoen aan verblijfseisen.
        /// </summary>
        [Fact]
        public async Task CheckConstraints_ShouldReturnConstraintFeedback()
        {
            var response = await _client.GetAsync("/api/zoo/checkconstraints");
            response.EnsureSuccessStatusCode();

            var raw = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonSerializer.Deserialize<JsonElement>(raw);
            var feedback = JsonSerializer.Deserialize<List<string>>(jsonObject.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });

            Assert.NotNull(feedback);
            Assert.NotEmpty(feedback);
        }
    }
}
