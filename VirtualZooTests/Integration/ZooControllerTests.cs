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
using System.Net.Http.Json;
using VirtualZooAPI.Factories;
using VirtualZooShared.Enums;
using VirtualZooShared.Models;

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

        /// <summary>
        /// Integration tests voor de AutoAssign actie in de ZooController.
        /// </summary>
        public class ZooControllerAutoAssignTests : IClassFixture<WebApplicationFactory<Program>>
        {
            private readonly HttpClient _client;

            public ZooControllerAutoAssignTests(WebApplicationFactory<Program> factory)
            {
                _client = factory.WithWebHostBuilder(builder =>
                {
                    var apiPath = Path.GetFullPath("../../../../VirtualZooAPI");
                    builder.UseContentRoot(apiPath);
                }).CreateClient();
            }

            /// <summary>
            /// Test of dieren zonder verblijf automatisch gekoppeld worden aan een nieuw verblijf.
            /// </summary>
            [Fact]
            public async Task AutoAssign_ShouldAssignAnimalsWithoutEnclosure()
            {
                var animal = AnimalFactory.CreateAnimal();
                animal.EnclosureId = null;

                var createResponse = await _client.PostAsJsonAsync("/api/animals", animal);
                createResponse.EnsureSuccessStatusCode();

                var assignResponse = await _client.PostAsync("/api/zoo/autoassign?resetExisting=false", null);
                assignResponse.EnsureSuccessStatusCode();

                var raw = await assignResponse.Content.ReadAsStringAsync();
                var feedback = JsonSerializer.Deserialize<List<string>>(raw, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });

                Assert.NotNull(feedback);
                Assert.Contains(feedback, msg => msg.Contains("gekoppeld"));
            }

            /// <summary>
            /// Test of alle verblijven en koppelingen worden verwijderd bij reset=true.
            /// </summary>
            [Fact]
            public async Task AutoAssign_WithReset_ShouldClearAndReassignAll()
            {
                var assignResponse = await _client.PostAsync("/api/zoo/autoassign?resetExisting=true", null);
                assignResponse.EnsureSuccessStatusCode();

                var raw = await assignResponse.Content.ReadAsStringAsync();
                var feedback = JsonSerializer.Deserialize<List<string>>(raw, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });

                Assert.NotNull(feedback);
                Assert.Contains(feedback, msg => msg.Contains("verblijven zijn verwijderd") || msg.Contains("gekoppeld"));
            }

            /// <summary>
            /// Test dat de actie een geldige lijst met strings teruggeeft zonder fouten.
            /// </summary>
            [Fact]
            public async Task AutoAssign_ShouldReturnValidFeedbackList()
            {
                var response = await _client.PostAsync("/api/zoo/autoassign?resetExisting=false", null);
                response.EnsureSuccessStatusCode();

                var raw = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<string>>(raw, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });

                Assert.NotNull(result);
                Assert.All(result, msg => Assert.False(string.IsNullOrWhiteSpace(msg)));
            }
        }
    }
}
