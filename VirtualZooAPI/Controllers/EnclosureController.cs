using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VirtualZooAPI.Services.Interfaces;
using VirtualZooShared.Enums;
using VirtualZooShared.Models;

namespace VirtualZooAPI.Controllers
{
    [Route("api/enclosures")]
    [ApiController]
    public class EnclosureController : ControllerBase
    {
        private readonly IEnclosureService _enclosureService;

        public EnclosureController(IEnclosureService enclosureService)
        {
            _enclosureService = enclosureService;
        }

        /// <summary>
        /// Haalt alle verblijven op.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all enclosures", Description = "Returns a list of all enclosures in the zoo.")]
        [SwaggerResponse(200, "Successfully retrieved list of enclosures", typeof(IEnumerable<Enclosure>))]
        public async Task<ActionResult<IEnumerable<Enclosure>>> GetEnclosures()
        {
            return Ok(await _enclosureService.GetAllEnclosuresAsync());
        }

        /// <summary>
        /// Haalt een verblijf op aan de hand van het opgegeven ID.
        /// </summary>
        /// <param name="id">ID van het verblijf</param>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get an enclosure by ID", Description = "Returns a single enclosure based on the given ID.")]
        [SwaggerResponse(200, "Successfully retrieved the enclosure", typeof(Enclosure))]
        [SwaggerResponse(404, "Enclosure not found")]
        public async Task<ActionResult<Enclosure>> GetEnclosure(int id)
        {
            var enclosure = await _enclosureService.GetEnclosureByIdAsync(id);
            if (enclosure == null) return NotFound();
            return Ok(enclosure);
        }

        /// <summary>
        /// Voegt een nieuw verblijf toe.
        /// </summary>
        /// <param name="enclosure">Het verblijf dat toegevoegd moet worden</param>
        [HttpPost]
        [SwaggerOperation(Summary = "Add a new enclosure", Description = "Creates a new enclosure in the system.")]
        [SwaggerResponse(201, "Enclosure successfully created", typeof(Enclosure))]
        [SwaggerResponse(400, "Invalid enclosure data")]
        public async Task<ActionResult> AddEnclosure([FromBody] Enclosure enclosure)
        {
            if (enclosure == null) return BadRequest("Invalid enclosure data.");
            await _enclosureService.AddEnclosureAsync(enclosure);
            return CreatedAtAction(nameof(GetEnclosure), new { id = enclosure.Id }, enclosure);
        }

        /// <summary>
        /// Wijzigt een bestaand verblijf.
        /// </summary>
        /// <param name="id">ID van het verblijf</param>
        /// <param name="enclosure">Het gewijzigde verblijf</param>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an existing enclosure", Description = "Updates the data of an existing enclosure.")]
        [SwaggerResponse(204, "Enclosure successfully updated")]
        [SwaggerResponse(400, "ID mismatch or invalid data")]
        public async Task<ActionResult> UpdateEnclosure(int id, [FromBody] Enclosure enclosure)
        {
            if (id != enclosure.Id) return BadRequest("ID mismatch.");
            await _enclosureService.UpdateEnclosureAsync(enclosure);
            return NoContent();
        }

