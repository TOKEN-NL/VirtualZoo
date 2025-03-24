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
    }
}
