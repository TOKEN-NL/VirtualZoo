using Bogus;
using VirtualZooShared.Enums;
using VirtualZooShared.Models;
using VirtualZooAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace VirtualZooAPI.Factories
{
    public static class AnimalFactory
    {
        public static Animal CreateAnimal(ApplicationDbContext? dbContext = null)
        {
            // Haal bestaande CategoryIds en EnclosureIds op
            var categoryIds = dbContext != null
                ? dbContext.Categories.Select(c => c.Id).ToList()
                : GetDefaultCategoryIds();

            var enclosureIds = dbContext != null
                ? dbContext.Enclosures.Select(e => e.Id).ToList()
                : GetDefaultEnclosureIds();

            var faker = new Faker<Animal>()
                .RuleFor(a => a.Name, f => f.Name.FirstName())
                .RuleFor(a => a.Species, f => f.Lorem.Word() + "us")
                .RuleFor(a => a.Size, f => f.PickRandom<Size>())
                .RuleFor(a => a.DietaryClass, f => f.PickRandom<DietaryClass>())
                .RuleFor(a => a.ActivityPattern, f => f.PickRandom<ActivityPattern>())
                .RuleFor(a => a.SpaceRequirement, f => Math.Round(f.Random.Double(5, 50), 2))
                .RuleFor(a => a.SecurityRequirement, f => f.PickRandom<SecurityLevel>())
                .RuleFor(a => a.Prey, f => f.Lorem.Word())
                .RuleFor(a => a.CategoryId, f => f.PickRandom(categoryIds))
                .RuleFor(a => a.EnclosureId, f => f.PickRandom(enclosureIds));

            return faker.Generate();
        }

        public static List<Animal> CreateAnimals(int count, ApplicationDbContext? dbContext = null)
        {
            return Enumerable.Range(1, count).Select(_ => CreateAnimal(dbContext)).ToList();
        }

        /// <summary>
        /// Haalt standaard ID's op uit de database als er geen context is.
        /// </summary>
        private static List<int> GetDefaultCategoryIds()
        {
            using var defaultContext = CreateDefaultDbContext();
            return defaultContext.Categories.Select(c => c.Id).ToList();
        }

        private static List<int> GetDefaultEnclosureIds()
        {
            using var defaultContext = CreateDefaultDbContext();
            return defaultContext.Enclosures.Select(e => e.Id).ToList();
        }

        /// <summary>
        /// Maakt een standaard databasecontext aan als er geen wordt meegegeven.
        /// </summary>
        private static ApplicationDbContext CreateDefaultDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=VirtueleDierentuin11;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
