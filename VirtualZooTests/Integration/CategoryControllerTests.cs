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

namespace VirtualZooTests.Integration
{
    public class CategoryControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CategoryControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                var apiPath = Path.GetFullPath("../../../../VirtualZooAPI");
                builder.UseContentRoot(apiPath);
            }).CreateClient();
        }

        /// <summary>
        /// Test of ophalen van categorieën een succesvolle response (200 OK) retourneert.
        /// </summary>
        [Fact]
        public async Task GetCategories_ShouldReturnOkResponse()
        {
            var response = await _client.GetAsync("/api/categories");
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonSerializer.Deserialize<JsonElement>(rawResponse);

            var categories = JsonSerializer.Deserialize<List<Category>>(jsonObject.GetProperty("$values").GetRawText());

            Assert.NotNull(categories);
            Assert.NotEmpty(categories);
        }

        /// <summary>
        /// Test of het aanmaken van een categorie correct werkt en een 201 Created response geeft.
        /// </summary>
        [Fact]
        public async Task PostCategory_ShouldReturnCreatedResponse()
        {
            var newCategory = CategoryFactory.CreateCategory();
            var response = await _client.PostAsJsonAsync("/api/categories", newCategory);
            var rawResponse = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve 
            };

            var jsonObject = JsonSerializer.Deserialize<JsonElement>(rawResponse, jsonOptions);
            var createdCategory = JsonSerializer.Deserialize<Category>(jsonObject.GetRawText(), jsonOptions);

            Assert.NotNull(createdCategory);
            Assert.Equal(newCategory.Name, createdCategory.Name);
            Assert.NotNull(createdCategory.Animals); 
            Assert.Empty(createdCategory.Animals); 
        }


        /// <summary>
        /// Test of een categorie correct verwijderd kan worden.
        /// </summary>
        [Fact]
        public async Task DeleteCategory_ShouldReturnNoContentResponse()
        {
            var newCategory = CategoryFactory.CreateCategory();
            var createResponse = await _client.PostAsJsonAsync("/api/categories", newCategory);
            createResponse.EnsureSuccessStatusCode();

            var rawResponse = await createResponse.Content.ReadAsStringAsync();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var jsonObject = JsonSerializer.Deserialize<JsonElement>(rawResponse, jsonOptions);
            var createdCategory = JsonSerializer.Deserialize<Category>(jsonObject.GetRawText(), jsonOptions);

            Assert.NotNull(createdCategory);
            Assert.True(createdCategory.Id > 0, "Created category must have a valid Id");

            var response = await _client.DeleteAsync($"/api/categories/{createdCategory.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

    }
}
