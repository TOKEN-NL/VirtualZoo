using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualZooShared.Data;
using VirtualZooAPI.Repositories.Implementations;
using VirtualZooShared.Models;
using VirtualZooShared.Factories;
using Xunit;

namespace VirtualZooTests.Unit
{
    /// <summary>
    /// Unit tests voor de CategoryRepository.
    /// </summary>
    public class CategoryRepositoryTests
    {
        /// <summary>
        /// Initialiseert een testdatabase en seed data indien nodig.
        /// </summary>
        private async Task<ApplicationDbContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=VirtueleDierentuin11Test;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            var databaseContext = new ApplicationDbContext(options);
            await databaseContext.Database.EnsureCreatedAsync();
            SeedData.Initialize(databaseContext);

            if (!await databaseContext.Categories.AnyAsync())
            {
                var categories = CategoryFactory.CreateCategories(2);
                databaseContext.Categories.AddRange(categories);
                await databaseContext.SaveChangesAsync();
            }

            return databaseContext;
        }

        /// <summary>
        /// Test of GetAllCategoriesAsync alle categorieën correct ophaalt.
        /// </summary>
        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnCategories()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new CategoryRepository(dbContext);
            var existingCategoryCount = await dbContext.Categories.CountAsync();

            var testCategories = CategoryFactory.CreateCategories(5);
            dbContext.Categories.AddRange(testCategories);
            await dbContext.SaveChangesAsync();

            var result = await repository.GetAllCategoriesAsync();

            Assert.NotNull(result);
            Assert.Equal(existingCategoryCount + testCategories.Count, result.Count());
        }

        /// <summary>
        /// Test of GetCategoryByIdAsync de juiste categorie ophaalt.
        /// </summary>
        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnCorrectCategory()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new CategoryRepository(dbContext);

            var testCategory = CategoryFactory.CreateCategory();
            dbContext.Categories.Add(testCategory);
            await dbContext.SaveChangesAsync();

            var result = await repository.GetCategoryByIdAsync(testCategory.Id);

            Assert.NotNull(result);
            Assert.Equal(testCategory.Name, result.Name);
        }

        /// <summary>
        /// Test of AddCategoryAsync een nieuwe categorie correct toevoegt.
        /// </summary>
        [Fact]
        public async Task AddCategoryAsync_ShouldAddCategory()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new CategoryRepository(dbContext);
            var newCategory = CategoryFactory.CreateCategory();

            await repository.AddCategoryAsync(newCategory);
            var result = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == newCategory.Name);

            Assert.NotNull(result);
            Assert.Equal(newCategory.Name, result.Name);
        }

        /// <summary>
        /// Test of UpdateCategoryAsync een bestaande categorie correct bijwerkt.
        /// </summary>
        [Fact]
        public async Task UpdateCategoryAsync_ShouldModifyCategory()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new CategoryRepository(dbContext);
            var category = await dbContext.Categories.FirstAsync();
            category.Name = "Updated Category";

            await repository.UpdateCategoryAsync(category);
            var updatedCategory = await dbContext.Categories.FindAsync(category.Id);

            Assert.Equal("Updated Category", updatedCategory.Name);
        }

        /// <summary>
        /// Test of DeleteCategoryAsync een categorie correct verwijdert.
        /// </summary>
        [Fact]
        public async Task DeleteCategoryAsync_ShouldRemoveCategory()
        {
            var dbContext = await GetDatabaseContext();
            var repository = new CategoryRepository(dbContext);

            var category = await dbContext.Categories.FirstAsync();
            await repository.DeleteCategoryAsync(category.Id);
            var result = await dbContext.Categories.FindAsync(category.Id);

            Assert.Null(result);
        }
    }
}
