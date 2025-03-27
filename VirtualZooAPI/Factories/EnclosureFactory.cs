using Bogus;
using VirtualZooShared.Enums;
using VirtualZooShared.Models;

namespace VirtualZooAPI.Factories
{
    public static class EnclosureFactory
    {
        public static Enclosure CreateEnclosure(int? id = null, int zooId = 1)
        {
            var faker = new Faker<Enclosure>()
                .RuleFor(e => e.Name, f => "Enclosure " + f.Random.Number(1, 50))
                .RuleFor(e => e.Climate, f => f.PickRandom<Climate>())
                .RuleFor(e => e.HabitatType, f => f.PickRandom<HabitatType>())
                .RuleFor(e => e.SecurityLevel, f => f.PickRandom<SecurityLevel>())
                .RuleFor(e => e.Size, f => Math.Round(f.Random.Double(50, 500), 2))
                .RuleFor(e => e.ZooId, _ => zooId); // Default to 1 unless specified

            return faker.Generate();
        }


        public static List<Enclosure> CreateEnclosures(int count)
        {
            return Enumerable.Range(1, count).Select(i => CreateEnclosure(i)).ToList();
        }
    }
}
