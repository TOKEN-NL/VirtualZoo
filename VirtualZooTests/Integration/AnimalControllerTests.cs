using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using VirtualZooShared.Models;
using VirtualZooShared.Factories;
using Xunit;
using VirtualZooAPI;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using VirtualZooShared.Enums;

namespace VirtualZooTests.Integration
{
    /// <summary>
    /// Integration tests voor de AnimalController API endpoints.
    /// </summary>
    public class AnimalControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initialisatie van de testclient met aangepaste content root.
        /// </summary>
        public AnimalControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                var apiPath = Path.GetFullPath("../../../../VirtualZooAPI");
                builder.UseContentRoot(apiPath);
            }).CreateClient();
        }

        /// <summary>
        /// Test of het ophalen van dieren een succesvolle response (200 OK) retourneert.
        /// </summary>
        [Fact]
        public async Task GetAnimals_ShouldReturnOkResponse()
        {
            var response = await _client.GetAsync("/api/animals");
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonSerializer.Deserialize<JsonElement>(rawResponse);

            var animals = JsonSerializer.Deserialize<List<Animal>>(jsonObject.GetProperty("$values").GetRawText());

            Assert.NotNull(animals);
            Assert.NotEmpty(animals);
        }

        /// <summary>
        /// Test of het aanmaken van een dier correct werkt en een 201 Created response geeft.
        /// </summary>
        [Fact]
        public async Task PostAnimal_ShouldReturnCreatedResponse()
        {
            var newAnimal = AnimalFactory.CreateAnimal();
            var response = await _client.PostAsJsonAsync("/api/animals", newAnimal);
            var rawResponse = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var createdAnimal = JsonSerializer.Deserialize<Animal>(rawResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(createdAnimal);
            Assert.Equal(newAnimal.Name, createdAnimal.Name);
        }

        /// <summary>
        /// Test of een dier correct verwijderd kan worden en een 204 No Content response geeft.
        /// </summary>
        [Fact]
        public async Task DeleteAnimal_ShouldReturnNoContentResponse()
        {
            var newAnimal = AnimalFactory.CreateAnimal();
            var createResponse = await _client.PostAsJsonAsync("/api/animals", newAnimal);
            createResponse.EnsureSuccessStatusCode();

            var rawResponse = await createResponse.Content.ReadAsStringAsync();
            var createdAnimal = JsonSerializer.Deserialize<Animal>(rawResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(createdAnimal);
            Assert.True(createdAnimal.Id > 0, "Created animal must have a valid Id");

            var response = await _client.DeleteAsync($"/api/animals/{createdAnimal.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        /// <summary>
        /// Test of de sunrise actie het juiste bericht geeft op basis van het activiteitspatroon.
        /// </summary>
        [Fact]
        public async Task Sunrise_ShouldReturnCorrectMessage()
        {
            var newAnimal = AnimalFactory.CreateAnimal();
            var response = await _client.PostAsJsonAsync("/api/animals", newAnimal);
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadAsStringAsync();
            var createdAnimal = JsonSerializer.Deserialize<Animal>(rawResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var sunriseResponse = await _client.GetAsync($"/api/animals/{createdAnimal.Id}/sunrise");
            sunriseResponse.EnsureSuccessStatusCode();
            var message = await sunriseResponse.Content.ReadAsStringAsync();

            Assert.Contains(createdAnimal.Name, message);
        }

        /// <summary>
        /// Test of de sunset actie het juiste bericht geeft op basis van het activiteitspatroon.
        /// </summary>
        [Fact]
        public async Task Sunset_ShouldReturnCorrectMessage()
        {
            var newAnimal = AnimalFactory.CreateAnimal();
            var response = await _client.PostAsJsonAsync("/api/animals", newAnimal);
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadAsStringAsync();
            var createdAnimal = JsonSerializer.Deserialize<Animal>(rawResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var sunsetResponse = await _client.GetAsync($"/api/animals/{createdAnimal.Id}/sunset");
            sunsetResponse.EnsureSuccessStatusCode();
            var message = await sunsetResponse.Content.ReadAsStringAsync();

            Assert.Contains(createdAnimal.Name, message);
        }

        /// <summary>
        /// Test of de feeding time actie het juiste bericht geeft over wat het dier eet.
        /// </summary>
        [Fact]
        public async Task FeedingTime_ShouldReturnCorrectMessage()
        {
            var newAnimal = AnimalFactory.CreateAnimal();
            var response = await _client.PostAsJsonAsync("/api/animals", newAnimal);
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadAsStringAsync();
            var createdAnimal = JsonSerializer.Deserialize<Animal>(rawResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var feedingResponse = await _client.GetAsync($"/api/animals/{createdAnimal.Id}/feedingtime");
            feedingResponse.EnsureSuccessStatusCode();
            var message = await feedingResponse.Content.ReadAsStringAsync();

            Assert.Contains(createdAnimal.Name, message);
        }

        /// <summary>
        /// Test of checkconstraints feedback geeft over verblijf en veiligheid.
        /// </summary>
        [Fact]
        public async Task CheckConstraints_ShouldReturnCorrectFeedbackForBothCases()
        {
            // Animal dat voldoet aan constraints (klein verblijf + laag beveiligingsniveau)
            var validAnimal = AnimalFactory.CreateAnimal();
            validAnimal.SpaceRequirement = 1; // Klein verblijf
            validAnimal.SecurityRequirement = SecurityLevel.Low; // Laag beveiligingsniveau
            var validCreateResponse = await _client.PostAsJsonAsync("/api/animals", validAnimal);
            validCreateResponse.EnsureSuccessStatusCode();

            var validJson = await validCreateResponse.Content.ReadAsStringAsync();
            var validAnimalObj = JsonSerializer.Deserialize<Animal>(validJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var validCheckResponse = await _client.GetAsync($"/api/animals/{validAnimalObj.Id}/checkconstraints");
            validCheckResponse.EnsureSuccessStatusCode();

            var validRaw = await validCheckResponse.Content.ReadAsStringAsync();
            List<string> validResult;
            try
            {
                validResult = JsonSerializer.Deserialize<List<string>>(validRaw);
            }
            catch
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(validRaw);
                var valuesElement = jsonElement.GetProperty("$values");
                validResult = JsonSerializer.Deserialize<List<string>>(valuesElement.GetRawText());
            }

            Assert.Single(validResult);
            Assert.Contains("Alle eisen zijn voldaan.", validResult);

            // Animal dat niet voldoet aan constraints (te veel ruimte nodig + hoog beveiligingsniveau)
            var invalidAnimal = AnimalFactory.CreateAnimal();
            invalidAnimal.SpaceRequirement = 9999; // Te veel ruimte nodig
            invalidAnimal.SecurityRequirement = SecurityLevel.High; // Hoog beveiligingsniveau
            invalidAnimal.EnclosureId = 1; // Verblijf met ID 1 (security level te laag)
            var invalidCreateResponse = await _client.PostAsJsonAsync("/api/animals", invalidAnimal);
            invalidCreateResponse.EnsureSuccessStatusCode();

            var invalidJson = await invalidCreateResponse.Content.ReadAsStringAsync();
            var invalidAnimalObj = JsonSerializer.Deserialize<Animal>(invalidJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var invalidCheckResponse = await _client.GetAsync($"/api/animals/{invalidAnimalObj.Id}/checkconstraints");
            invalidCheckResponse.EnsureSuccessStatusCode();

            var invalidRaw = await invalidCheckResponse.Content.ReadAsStringAsync();
            List<string> invalidResult;
            try
            {
                invalidResult = JsonSerializer.Deserialize<List<string>>(invalidRaw);
            }
            catch
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(invalidRaw);
                var valuesElement = jsonElement.GetProperty("$values");
                invalidResult = JsonSerializer.Deserialize<List<string>>(valuesElement.GetRawText());
            }

            Assert.Contains("Te weinig ruimte in het verblijf.", invalidResult);
            Assert.Contains("Beveiligingsniveau van verblijf is onvoldoende.", invalidResult);
        }
    }
}
