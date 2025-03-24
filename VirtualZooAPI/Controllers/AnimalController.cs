using Microsoft.AspNetCore.Mvc;
using VirtualZooAPI.Services.Interfaces;
using VirtualZooShared.Models;
using VirtualZooShared.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using VirtualZooAPI.Services.Implementations;

namespace VirtualZooAPI.Controllers
{

    [Route("api/animals")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IAnimalService _animalService;
        private readonly IEnclosureService _enclosureService;

        public AnimalController(IAnimalService animalService, IEnclosureService enclosureService)
        {
            _animalService = animalService;
            _enclosureService = enclosureService;
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

        /// <summary>
        /// Geeft aan of het dier wakker is tijdens zonsopgang.
        /// </summary>
        [HttpGet("{id}/sunrise")]
        [SwaggerOperation(Summary = "Geeft aan of het dier wakker wordt bij zonsopgang.")]
        [SwaggerResponse(200, "Statusbericht over het dier tijdens zonsopgang.", typeof(string))]
        [SwaggerResponse(404, "Dier niet gevonden.")]
        public async Task<ActionResult<string>> Sunrise(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null) return NotFound();

            return Ok(animal.ActivityPattern switch
            {
                ActivityPattern.Diurnal => $"{animal.Name} wordt wakker.",
                ActivityPattern.Nocturnal => $"{animal.Name} gaat slapen.",
                ActivityPattern.Cathemeral => $"{animal.Name} is actief.",
                _ => $"Activiteitspatroon van {animal.Name} is onbekend."
            });
        }

        /// <summary>
        /// Geeft aan of het dier wakker is tijdens zonsondergang.
        /// </summary>
        [HttpGet("{id}/sunset")]
        [SwaggerOperation(Summary = "Geeft aan of het dier wakker wordt bij zonsondergang.")]
        [SwaggerResponse(200, "Statusbericht over het dier tijdens zonsondergang.", typeof(string))]
        [SwaggerResponse(404, "Dier niet gevonden.")]
        public async Task<ActionResult<string>> Sunset(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null) return NotFound();

            return Ok(animal.ActivityPattern switch
            {
                ActivityPattern.Diurnal => $"{animal.Name} gaat slapen.",
                ActivityPattern.Nocturnal => $"{animal.Name} wordt wakker.",
                ActivityPattern.Cathemeral => $"{animal.Name} is actief.",
                _ => $"Activiteitspatroon van {animal.Name} is onbekend."
            });
        }

        /// <summary>
        /// Geeft aan wat het dier eet.
        /// </summary>
        [HttpGet("{id}/feedingtime")]
        [SwaggerOperation(Summary = "Geeft aan wat het dier eet.")]
        [SwaggerResponse(200, "Beschrijving van het dieet van het dier.", typeof(string))]
        [SwaggerResponse(404, "Dier niet gevonden.")]
        public async Task<ActionResult<string>> FeedingTime(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null) return NotFound();

            var eatsOtherAnimals = animal.DietaryClass == DietaryClass.Carnivore ||
                                   animal.DietaryClass == DietaryClass.Piscivore;

            return Ok(eatsOtherAnimals
                ? $"{animal.Name} eet andere dieren."
                : $"{animal.Name} eet {animal.Prey}.");
        }

        /// <summary>
        /// Controleert of het verblijf van het dier groot genoeg en veilig genoeg is.
        /// </summary>
        [HttpGet("{id}/checkconstraints")]
        [SwaggerOperation(Summary = "Controleert of het dier in een geschikt verblijf zit.")]
        [SwaggerResponse(200, "Lijst met constraint-issues of bevestiging dat alles in orde is.", typeof(List<string>))]
        [SwaggerResponse(404, "Dier niet gevonden.")]
        public async Task<ActionResult<List<string>>> CheckConstraints(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null) return NotFound();

            var issues = new List<string>();

            if (animal.EnclosureId == null)
            {
                issues.Add($"{animal.Name} is aan geen verblijf gekoppeld.");
            }
            else
            {
                // Haal de enclosure op via de EnclosureService
                var enclosure = await _enclosureService.GetEnclosureByIdAsync(animal.EnclosureId);
                if (enclosure == null)
                {
                    issues.Add($"Verblijf met ID {animal.EnclosureId} niet gevonden.");
                }
                else
                {
                    double totalRequired = enclosure.Animals.Sum(a => a.SpaceRequirement);
                    if (totalRequired > enclosure.Size)
                        issues.Add("Te weinig ruimte in het verblijf.");

                    if (enclosure.SecurityLevel < animal.SecurityRequirement)
                        issues.Add("Beveiligingsniveau van verblijf is onvoldoende.");
                }
            }

            return Ok(issues.Count == 0 ? new List<string> { "Alle eisen zijn voldaan." } : issues);
        }

    }
}
