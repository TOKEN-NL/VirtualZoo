using VirtualZooShared.Models;

namespace VirtualZooAPI.Services.Interfaces
{
    public interface IEnclosureService
    {
        Task<IEnumerable<Enclosure>> GetAllEnclosuresAsync();
        Task<Enclosure?> GetEnclosureByIdAsync(int id);
        Task AddEnclosureAsync(Enclosure enclosure);
        Task UpdateEnclosureAsync(Enclosure enclosure);
        Task DeleteEnclosureAsync(int id);
    }
}
