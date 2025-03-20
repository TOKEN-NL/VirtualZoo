using Microsoft.AspNetCore.Mvc;
using VirtualZooAPI.Services.Interfaces;
using VirtualZooShared.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtualZooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IAnimalService _animalService;

        public AnimalController(IAnimalService animalService)
        {
            _animalService = animalService;
        }

        /// <summary>
        /// Haal alle dieren op uit de database.
        /// </summary>
        /// <returns>Een lijst met dieren</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Haal alle dieren op", Description = "Geeft een lijst met alle dieren in de database.")]
        [SwaggerResponse(200, "Lijst met dieren is succesvol opgehaald.", typeof(IEnumerable<Animal>))]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals()
        {
            return Ok(await _animalService.GetAllAnimalsAsync());
        }

        /// <summary>
        /// Haal een specifiek dier op aan de hand van ID.
        /// </summary>
        /// <param name="id">Het ID van het dier</param>
        /// <returns>Een dier</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Haal een dier op", Description = "Geeft een dier terug op basis van het ID.")]
        [SwaggerResponse(200, "Dier succesvol gevonden.", typeof(Animal))]
        [SwaggerResponse(404, "Geen dier gevonden met het opgegeven ID.")]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null)
                return NotFound();

            return Ok(animal);
        }

        /// <summary>
        /// Voeg een nieuw dier toe aan de database.
        /// </summary>
        /// <param name="animal">Het dier dat toegevoegd moet worden</param>
        /// <returns>Het aangemaakte dier</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Voeg een nieuw dier toe", Description = "Maakt een nieuw dier aan in de database.")]
        [SwaggerResponse(201, "Dier succesvol aangemaakt.", typeof(Animal))]
        [SwaggerResponse(400, "Ongeldige invoer.")]
        public async Task<ActionResult> AddAnimal([FromBody] Animal animal)
        {
            if (animal == null)
                return BadRequest();

            await _animalService.AddAnimalAsync(animal);
            return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animal);
        }

        /// <summary>
        /// Update een bestaand dier.
        /// </summary>
        /// <param name="id">Het ID van het dier</param>
        /// <param name="animal">Het dier met de gewijzigde gegevens</param>
        /// <returns>Geen inhoud als het updaten is gelukt</returns>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update een dier", Description = "Wijzigt een bestaand dier.")]
        [SwaggerResponse(204, "Dier succesvol geüpdatet.")]
        [SwaggerResponse(400, "ID mismatch of ongeldige invoer.")]
        public async Task<ActionResult> UpdateAnimal(int id, [FromBody] Animal animal)
        {
            if (id != animal.Id)
                return BadRequest();

            await _animalService.UpdateAnimalAsync(animal);
            return NoContent();
        }

        /// <summary>
        /// Verwijder een dier uit de database.
        /// </summary>
        /// <param name="id">Het ID van het dier dat verwijderd moet worden</param>
        /// <returns>Geen inhoud als het verwijderen is gelukt</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Verwijder een dier", Description = "Verwijdert een dier uit de database.")]
        [SwaggerResponse(204, "Dier succesvol verwijderd.")]
        [SwaggerResponse(404, "Geen dier gevonden met het opgegeven ID.")]
        public async Task<ActionResult> DeleteAnimal(int id)
        {
            await _animalService.DeleteAnimalAsync(id);
            return NoContent();
        }
    }
}
