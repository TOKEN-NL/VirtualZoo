using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using VirtualZooAPI.Services.Interfaces;
using VirtualZooShared.Models;

namespace VirtualZooWebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IAnimalService _animalService;

        public CategoryController(ICategoryService categoryService, IAnimalService animalService)
        {
            _categoryService = categoryService;
            _animalService = animalService;
        }

        private async Task FillViewBag()
        {
            var animals = await _animalService.GetAllAnimalsAsync();
            ViewBag.Animals = animals;
        }

        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                categories = categories.Where(c => c.Name.Contains(searchTerm)).ToList();
            }

            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            await FillViewBag();
            return View(category);
        }

        public async Task<IActionResult> Create()
        {
            await FillViewBag();
            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.AddCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }

            await FillViewBag();
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            await FillViewBag();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category, List<int> AnimalIds)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(category);

                // Ontkoppel bestaande dieren
                var allAnimals = await _animalService.GetAllAnimalsAsync();
                foreach (var animal in allAnimals.Where(a => a.CategoryId == category.Id))
                {
                    animal.CategoryId = null;
                    await _animalService.UpdateAnimalAsync(animal);
                }

                // Koppel nieuwe selectie
                foreach (var animalId in AnimalIds)
                {
                    var animal = await _animalService.GetAnimalByIdAsync(animalId);
                    if (animal != null)
                    {
                        animal.CategoryId = category.Id;
                        await _animalService.UpdateAnimalAsync(animal);
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            await FillViewBag();
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Ontkoppel dieren eerst
            var animals = await _animalService.GetAllAnimalsAsync();
            foreach (var animal in animals.Where(a => a.CategoryId == id))
            {
                animal.CategoryId = null;
                await _animalService.UpdateAnimalAsync(animal);
            }

            await _categoryService.DeleteCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
