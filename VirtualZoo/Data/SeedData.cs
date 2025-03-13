using Microsoft.EntityFrameworkCore;
using VirtualZoo.Models;
using VirtualZoo.Enums;

namespace VirtualZoo.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (context.Animals.Any() || context.Categories.Any() || context.Enclosures.Any() || context.Zoos.Any())
            {
                return; // Database is al gevuld
            }

            // Categorieën toevoegen
            var categories = new List<Category>
            {
                new Category { Name = "Mammals" },
                new Category { Name = "Birds" },
                new Category { Name = "Reptiles" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Verblijven toevoegen
            var enclosures = new List<Enclosure>
            {
                new Enclosure { Name = "Savannah Habitat", Climate = Climate.Tropical, HabitatType = HabitatType.Forest | HabitatType.Desert, SecurityLevel = SecurityLevel.Medium, Size = 1000 },
                new Enclosure { Name = "Rainforest Habitat", Climate = Climate.Tropical, HabitatType = HabitatType.Forest | HabitatType.Aquatic, SecurityLevel = SecurityLevel.High, Size = 500 }
            };
            context.Enclosures.AddRange(enclosures);
            context.SaveChanges();

            // Dieren toevoegen
            var animals = new List<Animal>
            {
                new Animal { Name = "Lion", Species = "Panthera leo", CategoryId = categories[0].Id, EnclosureId = enclosures[0].Id, Size = Size.Large, DietaryClass = DietaryClass.Carnivore, ActivityPattern = ActivityPattern.Diurnal, Prey = "Zebra", SpaceRequirement = 50, SecurityRequirement = SecurityLevel.Medium },
                new Animal { Name = "Parrot", Species = "Psittaciformes", CategoryId = categories[1].Id, EnclosureId = enclosures[1].Id, Size = Size.Small, DietaryClass = DietaryClass.Omnivore, ActivityPattern = ActivityPattern.Diurnal, Prey = "Insects", SpaceRequirement = 2, SecurityRequirement = SecurityLevel.Low }
            };
            context.Animals.AddRange(animals);
            context.SaveChanges();
        }
    }
}
