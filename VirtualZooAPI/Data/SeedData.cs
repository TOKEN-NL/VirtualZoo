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
            if (!context.Categories.Any())
            {
                var categoryList = new List<Category>
                {
                    new Category { Name = "Mammals" },
                    new Category { Name = "Birds" },
                    new Category { Name = "Reptiles" }
                };

                context.Categories.AddRange(categoryList);
                context.SaveChanges();
            }

            // Enclosures toevoegen
            if (!context.Enclosures.Any())
            {
                var enclosureList = new List<Enclosure>
                {
                    new Enclosure { Name = "Savannah Habitat", Climate = Climate.Tropical, HabitatType = HabitatType.Forest | HabitatType.Desert, SecurityLevel = SecurityLevel.Medium, Size = 1000, ZooId = 1 },
                    new Enclosure { Name = "Rainforest Habitat", Climate = Climate.Tropical, HabitatType = HabitatType.Forest | HabitatType.Aquatic, SecurityLevel = SecurityLevel.High, Size = 500, ZooId = 1 }
                };

                context.Enclosures.AddRange(enclosureList);
                context.SaveChanges();
            }

            // Dieren toevoegen
            if (!context.Animals.Any())
            {
                var categories = context.Categories.ToList();
                var enclosures = context.Enclosures.ToList();

                var animals = new List<Animal>
                {
                    new Animal
                    {
                        Name = "Lion1",
                        Species = "Panthera leo",
                        CategoryId = categories[0].Id,
                        EnclosureId = enclosures[0].Id,
                        Size = Size.Large,
                        DietaryClass = DietaryClass.Carnivore,
                        ActivityPattern = ActivityPattern.Diurnal,
                        Prey = "Zebra",
                        SpaceRequirement = 50,
                        SecurityRequirement = SecurityLevel.Medium
                    },
                    new Animal
                    {
                        Name = "Parrot1",
                        Species = "Psittaciformes",
                        CategoryId = categories[1].Id,
                        EnclosureId = enclosures[1].Id,
                        Size = Size.Small,
                        DietaryClass = DietaryClass.Omnivore,
                        ActivityPattern = ActivityPattern.Diurnal,
                        Prey = "Insects",
                        SpaceRequirement = 2,
                        SecurityRequirement = SecurityLevel.Low
                    },
                    new Animal
                    {
                        Name = "Elephant",
                        Species = "Loxodonta africana",
                        CategoryId = categories[0].Id,
                        EnclosureId = enclosures[0].Id,
                        Size = Size.Large,
                        DietaryClass = DietaryClass.Herbivore,
                        ActivityPattern = ActivityPattern.Diurnal,
                        Prey = "",
                        SpaceRequirement = 100,
                        SecurityRequirement = SecurityLevel.Medium
                    },
                    new Animal
                    {
                        Name = "Giraffe",
                        Species = "Giraffa camelopardalis",
                        CategoryId = categories[0].Id,
                        EnclosureId = enclosures[0].Id,
                        Size = Size.Large,
                        DietaryClass = DietaryClass.Herbivore,
                        ActivityPattern = ActivityPattern.Diurnal,
                        Prey = "",
                        SpaceRequirement = 80,
                        SecurityRequirement = SecurityLevel.Low
                    },
                    new Animal
                    {
                        Name = "Zebra",
                        Species = "Equus quagga",
                        CategoryId = categories[0].Id,
                        EnclosureId = enclosures[0].Id,
                        Size = Size.Medium,
                        DietaryClass = DietaryClass.Herbivore,
                        ActivityPattern = ActivityPattern.Diurnal,
                        Prey = "",
                        SpaceRequirement = 40,
                        SecurityRequirement = SecurityLevel.Low
                    },
                    new Animal
                    {
                        Name = "Chimpanzee",
                        Species = "Pan troglodytes",
                        CategoryId = categories[1].Id,
                        EnclosureId = enclosures[1].Id,
                        Size = Size.Medium,
                        DietaryClass = DietaryClass.Omnivore,
                        ActivityPattern = ActivityPattern.Diurnal,
                        Prey = "Fruits, Insects",
                        SpaceRequirement = 30,
                        SecurityRequirement = SecurityLevel.Medium
                    },
                    new Animal
                    {
                        Name = "Python",
                        Species = "Python regius",
                        CategoryId = categories[1].Id,
                        EnclosureId = enclosures[1].Id,
                        Size = Size.Medium,
                        DietaryClass = DietaryClass.Carnivore,
                        ActivityPattern = ActivityPattern.Nocturnal,
                        Prey = "Rodents",
                        SpaceRequirement = 15,
                        SecurityRequirement = SecurityLevel.Medium
                    },
                    new Animal
                    {
                        Name = "Toucan",
                        Species = "Ramphastos toco",
                        CategoryId = categories[1].Id,
                        EnclosureId = enclosures[1].Id,
                        Size = Size.Small,
                        DietaryClass = DietaryClass.Omnivore,
                        ActivityPattern = ActivityPattern.Diurnal,
                        Prey = "Fruit, insects",
                        SpaceRequirement = 3,
                        SecurityRequirement = SecurityLevel.Low
                    },
                    new Animal
                    {
                        Name = "Meerkat",
                        Species = "Suricata suricatta",
                        CategoryId = categories[0].Id,
                        EnclosureId = enclosures[0].Id,
                        Size = Size.Small,
                        DietaryClass = DietaryClass.Omnivore,
                        ActivityPattern = ActivityPattern.Diurnal,
                        Prey = "Insects",
                        SpaceRequirement = 5,
                        SecurityRequirement = SecurityLevel.Low
                    },
                    new Animal
                    {
                        Name = "Crocodile",
                        Species = "Crocodylus niloticus",
                        CategoryId = categories[1].Id,
                        EnclosureId = enclosures[1].Id,
                        Size = Size.Large,
                        DietaryClass = DietaryClass.Carnivore,
                        ActivityPattern = ActivityPattern.Nocturnal,
                        Prey = "Fish, mammals",
                        SpaceRequirement = 60,
                        SecurityRequirement = SecurityLevel.High
                    },
                    new Animal
                    {
                        Name = "Flamingo",
                        Species = "Phoenicopterus roseus",
                        CategoryId = categories[1].Id,
                        EnclosureId = enclosures[1].Id,
                        Size = Size.Medium,
                        DietaryClass = DietaryClass.Omnivore,
                        ActivityPattern = ActivityPattern.Diurnal,
                        Prey = "Shrimp, algae",
                        SpaceRequirement = 10,
                        SecurityRequirement = SecurityLevel.Low
                    },
                    new Animal
                    {
                        Name = "Rhino",
                        Species = "Ceratotherium simum",
                        CategoryId = categories[0].Id,
                        EnclosureId = enclosures[0].Id,
                        Size = Size.Large,
                        DietaryClass = DietaryClass.Herbivore,
                        ActivityPattern = ActivityPattern.Nocturnal,
                        Prey = "",
                        SpaceRequirement = 90,
                        SecurityRequirement = SecurityLevel.High
                    },
                    new Animal
                    {
                        Name = "Jaguar",
                        Species = "Panthera onca",
                        CategoryId = categories[1].Id,
                        EnclosureId = enclosures[1].Id,
                        Size = Size.Medium,
                        DietaryClass = DietaryClass.Carnivore,
                        ActivityPattern = ActivityPattern.Nocturnal,
                        Prey = "Deer, rodents",
                        SpaceRequirement = 40,
                        SecurityRequirement = SecurityLevel.High
                    },
                    new Animal
                    {
                        Name = "Koala",
                        Species = "Phascolarctos cinereus",
                        CategoryId = categories[1].Id,
                        EnclosureId = enclosures[1].Id,
                        Size = Size.Small,
                        DietaryClass = DietaryClass.Herbivore,
                        ActivityPattern = ActivityPattern.Nocturnal,
                        Prey = "",
                        SpaceRequirement = 10,
                        SecurityRequirement = SecurityLevel.Medium
                    }
                };


                context.Animals.AddRange(animals);
                context.SaveChanges();
            }
        }
    }
}
