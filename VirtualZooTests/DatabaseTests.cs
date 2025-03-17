using Microsoft.EntityFrameworkCore;
using VirtualZooAPI.Data;
using VirtualZooShared.Models;
using VirtualZooShared.Enums;
using Xunit;

namespace VirtualZooTests
{

    public class DatabaseTests : IDisposable
    {
        private readonly ApplicationDbContext _context;

        public DatabaseTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=.;Database=VirtualZooTest;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            _context = new ApplicationDbContext(options);

            // Database opschonen voor elke test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            SeedData.Initialize(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void CanConnectToDatabase()
        {
            bool canConnect = _context.Database.CanConnect();
            Assert.True(canConnect);
        }

        [Fact]
        public void DatabaseHasSeededData()
        {
            var animalCount = _context.Animals.Count();
            Assert.True(animalCount > 0);
        }

        [Fact]
        public void CanSaveAndRetrieveAnimal()
        {
            var category = _context.Categories.FirstOrDefault();
            var enclosure = _context.Enclosures.FirstOrDefault();
            var animal = new Animal { Name = "Lion2", Species = "Panthera leo", CategoryId = category.Id, EnclosureId = enclosure.Id, Size = Size.Large, DietaryClass = DietaryClass.Carnivore, ActivityPattern = ActivityPattern.Diurnal, Prey = "Zebra", SpaceRequirement = 50, SecurityRequirement = SecurityLevel.Medium };

            _context.Animals.Add(animal);
            _context.SaveChanges();

            var retrievedAnimal = _context.Animals.FirstOrDefault(a => a.Name == "Lion2");
            Assert.NotNull(retrievedAnimal);
            Assert.Equal(Size.Large, retrievedAnimal.Size);
        }
    }
}
