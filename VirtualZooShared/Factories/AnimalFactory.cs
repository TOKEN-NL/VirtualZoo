using Bogus;
using VirtualZooShared.Enums;
using VirtualZooShared.Models;

namespace VirtualZooShared.Factories
{
    public static class AnimalFactory
    {
        public static Animal CreateAnimal(int? id = null)
        {
            var faker = new Faker<Animal>()
                .RuleFor(a => a.Id, f => id ?? 0) // Als geen ID wordt opgegeven, laat EF Core het invullen
                .RuleFor(a => a.Name, f => f.Name.FirstName())
                .RuleFor(a => a.Species, f => f.Lorem.Word() + "us")
                .RuleFor(a => a.Size, f => f.PickRandom<Size>())
                .RuleFor(a => a.DietaryClass, f => f.PickRandom<DietaryClass>())
                .RuleFor(a => a.ActivityPattern, f => f.PickRandom<ActivityPattern>())
                .RuleFor(a => a.SpaceRequirement, f => f.Random.Double(5, 50))
                .RuleFor(a => a.SecurityRequirement, f => f.PickRandom<SecurityLevel>())
                .RuleFor(a => a.Prey, f => f.Lorem.Word())
                .RuleFor(a => a.CategoryId, f => f.Random.Int(1, 3)) 
                .RuleFor(a => a.EnclosureId, f => f.Random.Int(1, 2)); 

            return faker.Generate();
        }

        public static List<Animal> CreateAnimals(int count)
        {
            return Enumerable.Range(1, count).Select(i => CreateAnimal()).ToList();
        }
    }
}
