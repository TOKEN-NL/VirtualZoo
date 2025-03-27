using Microsoft.AspNetCore.Mvc;
using VirtualZooAPI.Services.Interfaces;
using VirtualZoo.ViewModels;
using System.Threading.Tasks;

namespace VirtualZoo.Controllers
{
    public class ZooController : Controller
    {
        private readonly IAnimalService _animalService;
        private readonly IEnclosureService _enclosureService;
        private readonly ICategoryService _categoryService;
        private readonly IZooService _zooService;

        public ZooController(
            IAnimalService animalService,
            IEnclosureService enclosureService,
            ICategoryService categoryService,
            IZooService zooService)
        {
            _animalService = animalService;
            _enclosureService = enclosureService;
            _categoryService = categoryService;
            _zooService = zooService;
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


        [HttpPost]
        public async Task<IActionResult> AutoAssign()
        {
            await _zooService.AutoAssignAsync(false);
            return RedirectToAction(nameof(Index));
        }
    }
}
