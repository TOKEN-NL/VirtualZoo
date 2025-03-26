using Microsoft.EntityFrameworkCore;
using VirtualZooShared.Models;
using VirtualZooShared.Enums;

namespace VirtualZooAPI.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Zoo toevoegen
            if (!context.Zoos.Any(z => z.Id == 1))
            {
                // Voeg een default zoo toe met ID 1 geforceerd. Dit is nodig om de standaard relaties te leggen.
                context.Database.ExecuteSqlRaw(@"
                    SET IDENTITY_INSERT Zoos ON;
                    INSERT INTO Zoos (Id, Name) VALUES (1, 'Default Zoo');
                    SET IDENTITY_INSERT Zoos OFF;
                ");

            }

            // Categorieën toevoegen
            var categories = context.Categories.ToList();
            if (!context.Categories.Any())
            {

                categories = new List<Category>
                {
                    new Category { Name = "Mammals" },
                    new Category { Name = "Birds" },
                    new Category { Name = "Reptiles" }
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // Verblijven toevoegen
            var enclosures = context.Enclosures.ToList();
            if (!context.Enclosures.Any())
            {
                enclosures = new List<Enclosure>
                {
                    new Enclosure { Name = "Savannah Habitat", Climate = Climate.Tropical, HabitatType = HabitatType.Forest | HabitatType.Desert, SecurityLevel = SecurityLevel.Medium, Size = 1000, ZooId = 1 },
                    new Enclosure { Name = "Rainforest Habitat", Climate = Climate.Tropical, HabitatType = HabitatType.Forest | HabitatType.Aquatic, SecurityLevel = SecurityLevel.High, Size = 500, ZooId = 1 }
                };
                context.Enclosures.AddRange(enclosures);
                context.SaveChanges();
            }

            // Dieren toevoegen
            if (!context.Animals.Any())
            {
                var animals = new List<Animal>
                {
                    new Animal { Name = "Lion1", Species = "Panthera leo", CategoryId = categories[0].Id, EnclosureId = enclosures[0].Id, Size = Size.Large, DietaryClass = DietaryClass.Carnivore, ActivityPattern = ActivityPattern.Diurnal, Prey = "Zebra", SpaceRequirement = 50, SecurityRequirement = SecurityLevel.Medium },
                    new Animal { Name = "Parrot1", Species = "Psittaciformes", CategoryId = categories[1].Id, EnclosureId = enclosures[1].Id, Size = Size.Small, DietaryClass = DietaryClass.Omnivore, ActivityPattern = ActivityPattern.Diurnal, Prey = "Insects", SpaceRequirement = 2, SecurityRequirement = SecurityLevel.Low }
                };
                context.Animals.AddRange(animals);
                context.SaveChanges();
            }
        }
    }
}
