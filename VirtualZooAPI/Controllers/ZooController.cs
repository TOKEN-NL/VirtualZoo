using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using VirtualZooAPI.Services.Implementations;
using VirtualZooAPI.Services.Interfaces;
using VirtualZooShared.Enums;

namespace VirtualZooAPI.Controllers
{
    [ApiController]
    [Route("api/zoo")]
    public class ZooController : ControllerBase
    {
        private readonly IAnimalService _animalService;
        private readonly IEnclosureService _enclosureService;
        private readonly IZooService _zooService;

        public ZooController(IAnimalService animalService, IEnclosureService enclosureService, IZooService zooService)
        {
            _animalService = animalService;
            _enclosureService = enclosureService;
            _zooService = zooService;
        }

        /// <summary>
        /// Geeft aan welke dieren wakker worden of gaan slapen tijdens zonsopgang.
        /// </summary>
        [HttpGet("sunrise")]
        [SwaggerOperation(Summary = "Toont het gedrag van alle dieren bij zonsopgang.")]
        [SwaggerResponse(200, "Lijst met reacties van dieren.", typeof(List<string>))]
        public async Task<ActionResult<List<string>>> Sunrise()
        {
            var animals = await _animalService.GetAllAnimalsAsync();

            var responses = animals.Select(animal =>
                animal.ActivityPattern switch
                {
                    ActivityPattern.Diurnal => $"{animal.Name} wordt wakker.",
                    ActivityPattern.Nocturnal => $"{animal.Name} gaat slapen.",
                    ActivityPattern.Cathemeral => $"{animal.Name} is actief.",
                    _ => $"{animal.Name}: onbekend patroon."
                }).ToList();

            return Ok(responses);
        }

        /// <summary>
        /// Geeft aan welke dieren wakker worden of gaan slapen tijdens zonsondergang.
        /// </summary>
        [HttpGet("sunset")]
        [SwaggerOperation(Summary = "Toont het gedrag van alle dieren bij zonsondergang.")]
        [SwaggerResponse(200, "Lijst met reacties van dieren.", typeof(List<string>))]
        public async Task<ActionResult<List<string>>> Sunset()
        {
            var animals = await _animalService.GetAllAnimalsAsync();

            var responses = animals.Select(animal =>
                animal.ActivityPattern switch
                {
                    ActivityPattern.Diurnal => $"{animal.Name} gaat slapen.",
                    ActivityPattern.Nocturnal => $"{animal.Name} wordt wakker.",
                    ActivityPattern.Cathemeral => $"{animal.Name} is actief.",
                    _ => $"{animal.Name}: onbekend patroon."
                }).ToList();

            return Ok(responses);
        }

        /// <summary>
        /// Geeft aan wat alle dieren eten.
        /// </summary>
        [HttpGet("feedingtime")]
        [SwaggerOperation(Summary = "Toont het dieet van alle dieren.")]
        [SwaggerResponse(200, "Lijst met voedselsituaties per dier.", typeof(List<string>))]
        public async Task<ActionResult<List<string>>> FeedingTime()
        {
            var animals = await _animalService.GetAllAnimalsAsync();

            var responses = animals.Select(animal =>
            {
                // Als dier andere dieren eet (Carnivore of Piscivore), dan gaat dat boven alles
                if (animal.DietaryClass == DietaryClass.Carnivore || animal.DietaryClass == DietaryClass.Piscivore)
                {
                    return $"{animal.Name} eet andere dieren ({animal.Prey}).";
                }

                // Voor alle andere types: toon wat hij eet (bijv. planten, insecten, etc.)
                return $"{animal.Name} eet {animal.Prey}.";
            }).ToList();

            return Ok(responses);
        }


        /// <summary>
        /// Controleert of alle verblijven voldoen aan de eisen van hun dieren.
        /// </summary>
        [HttpGet("checkconstraints")]
        [SwaggerOperation(Summary = "Controleert of alle verblijven voldoen aan de eisen van hun dieren.")]
        [SwaggerResponse(200, "Lijst met eventuele problemen per verblijf.", typeof(List<string>))]
        public async Task<ActionResult<List<string>>> CheckConstraints()
        {
            var enclosures = await _enclosureService.GetAllEnclosuresAsync();
            var feedback = new List<string>();

            foreach (var enclosure in enclosures)
            {
                if (enclosure.Animals == null || enclosure.Animals.Count == 0)
                {
                    feedback.Add($"{enclosure.Name} bevat geen dieren.");
                    continue;
                }

                double totalRequired = enclosure.Animals.Sum(a => a.SpaceRequirement);
                if (totalRequired > enclosure.Size)
                {
                    feedback.Add($"{enclosure.Name}: onvoldoende ruimte (benodigd: {totalRequired:F2} m², beschikbaar: {enclosure.Size:F2} m²).");
                }

                var securityIssue = enclosure.Animals
                    .Any(a => enclosure.SecurityLevel < a.SecurityRequirement);

                if (securityIssue)
                {
                    feedback.Add($"{enclosure.Name}: beveiligingsniveau onvoldoende voor minstens één dier.");
                }
            }

            return Ok(feedback.Count == 0
                ? new List<string> { "Alle verblijven voldoen aan de eisen." }
                : feedback);
        }

        /// <summary>
        /// Wijs dieren automatisch toe aan verblijven, met optie om bestaande indeling te resetten.
        /// </summary>
        /// <param name="resetExisting">Als true: verwijder alle verblijven en maak nieuwe. Als false: vul bestaande verblijven aan en verplaats waar nodig.</param>
        /// <returns>Lijst met feedback per dier.</returns>
        [HttpPost("autoassign")]
        [SwaggerOperation(Summary = "Wijs dieren automatisch toe aan verblijven.")]
        [SwaggerResponse(200, "Lijst met toewijzingsfeedback per dier.", typeof(List<string>))]
        public async Task<ActionResult<List<string>>> AutoAssign([FromQuery] bool resetExisting = false)
        {
            var result = await _zooService.AutoAssignAsync(resetExisting);
            return Ok(result);
        }
    }
}
