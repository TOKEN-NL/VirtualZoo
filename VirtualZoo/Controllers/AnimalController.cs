using Microsoft.AspNetCore.Mvc;
using VirtualZooAPI.Services.Interfaces;
using VirtualZooShared.Models;
using System.Threading.Tasks;
using VirtualZooAPI.Services.Implementations;

namespace VirtualZooWebApp.Controllers
{
    public class AnimalController : Controller
    {
        private readonly IAnimalService _animalService;
        private readonly ICategoryService _categoryService;
        private readonly IEnclosureService _enclosureService;

        public AnimalController(IAnimalService animalService, ICategoryService categoryService, IEnclosureService enclosureService)
        {
            _animalService = animalService;
            _categoryService = categoryService;
            _enclosureService = enclosureService;
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
    }
}
