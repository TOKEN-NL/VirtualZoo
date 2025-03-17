using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualZooShared.Models;

namespace VirtualZooAPI.Repositories.Interfaces
{
    public interface IAnimalRepository
    {
        Task<IEnumerable<Animal>> GetAllAnimalsAsync();
        Task<Animal> GetAnimalByIdAsync(int id);
        Task AddAnimalAsync(Animal animal);
        Task UpdateAnimalAsync(Animal animal);
        Task DeleteAnimalAsync(int id);
    }
}
