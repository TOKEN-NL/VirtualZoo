using VirtualZooAPI.Repositories.Interfaces;
using VirtualZooAPI.Services.Interfaces;
using VirtualZooShared.Models;

namespace VirtualZooAPI.Services.Implementations
{
    public class EnclosureService : IEnclosureService
    {
        private readonly IEnclosureRepository _repository;

        public EnclosureService(IEnclosureRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Enclosure>> GetAllEnclosuresAsync() =>
            await _repository.GetAllEnclosuresAsync();

        public async Task<Enclosure?> GetEnclosureByIdAsync(int id) =>
            await _repository.GetEnclosureByIdAsync(id);

        public async Task AddEnclosureAsync(Enclosure enclosure) =>
            await _repository.AddEnclosureAsync(enclosure);

        public async Task UpdateEnclosureAsync(Enclosure enclosure) =>
            await _repository.UpdateEnclosureAsync(enclosure);

        public async Task DeleteEnclosureAsync(int id) =>
            await _repository.DeleteEnclosureAsync(id);
    }
}
