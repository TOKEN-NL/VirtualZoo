using Microsoft.AspNetCore.Mvc;
using VirtualZooAPI.Services.Interfaces;
using VirtualZooShared.Models;
using System.Threading.Tasks;
using VirtualZooAPI.Services.Implementations;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace VirtualZooWebApp.Controllers
{
    public class AnimalController : Controller
    {
        private readonly IAnimalService _animalService;
        private readonly ICategoryService _categoryService;
        private readonly IEnclosureService _enclosureService;
        private readonly HttpClient _httpClient;

        public AnimalController(IAnimalService animalService, ICategoryService categoryService, IEnclosureService enclosureService, IHttpClientFactory httpClientFactory)
        {
            _animalService = animalService;
            _categoryService = categoryService;
            _enclosureService = enclosureService;
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // HttpClient voor het aanroepen van de API
        public new async Task<string> Request(string query)
        {
            HttpResponseMessage response;
            response = await _httpClient.GetAsync(query);

            response.EnsureSuccessStatusCode();
            var feedback = await response.Content.ReadAsStringAsync();
            
            return feedback;
        }

        // Vul ViewBag met benodigde lijsten
        private async Task FillViewBag()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var enclosures = await _enclosureService.GetAllEnclosuresAsync();
            ViewBag.Categories = categories;
            ViewBag.Enclosures = enclosures;

        }

        // Actie voor het weergeven van de dierenlijst
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var animals = await _animalService.GetAllAnimalsAsync();

            // Filteren op zoekterm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                animals = animals.Where(a => a.Name.Contains(searchTerm)).ToList();
            }
            await FillViewBag();
            return View(animals);
        }

        // Actie voor het weergeven van de details van een dier
        public async Task<IActionResult> Details(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);

            if (animal == null)
            {
                return NotFound();
            }
            await FillViewBag();
            return View(animal);
        }

        // Create - GET
        public async Task<IActionResult> Create()
        {
            await FillViewBag();
            var newAnimal = new Animal();

            return View(newAnimal);
        }

        // Create - POST
        [HttpPost]
        public async Task<IActionResult> Create(Animal animal)
        {
            if (ModelState.IsValid)
            {
                await _animalService.AddAnimalAsync(animal);
                return RedirectToAction(nameof(Index));  
            }

            return View(animal);
        }

        // Edit - GET
        public async Task<IActionResult> Edit(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            await FillViewBag();
            return View(animal);
        }

        // Edit - POST
        [HttpPost]
        public async Task<IActionResult> Edit(Animal animal)
        {
            
            if (ModelState.IsValid)
            {
                await _animalService.UpdateAnimalAsync(animal);
                return RedirectToAction(nameof(Index));  
            }
            return View(animal);
        }

        // Delete - GET
        public async Task<IActionResult> Delete(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            return View(animal);
        }

        // Delete - POST
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _animalService.DeleteAnimalAsync(id);
            return RedirectToAction(nameof(Index));  
        }

        [HttpPost]
        public async Task<IActionResult> SunriseResult(int id)
        {
            var feedback = await Request("/api/animals/" + id + "/sunrise");
            return View("Result", new List<string> { feedback });
        }

        [HttpPost]
        public async Task<IActionResult> SunsetResult(int id)
        {
            var feedback = await Request("/api/animals/" + id + "/sunset");
            return View("Result", new List<string> { feedback });
        }

        [HttpPost]
        public async Task<IActionResult> FeedingTimeResult(int id)
        {
            var feedback = await Request("/api/animals/" + id + "/feedingtime");
            return View("Result", new List<string> { feedback });
        }

        [HttpPost]
        public async Task<IActionResult> CheckConstraintsResult(int id)
        {
            var raw = await Request("/api/animals/" + id + "/checkconstraints");
            var feedback = JsonSerializer.Deserialize<List<string>>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return View("Result", feedback);
        }

    }
}
