using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualZooAPI.Data;
using VirtualZooAPI.Repositories.Implementations;
using VirtualZooShared.Models;
using VirtualZooAPI.Factories;
using Xunit;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace VirtualZooTests.Unit
{
    /// <summary>
    /// Unit tests voor de EnclosureRepository.
    /// </summary>
    public class EnclosureRepositoryTests
    {
        /// <summary>
        /// Initialiseert een testdatabase en vult deze met seed data als er nog geen verblijven zijn.
        /// </summary>
        /// <returns>Een instantie van ApplicationDbContext met testdata.</returns>
        private async Task<ApplicationDbContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=VirtueleDierentuin11Test;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            var databaseContext = new ApplicationDbContext(options);
            await databaseContext.Database.EnsureCreatedAsync();
            SeedData.Initialize(databaseContext);


            if (!await databaseContext.Enclosures.AnyAsync())
            {
                var enclosures = EnclosureFactory.CreateEnclosures(2);
                databaseContext.Enclosures.AddRange(enclosures);
                await databaseContext.SaveChangesAsync();
            }

            return databaseContext;
        }

        /// <summary>
        /// Test of GetAllEnclosuresAsync alle verblijven correct ophaalt.
        /// </summary>
        [Fact]
        public async Task GetAllEnclosuresAsync_ShouldReturnEnclosures()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new EnclosureRepository(dbContext);
            var existingCount = await dbContext.Enclosures.CountAsync();

            var testEnclosures = EnclosureFactory.CreateEnclosures(3);
            dbContext.Enclosures.AddRange(testEnclosures);
            await dbContext.SaveChangesAsync();

            var result = await repository.GetAllEnclosuresAsync();

            Assert.NotNull(result);
            Assert.Equal(existingCount + testEnclosures.Count, result.Count());
        }

        /// <summary>
        /// Test of GetEnclosureByIdAsync de juiste verblijfgegevens ophaalt.
        /// </summary>
        [Fact]
        public async Task GetEnclosureByIdAsync_ShouldReturnCorrectEnclosure()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new EnclosureRepository(dbContext);

            var testEnclosure = EnclosureFactory.CreateEnclosure();
            dbContext.Enclosures.Add(testEnclosure);
            await dbContext.SaveChangesAsync();

            var result = await repository.GetEnclosureByIdAsync(testEnclosure.Id);

            Assert.NotNull(result);
            Assert.Equal(testEnclosure.Name, result.Name);
        }

        /// <summary>
        /// Test of AddEnclosureAsync een nieuw verblijf correct toevoegt aan de database.
        /// </summary>
        [Fact]
        public async Task AddEnclosureAsync_ShouldAddEnclosure()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new EnclosureRepository(dbContext);
            var newEnclosure = EnclosureFactory.CreateEnclosure();

            await repository.AddEnclosureAsync(newEnclosure);
            var result = await dbContext.Enclosures.FirstOrDefaultAsync(e => e.Name == newEnclosure.Name);

            Assert.NotNull(result);
            Assert.Equal(newEnclosure.Name, result.Name);
        }

        /// <summary>
        /// Test of UpdateEnclosureAsync een bestaand verblijf correct bijwerkt.
        /// </summary>
        [Fact]
        public async Task UpdateEnclosureAsync_ShouldModifyEnclosure()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new EnclosureRepository(dbContext);
            var enclosure = await dbContext.Enclosures.FirstAsync();
            enclosure.Name = "Updated Enclosure";

            await repository.UpdateEnclosureAsync(enclosure);
            var updated = await dbContext.Enclosures.FindAsync(enclosure.Id);

            Assert.Equal("Updated Enclosure", updated.Name);
        }

        /// <summary>
        /// Test of DeleteEnclosureAsync een verblijf correct verwijdert uit de database.
        /// </summary>
        [Fact]
        public async Task DeleteEnclosureAsync_ShouldRemoveEnclosure()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new EnclosureRepository(dbContext);

            var enclosure = await dbContext.Enclosures.FirstAsync();
            await repository.DeleteEnclosureAsync(enclosure.Id);
            var result = await dbContext.Enclosures.FindAsync(enclosure.Id);

            Assert.Null(result);
        }
    }
}
