using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtualZooShared.Models;
using VirtualZooAPI.Services.Interfaces;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals()
        {
            return Ok(await _animalService.GetAllAnimalsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null)
                return NotFound();

            return Ok(animal);
        }

        [HttpPost]
        public async Task<ActionResult> AddAnimal([FromBody] Animal animal)
        {
            if (animal == null)
                return BadRequest();

            await _animalService.AddAnimalAsync(animal);
            return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animal);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAnimal(int id, [FromBody] Animal animal)
        {
            if (id != animal.Id)
                return BadRequest();

            await _animalService.UpdateAnimalAsync(animal);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAnimal(int id)
        {
            await _animalService.DeleteAnimalAsync(id);
            return NoContent();
        }
    }
}
