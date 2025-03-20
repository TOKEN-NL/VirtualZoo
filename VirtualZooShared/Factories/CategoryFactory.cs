﻿using Bogus;
using VirtualZooShared.Models;

namespace VirtualZooShared.Factories
{
    public static class CategoryFactory
    {
        public static Category CreateCategory(int? id = null)
        {
            var faker = new Faker<Category>()
                .RuleFor(c => c.Id, f => id ?? 0)
                .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0]);

            return faker.Generate();
        }

        public static List<Category> CreateCategories(int count)
        {
            return Enumerable.Range(1, count).Select(i => CreateCategory(i)).ToList();
        }
    }
}
