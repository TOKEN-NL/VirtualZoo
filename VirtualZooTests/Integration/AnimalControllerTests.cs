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
using System.Text.Json.Serialization;
using System.Text.Json;

namespace VirtualZooTests.Integration
{
    public class AnimalControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AnimalControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                var apiPath = Path.GetFullPath("../../../../VirtualZooAPI");
                builder.UseContentRoot(apiPath); 
            }).CreateClient();
        }

        [Fact]
        public async Task GetAnimals_ShouldReturnOkResponse()
        {
            var response = await _client.GetAsync("/api/animal");
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonSerializer.Deserialize<JsonElement>(rawResponse);

            var animals = JsonSerializer.Deserialize<List<Animal>>(jsonObject.GetProperty("$values").GetRawText());

            Assert.NotNull(animals);
            Assert.NotEmpty(animals);
        }

        [Fact]
        public async Task PostAnimal_ShouldReturnCreatedResponse()
        {
            var newAnimal = AnimalFactory.CreateAnimal();
            var response = await _client.PostAsJsonAsync("/api/Animal", newAnimal);
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


        [Fact]
        public async Task DeleteAnimal_ShouldReturnNoContentResponse()
        {
            var newAnimal = AnimalFactory.CreateAnimal();
            var createResponse = await _client.PostAsJsonAsync("/api/Animal", newAnimal);
            createResponse.EnsureSuccessStatusCode();

            var rawResponse = await createResponse.Content.ReadAsStringAsync();
            var createdAnimal = JsonSerializer.Deserialize<Animal>(rawResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(createdAnimal);
            Assert.True(createdAnimal.Id > 0, "Created animal must have a valid Id");

            var response = await _client.DeleteAsync($"/api/Animal/{createdAnimal.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

    }
}