        /// <summary>
        /// Verwijdert een verblijf aan de hand van het opgegeven ID.
        /// </summary>
        /// <param name="id">ID van het verblijf</param>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete an enclosure", Description = "Deletes the specified enclosure by ID.")]
        [SwaggerResponse(204, "Enclosure successfully deleted")]
        public async Task<ActionResult> DeleteEnclosure(int id)
        {
            await _enclosureService.DeleteEnclosureAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Geeft aan welke dieren in het verblijf wakker worden of gaan slapen.
        /// </summary>
        [HttpGet("{id}/sunrise")]
        [SwaggerOperation(Summary = "Geeft aan welke dieren wakker worden of gaan slapen bij zonsopgang.")]
        [SwaggerResponse(200, "Lijst met beschrijvingen van de status van de dieren.", typeof(List<string>))]
        [SwaggerResponse(404, "Verblijf niet gevonden.")]
        public async Task<ActionResult<List<string>>> Sunrise(int id)
        {
            var enclosure = await _enclosureService.GetEnclosureByIdAsync(id);
            if (enclosure == null) return NotFound();

            var result = enclosure.Animals.Select(a => $"{a.Name}: " + a.ActivityPattern switch
            {
                ActivityPattern.Diurnal => "wordt wakker.",
                ActivityPattern.Nocturnal => "gaat slapen.",
                ActivityPattern.Cathemeral => "is actief.",
                _ => "heeft een onbekend patroon."
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Geeft aan welke dieren in het verblijf wakker worden of gaan slapen bij zonsondergang.
        /// </summary>
        [HttpGet("{id}/sunset")]
        [SwaggerOperation(Summary = "Geeft aan welke dieren wakker worden of gaan slapen bij zonsondergang.")]
        [SwaggerResponse(200, "Lijst met beschrijvingen van de status van de dieren.", typeof(List<string>))]
        [SwaggerResponse(404, "Verblijf niet gevonden.")]
        public async Task<ActionResult<List<string>>> Sunset(int id)
        {
            var enclosure = await _enclosureService.GetEnclosureByIdAsync(id);
            if (enclosure == null) return NotFound();

            var result = enclosure.Animals.Select(a => $"{a.Name}: " + a.ActivityPattern switch
            {
                ActivityPattern.Diurnal => "gaat slapen.",
                ActivityPattern.Nocturnal => "wordt wakker.",
                ActivityPattern.Cathemeral => "is actief.",
                _ => "heeft een onbekend patroon."
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Geeft aan wat alle dieren in dit verblijf eten.
        /// </summary>
        [HttpGet("{id}/feedingtime")]
        [SwaggerOperation(Summary = "Geeft aan wat alle dieren in dit verblijf eten.")]
        [SwaggerResponse(200, "Lijst met wat elk dier eet.", typeof(List<string>))]
        [SwaggerResponse(404, "Verblijf niet gevonden.")]
        public async Task<ActionResult<List<string>>> FeedingTime(int id)
        {
            var enclosure = await _enclosureService.GetEnclosureByIdAsync(id);
            if (enclosure == null) return NotFound();

            var result = enclosure.Animals.Select(a => $"{a.Name}: " +
                (a.DietaryClass == DietaryClass.Carnivore || a.DietaryClass == DietaryClass.Piscivore
                    ? "eet andere dieren."
                    : $"eet {a.Prey}.")).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Controleert of het verblijf voldoet aan de ruimte- en beveiligingseisen.
        /// </summary>
        [HttpGet("{id}/checkconstraints")]
        [SwaggerOperation(Summary = "Controleert of het verblijf voldoet aan ruimte- en beveiligingseisen.")]
        [SwaggerResponse(200, "Lijst met bevindingen of bevestiging dat alles in orde is.", typeof(List<string>))]
        [SwaggerResponse(404, "Verblijf niet gevonden.")]
        public async Task<ActionResult<List<string>>> CheckConstraints(int id)
        {
            var enclosure = await _enclosureService.GetEnclosureByIdAsync(id);
            if (enclosure == null) return NotFound();

            var issues = new List<string>();
            double totalRequired = enclosure.Animals.Sum(a => a.SpaceRequirement);
            if (totalRequired > enclosure.Size)
                issues.Add("Te weinig ruimte in het verblijf.");

            foreach (var animal in enclosure.Animals)
            {
                if (enclosure.SecurityLevel < animal.SecurityRequirement)
                    issues.Add($"{animal.Name} vereist een hoger beveiligingsniveau dan beschikbaar is.");
            }

            return Ok(issues.Count == 0 ? new List<string> { "Alle eisen zijn voldaan." } : issues);
        }
    }
}
