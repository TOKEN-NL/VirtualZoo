using Bogus;
using VirtualZooShared.Enums;
using VirtualZooShared.Models;

namespace VirtualZooShared.Factories
{
    public static class EnclosureFactory
    {
        public static Enclosure CreateEnclosure(int? id = null)
        {
            var faker = new Faker<Enclosure>()
                .RuleFor(e => e.Id, f => id ?? 0)
                .RuleFor(e => e.Name, f => "Enclosure " + f.Random.Number(1, 50))
                .RuleFor(e => e.Climate, f => f.PickRandom<Climate>())
                .RuleFor(e => e.HabitatType, f => f.PickRandom<HabitatType>())
                .RuleFor(e => e.SecurityLevel, f => f.PickRandom<SecurityLevel>())
                .RuleFor(e => e.Size, f => f.Random.Double(50, 500));

            return faker.Generate();
        }

        public static List<Enclosure> CreateEnclosures(int count)
        {
            return Enumerable.Range(1, count).Select(i => CreateEnclosure(i)).ToList();
        }
    }
}
