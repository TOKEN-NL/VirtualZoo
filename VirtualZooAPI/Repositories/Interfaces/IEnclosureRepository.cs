using VirtualZooShared.Models;

namespace VirtualZooAPI.Repositories.Interfaces
{
    public interface IEnclosureRepository
    {
        Task<IEnumerable<Enclosure>> GetAllEnclosuresAsync();
        Task<Enclosure?> GetEnclosureByIdAsync(int id);
        Task AddEnclosureAsync(Enclosure enclosure);
        Task UpdateEnclosureAsync(Enclosure enclosure);
        Task DeleteEnclosureAsync(int id);
    }
}
