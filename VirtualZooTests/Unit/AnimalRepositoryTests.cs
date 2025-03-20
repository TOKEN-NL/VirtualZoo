using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualZooAPI.Data;
using VirtualZooAPI.Repositories.Implementations;
using VirtualZooShared.Models;
using VirtualZooShared.Factories;
using Xunit;

namespace VirtualZooTests.Unit
{
    public class AnimalRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=VirtueleDierentuin11Test;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            var databaseContext = new ApplicationDbContext(options);
            await databaseContext.Database.EnsureCreatedAsync();
            SeedData.Initialize(databaseContext);
            if (!await databaseContext.Animals.AnyAsync())
            {
                var animals = AnimalFactory.CreateAnimals(2);
                databaseContext.Animals.AddRange(animals);
                await databaseContext.SaveChangesAsync();
            }

            return databaseContext;
        }

        [Fact]
        public async Task GetAllAnimalsAsync_ShouldReturnAnimals()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new AnimalRepository(dbContext); 
            var existingAnimalCount = await dbContext.Animals.CountAsync();

            var testAnimals = AnimalFactory.CreateAnimals(5);
            dbContext.Animals.AddRange(testAnimals);
            await dbContext.SaveChangesAsync();

            var result = await repository.GetAllAnimalsAsync();

            Assert.NotNull(result);
            Assert.Equal(existingAnimalCount + testAnimals.Count, result.Count()); 
        }

        [Fact]
        public async Task GetAnimalByIdAsync_ShouldReturnCorrectAnimal()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new AnimalRepository(dbContext);

            var testAnimal = AnimalFactory.CreateAnimal(); 
            dbContext.Animals.Add(testAnimal);
            await dbContext.SaveChangesAsync();

            var result = await repository.GetAnimalByIdAsync(testAnimal.Id);

            Assert.NotNull(result);
            Assert.Equal(testAnimal.Name, result.Name);
            Assert.Equal(testAnimal.Species, result.Species);
        }

        [Fact]
        public async Task AddAnimalAsync_ShouldAddAnimal()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new AnimalRepository(dbContext);
            var newAnimal = AnimalFactory.CreateAnimal();

            await repository.AddAnimalAsync(newAnimal);
            var result = await dbContext.Animals.FirstOrDefaultAsync(a => a.Name == newAnimal.Name);

            Assert.NotNull(result);
            Assert.Equal(newAnimal.Name, result.Name);
        }

        [Fact]
        public async Task UpdateAnimalAsync_ShouldModifyAnimal()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new AnimalRepository(dbContext);
            var animal = await dbContext.Animals.FirstAsync();
            animal.Name = "Updated Name";

            await repository.UpdateAnimalAsync(animal);
            var updatedAnimal = await dbContext.Animals.FindAsync(animal.Id);

            Assert.Equal("Updated Name", updatedAnimal.Name);
        }

        [Fact]
        public async Task DeleteAnimalAsync_ShouldRemoveAnimal()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new AnimalRepository(dbContext);

            var animal = await dbContext.Animals.FirstAsync();
            await repository.DeleteAnimalAsync(animal.Id);
            var result = await dbContext.Animals.FindAsync(animal.Id);

            Assert.Null(result);
        }
    }
}
