using Microsoft.AspNetCore.Mvc;
using VirtualZooAPI.Services.Interfaces;
using VirtualZooShared.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtualZooAPI.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Haal alle categorieën op.
        /// </summary>
        /// <returns>Een lijst met categorieën</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Haal alle categorieën op", Description = "Geeft een lijst met alle categorieën in de database.")]
        [SwaggerResponse(200, "Lijst met categorieën succesvol opgehaald.", typeof(IEnumerable<Category>))]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return Ok(await _categoryService.GetAllCategoriesAsync());
        }

        /// <summary>
        /// Haal een specifieke categorie op via ID.
        /// </summary>
        /// <param name="id">Het ID van de categorie</param>
        /// <returns>De categorie</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Haal een categorie op", Description = "Geeft een categorie terug op basis van het ID.")]
        [SwaggerResponse(200, "Categorie succesvol gevonden.", typeof(Category))]
        [SwaggerResponse(404, "Geen categorie gevonden met het opgegeven ID.")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(new
            {
                category.Id,
                category.Name,
                Animals = category.Animals ?? new List<Animal>() // Zorgt voor een lege lijst
            });
        }

        /// <summary>
        /// Voeg een nieuwe categorie toe.
        /// </summary>
        /// <param name="category">De categorie die moet worden toegevoegd</param>
        /// <returns>De aangemaakte categorie</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Voeg een nieuwe categorie toe", Description = "Maakt een nieuwe categorie aan in de database.")]
        [SwaggerResponse(201, "Categorie succesvol aangemaakt.", typeof(Category))]
        [SwaggerResponse(400, "Ongeldige invoer.")]
        public async Task<ActionResult> AddCategory([FromBody] Category category)
        {
            if (category == null) return BadRequest("Invalid category data.");
            category.Animals ??= new List<Animal>();
            await _categoryService.AddCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        /// <summary>
        /// Update een bestaande categorie.
        /// </summary>
        /// <param name="id">Het ID van de categorie</param>
        /// <param name="category">De categorie met de gewijzigde gegevens</param>
        /// <returns>Geen inhoud als het updaten is gelukt</returns>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update een categorie", Description = "Wijzigt een bestaande categorie.")]
        [SwaggerResponse(204, "Categorie succesvol geüpdatet.")]
        [SwaggerResponse(400, "ID mismatch of ongeldige invoer.")]
        public async Task<ActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.Id) return BadRequest("ID mismatch.");
            await _categoryService.UpdateCategoryAsync(category);
            return NoContent();
        }

        /// <summary>
        /// Verwijder een categorie uit de database.
        /// </summary>
        /// <param name="id">Het ID van de categorie die moet worden verwijderd</param>
        /// <returns>Geen inhoud als het verwijderen is gelukt</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Verwijder een categorie", Description = "Verwijdert een categorie uit de database.")]
        [SwaggerResponse(204, "Categorie succesvol verwijderd.")]
        [SwaggerResponse(404, "Geen categorie gevonden met het opgegeven ID.")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
