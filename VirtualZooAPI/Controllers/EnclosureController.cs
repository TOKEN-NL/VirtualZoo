using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VirtualZooAPI.Services.Interfaces;
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
    }
}
