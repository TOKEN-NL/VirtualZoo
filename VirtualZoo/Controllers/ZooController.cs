using Microsoft.AspNetCore.Mvc;
using VirtualZooAPI.Services.Interfaces;
using VirtualZoo.ViewModels;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace VirtualZoo.Controllers
{
    public class ZooController : Controller
    {
        private readonly IAnimalService _animalService;
        private readonly IEnclosureService _enclosureService;
        private readonly ICategoryService _categoryService;
        private readonly IZooService _zooService;
        private readonly HttpClient _httpClient;

        public ZooController(
            IAnimalService animalService,
            IEnclosureService enclosureService,
            ICategoryService categoryService,
            IZooService zooService,
            IHttpClientFactory httpClientFactory)
        {
            _animalService = animalService;
            _enclosureService = enclosureService;
            _categoryService = categoryService;
            _zooService = zooService;
            _httpClient = httpClientFactory.CreateClient("ApiClient"); 
        }

        public async Task<IActionResult> Index()
        {
            var model = new ZooViewModel
            {
                Animals = (await _animalService.GetAllAnimalsAsync()).ToList(),
                Enclosures = (await _enclosureService.GetAllEnclosuresAsync()).ToList(),
                Categories = (await _categoryService.GetAllCategoriesAsync()).ToList()
            };

            return View(model);
        }

        public async Task<List<string>> Request(string query)
        {
            //var response = await _zooService.AutoAssignAsync(resetExisting);
            var response = await _httpClient.PostAsync($"{query}", null);
            response.EnsureSuccessStatusCode();
            var raw = await response.Content.ReadAsStringAsync();
            var feedback = JsonSerializer.Deserialize<List<string>>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return feedback;
        }

        [HttpPost]
        public async Task<IActionResult> Sunrise()
        {
            await _httpClient.PostAsync("/api/zoo/sunrise", null);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Sunset()
        {
            await _httpClient.PostAsync("/api/zoo/sunset", null);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> FeedingTime()
        {
            await _httpClient.PostAsync("/api/zoo/feedingtime", null);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CheckConstraints()
        {
            await _httpClient.PostAsync("/api/zoo/checkconstraints", null);
            return RedirectToAction(nameof(Index));
        }

        // AutoAssign Confirm Page
        public IActionResult ConfirmAutoAssign()
        {
            return View(); 
        }

        // AutoAssign Handler
        [HttpPost]
        public async Task<IActionResult> AutoAssignResponse(bool resetExisting)
        {

            var feedback = await Request("api/zoo/autoassign?resetExisting=" + resetExisting.ToString().ToLower());
            
            return View("AutoAssignResult", feedback);
        }
    }
}
