using VirtualZooAPI.Services.Interfaces;
using VirtualZooAPI.Repositories.Interfaces;
using VirtualZooShared.Models;
using VirtualZooShared.Enums;
using VirtualZooAPI.Factories;

namespace VirtualZooAPI.Services.Implementations
{
    public class ZooService : IZooService
    {
        private readonly IAnimalService _animalService;
        private readonly IEnclosureService _enclosureService;

        public ZooService(IAnimalService animalService, IEnclosureService enclosureService)
        {
            _animalService = animalService;
            _enclosureService = enclosureService;
        }

        public async Task<List<string>> AutoAssignAsync(bool resetExisting)
        {
            var feedback = new List<string>();
            var allAnimals = await _animalService.GetAllAnimalsAsync();
            var remainingAnimals = new List<Animal>();

            if (resetExisting)
            {
                // Ontkoppel eerst alle dieren
                foreach (var animal in allAnimals)
                {
                    animal.EnclosureId = null;
                    await _animalService.UpdateAnimalAsync(animal);
                }

                // Verwijder alle verblijven
                var enclosures = await _enclosureService.GetAllEnclosuresAsync();
                foreach (var enc in enclosures)
                {
                    await _enclosureService.DeleteEnclosureAsync(enc.Id);
                }

                feedback.Add("Alle dieren zijn ontkoppeld en alle verblijven zijn verwijderd.");
                remainingAnimals = allAnimals.ToList();
            }
            else
            {
                // Alleen dieren zonder geldig verblijf of die niet voldoen aan de eisen
                foreach (var animal in allAnimals)
                {
                    if (animal.EnclosureId == null)
                    {
                        remainingAnimals.Add(animal);
                        continue;
                    }

                    var enclosure = await _enclosureService.GetEnclosureByIdAsync(animal.EnclosureId.Value);
                    if (enclosure == null)
                    {
                        remainingAnimals.Add(animal);
                        continue;
                    }

                    double ruimteOver = enclosure.Size - enclosure.Animals.Sum(x => x.SpaceRequirement);
                    bool ruimteNietGenoeg = ruimteOver < animal.SpaceRequirement;
                    bool securityTeLaag = enclosure.SecurityLevel < animal.SecurityRequirement;

                    if (ruimteNietGenoeg || securityTeLaag)
                    {
                        // Ontkoppel dier en markeer als needing reassignment
                        animal.EnclosureId = null;
                        await _animalService.UpdateAnimalAsync(animal);
                        remainingAnimals.Add(animal);
                    }
                }
            }

            var huidigeVerblijven = await _enclosureService.GetAllEnclosuresAsync();

            // Automatisch dieren toewijzen
            foreach (var animal in remainingAnimals)
            {
                var passendVerblijf = huidigeVerblijven.FirstOrDefault(e =>
                    e.SecurityLevel >= animal.SecurityRequirement &&
                    e.Size - e.Animals.Sum(a => a.SpaceRequirement) >= animal.SpaceRequirement);

                if (passendVerblijf == null)
                {
                    var newEnclosure = EnclosureFactory.CreateEnclosure();
                    newEnclosure.Name = $"AutoVerblijf {animal.Name}";
                    newEnclosure.Size = animal.SpaceRequirement * 2;
                    newEnclosure.SecurityLevel = animal.SecurityRequirement;
                    newEnclosure.ZooId = 1;

                    var created = await _enclosureService.AddEnclosureAsync(newEnclosure);
                    animal.EnclosureId = created.Id;
                    feedback.Add($"{animal.Name} is gekoppeld aan nieuw verblijf {created.Name}.");

                }
                else
                {
                    animal.EnclosureId = passendVerblijf.Id;
                    feedback.Add($"{animal.Name} is gekoppeld aan bestaand verblijf {passendVerblijf.Name}.");
                }

                await _animalService.UpdateAnimalAsync(animal);
            }

            // Als alle animals een verblijf hebben dat voldoet aan de eisen, is er geen herverdeling nodig
            if (!feedback.Any())
            {
                feedback.Add("Alle dieren zitten al in een geschikt verblijf. Er was geen herverdeling nodig.");
            }

            return feedback;
        }



    }
}
