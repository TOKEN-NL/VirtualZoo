using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using VirtualZooShared.Models;
using VirtualZooAPI.Factories;
using Xunit;
using VirtualZooAPI;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace VirtualZooTests.Integration
{
    /// <summary>
    /// Integration tests voor de EnclosureController API endpoints.
    /// </summary>
    public class EnclosureControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initialisatie van de testclient met aangepaste content root.
        /// </summary>
        public EnclosureControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                var apiPath = Path.GetFullPath("../../../../VirtualZooAPI");
                builder.UseContentRoot(apiPath);
            }).CreateClient();
        }

        /// <summary>
        /// Test of ophalen van verblijven een succesvolle response (200 OK) retourneert.
        /// </summary>
        [Fact]
        public async Task GetEnclosures_ShouldReturnOkResponse()
        {
            var response = await _client.GetAsync("/api/enclosures");
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonSerializer.Deserialize<JsonElement>(rawResponse);

            var enclosures = JsonSerializer.Deserialize<List<Enclosure>>(jsonObject.GetProperty("$values").GetRawText());

            Assert.NotNull(enclosures);
            Assert.NotEmpty(enclosures);
        }

        /// <summary>
        /// Test of het aanmaken van een verblijf correct werkt en een 201 Created response geeft.
        /// </summary>
        [Fact]
        public async Task PostEnclosure_ShouldReturnCreatedResponse()
        {
            var newEnclosure = EnclosureFactory.CreateEnclosure();
            var response = await _client.PostAsJsonAsync("/api/enclosures", newEnclosure);
            var rawResponse = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var jsonObject = JsonSerializer.Deserialize<JsonElement>(rawResponse, jsonOptions);
            var createdEnclosure = JsonSerializer.Deserialize<Enclosure>(jsonObject.GetRawText(), jsonOptions);

            Assert.NotNull(createdEnclosure);
            Assert.Equal(newEnclosure.Name, createdEnclosure.Name);
            Assert.NotNull(createdEnclosure.Animals);
        }

        /// <summary>
        /// Test of een verblijf correct verwijderd kan worden.
        /// </summary>
        [Fact]
        public async Task DeleteEnclosure_ShouldReturnNoContentResponse()
        {
            var newEnclosure = EnclosureFactory.CreateEnclosure();
            var createResponse = await _client.PostAsJsonAsync("/api/enclosures", newEnclosure);
            createResponse.EnsureSuccessStatusCode();

            var rawResponse = await createResponse.Content.ReadAsStringAsync();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var jsonObject = JsonSerializer.Deserialize<JsonElement>(rawResponse, jsonOptions);
            var createdEnclosure = JsonSerializer.Deserialize<Enclosure>(jsonObject.GetRawText(), jsonOptions);

            Assert.NotNull(createdEnclosure);
            Assert.True(createdEnclosure.Id > 0, "Created enclosure must have a valid Id");

            var response = await _client.DeleteAsync($"/api/enclosures/{createdEnclosure.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        /// <summary>
        /// Test of de Sunrise-actie een lijst teruggeeft met activiteit van dieren in het verblijf.
        /// </summary>
        [Fact]
        public async Task Sunrise_ShouldReturnAnimalActivityMessages()
        {
            var response = await _client.GetAsync("/api/enclosures/1/sunrise");
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
                msg.Contains("wordt wakker", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("gaat slapen", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("actief", StringComparison.OrdinalIgnoreCase),
                $"Unexpected message: {msg}"));
        }

        /// <summary>
        /// Test of de Sunset-actie een lijst teruggeeft met activiteit van dieren in het verblijf.
        /// </summary>
        [Fact]
        public async Task Sunset_ShouldReturnAnimalActivityMessages()
        {
            var response = await _client.GetAsync("/api/enclosures/1/sunset");
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
                msg.Contains("wordt wakker", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("gaat slapen", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("actief", StringComparison.OrdinalIgnoreCase),
                $"Unexpected message: {msg}"));
        }

        /// <summary>
        /// Test of de FeedingTime-actie correcte informatie geeft over het dieet van de dieren.
        /// </summary>
        [Fact]
        public async Task FeedingTime_ShouldReturnFeedingInfo()
        {
            var response = await _client.GetAsync("/api/enclosures/1/feedingtime");
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
                Assert.Contains("eet", msg, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Test of de CheckConstraints-actie teruggeeft of alle eisen in het verblijf worden nageleefd.
        /// </summary>
        [Fact]
        public async Task CheckConstraints_ShouldReturnConstraintFeedback()
        {
            var response = await _client.GetAsync("/api/enclosures/1/checkconstraints");
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