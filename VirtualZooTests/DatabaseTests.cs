using Microsoft.EntityFrameworkCore;
using VirtualZoo.Data;
using VirtualZoo.Models;
using VirtualZoo.Enums;
using Xunit;

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

    [Fact]
    public void CannotSaveAnimalWithoutCategory()
    {
        // Arrange: Haal een omheining op
        var enclosure = _context.Enclosures.FirstOrDefault();
        Assert.NotNull(enclosure);  // Verzeker dat er een omheining is.

        // Act: Probeer een dier toe te voegen zonder categorie
        var animal = new Animal
        {
            Name = "Lion3",
            Species = "Panthera leo",
            EnclosureId = enclosure.Id,
            Size = Size.Large,
            DietaryClass = DietaryClass.Carnivore,
            ActivityPattern = ActivityPattern.Diurnal,
            Prey = "Zebra",
            SpaceRequirement = 50,
            SecurityRequirement = SecurityLevel.Medium
        };

        // Assert: Vergewis je ervan dat er een fout optreedt (bijvoorbeeld dat de categorie verplicht is)
        Assert.Throws<DbUpdateException>(() => _context.Animals.Add(animal));
    }

    [Fact]
    public void CannotSaveAnimalWithoutEnclosure()
    {
        // Arrange: Haal een categorie op
        var category = _context.Categories.FirstOrDefault();
        Assert.NotNull(category);  // Verzeker dat er een categorie is.

        // Act: Probeer een dier toe te voegen zonder omheining
        var animal = new Animal
        {
            Name = "Lion4",
            Species = "Panthera leo",
            CategoryId = category.Id,
            Size = Size.Large,
            DietaryClass = DietaryClass.Carnivore,
            ActivityPattern = ActivityPattern.Diurnal,
            Prey = "Zebra",
            SpaceRequirement = 50,
            SecurityRequirement = SecurityLevel.Medium
        };

        // Assert: Vergewis je ervan dat er een fout optreedt (bijvoorbeeld dat de omheining verplicht is)
        Assert.Throws<DbUpdateException>(() => _context.Animals.Add(animal));
    }

    [Fact]
    public void CannotSaveAnimalWithoutName()
    {
        // Arrange: Haal een categorie en een omheining op
        var category = _context.Categories.FirstOrDefault();
        var enclosure = _context.Enclosures.FirstOrDefault();
        Assert.NotNull(category);
        Assert.NotNull(enclosure);

        // Act: Probeer een dier toe te voegen zonder naam
        var animal = new Animal
        {
            Species = "Panthera leo",
            CategoryId = category.Id,
            EnclosureId = enclosure.Id,
            Size = Size.Large,
            DietaryClass = DietaryClass.Carnivore,
            ActivityPattern = ActivityPattern.Diurnal,
            Prey = "Zebra",
            SpaceRequirement = 50,
            SecurityRequirement = SecurityLevel.Medium
        };

        // Assert: Vergewis je ervan dat er een fout optreedt
        Assert.Throws<DbUpdateException>(() => _context.Animals.Add(animal));
    }

    [Fact]
    public void CannotSaveAnimalWithInvalidSpaceRequirement()
    {
        // Arrange: Haal een categorie en een omheining op
        var category = _context.Categories.FirstOrDefault();
        var enclosure = _context.Enclosures.FirstOrDefault();
        Assert.NotNull(category);
        Assert.NotNull(enclosure);

        // Act: Probeer een dier toe te voegen met een ongeldige ruimtevereiste
        var animal = new Animal
        {
            Name = "Lion5",
            Species = "Panthera leo",
            CategoryId = category.Id,
            EnclosureId = enclosure.Id,
            Size = Size.Large,
            DietaryClass = DietaryClass.Carnivore,
            ActivityPattern = ActivityPattern.Diurnal,
            Prey = "Zebra",
            SpaceRequirement = -1, // Ongeldige waarde
            SecurityRequirement = SecurityLevel.Medium
        };

        // Assert: Vergewis je ervan dat er een fout optreedt
        Assert.Throws<DbUpdateException>(() => _context.Animals.Add(animal));
    }



}

