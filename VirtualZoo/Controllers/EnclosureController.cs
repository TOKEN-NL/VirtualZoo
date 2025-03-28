using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using VirtualZooAPI.Services.Interfaces;
using VirtualZooShared.Models;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace VirtualZooWebApp.Controllers
{
    public class EnclosureController : Controller
    {
        private readonly IEnclosureService _enclosureService;
        private readonly IAnimalService _animalService;
        private readonly HttpClient _httpClient = new HttpClient();

        public EnclosureController(IEnclosureService enclosureService, IAnimalService animalService, IHttpClientFactory httpClientFactory)
        {
            _enclosureService = enclosureService;
            _animalService = animalService;
            _httpClient = httpClientFactory.CreateClient("ApiClient"); 
        }

        // Vul ViewBag met benodigde lijsten
        private async Task FillViewBag()
        {
            var animals = await _animalService.GetAllAnimalsAsync();
            ViewBag.Animals = animals;
        }
        public new async Task<List<string>> Request(string query)
        {
            HttpResponseMessage response;
            response = await _httpClient.GetAsync(query);
            
            response.EnsureSuccessStatusCode();
            var raw = await response.Content.ReadAsStringAsync();
            var feedback = JsonSerializer.Deserialize<List<string>>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return feedback;
        }
        // Lijst van alle Enclosures
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var enclosures = await _enclosureService.GetAllEnclosuresAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                enclosures = enclosures.Where(e => e.Name.Contains(searchTerm)).ToList();
            }

            await FillViewBag();
            return View(enclosures);
        }

        // Details van één Enclosure
        public async Task<IActionResult> Details(int id)
        {
            var enclosure = await _enclosureService.GetEnclosureByIdAsync(id);
            if (enclosure == null) return NotFound();

            await FillViewBag();
            return View(enclosure);
        }

        // Create - GET
        public async Task<IActionResult> Create()
        {
            await FillViewBag();
            var newEnclosure = new Enclosure();
            return View(newEnclosure);
        }

        // Create - POST
        [HttpPost]
        public async Task<IActionResult> Create(Enclosure enclosure, List<int> AnimalIds)
        {
            if (ModelState.IsValid)
            {
                await _enclosureService.AddEnclosureAsync(enclosure);

                // Koppel dieren
                foreach (var animalId in AnimalIds)
                {
                    var animal = await _animalService.GetAnimalByIdAsync(animalId);
                    if (animal != null)
                    {
                        animal.EnclosureId = enclosure.Id;
                        await _animalService.UpdateAnimalAsync(animal);
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            await FillViewBag();
            return View(enclosure);
        }


        // Edit - GET
        public async Task<IActionResult> Edit(int id)
        {
            var enclosure = await _enclosureService.GetEnclosureByIdAsync(id);
            if (enclosure == null) return NotFound();

            await FillViewBag();
            return View(enclosure);
        }

        // Edit - POST
        [HttpPost]
        public async Task<IActionResult> Edit(Enclosure enclosure, List<int> AnimalIds)
        {
            if (ModelState.IsValid)
            {
                await _enclosureService.UpdateEnclosureAsync(enclosure);

                // Ontkoppel alle dieren
                var allAnimals = await _animalService.GetAllAnimalsAsync();
                foreach (var animal in allAnimals.Where(a => a.EnclosureId == enclosure.Id))
                {
                    animal.EnclosureId = null;
                    await _animalService.UpdateAnimalAsync(animal);
                }

                // Koppel geselecteerde dieren
                foreach (var animalId in AnimalIds)
                {
                    var animal = await _animalService.GetAnimalByIdAsync(animalId);
                    if (animal != null)
                    {
                        animal.EnclosureId = enclosure.Id;
                        await _animalService.UpdateAnimalAsync(animal);
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            await FillViewBag();
            return View(enclosure);
        }

        // Delete - GET
        public async Task<IActionResult> Delete(int id)
        {
            var enclosure = await _enclosureService.GetEnclosureByIdAsync(id);
            if (enclosure == null) return NotFound();

            return View(enclosure);
        }

        // Delete - POST
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Ontkoppel alle dieren
            var animals = await _animalService.GetAllAnimalsAsync();
            foreach (var animal in animals.Where(a => a.EnclosureId == id))
            {
                animal.EnclosureId = null;
                await _animalService.UpdateAnimalAsync(animal);
            }

            // Verwijder de enclosure
            await _enclosureService.DeleteEnclosureAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SunriseResult(int id)
        {
            var feedback = await Request("/api/enclosures/"+id+"/sunrise");
            return View("Result", feedback);
        }

        [HttpPost]
        public async Task<IActionResult> SunsetResult(int id)
        {
            var feedback = await Request("/api/enclosures/"+id+"/sunset");
            return View("Result", feedback);
        }

        [HttpPost]
        public async Task<IActionResult> FeedingTimeResult(int id)
        {
            var feedback = await Request("/api/enclosures/"+id+"/feedingtime");
            return View("Result", feedback);
        }

        [HttpPost]
        public async Task<IActionResult> CheckConstraintsResult(int id)
        {
            var feedback = await Request("/api/enclosures/"+id+"/checkconstraints");
            return View("Result", feedback);
        }

    }
}
