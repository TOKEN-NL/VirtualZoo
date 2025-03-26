using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualZooShared.Models;
using VirtualZooAPI.Repositories.Interfaces;
using VirtualZooAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using VirtualZooAPI.Data;

namespace VirtualZooAPI.Services.Implementations
{
    public class AnimalService : IAnimalService
    {
        private readonly IAnimalRepository _animalRepository;

        public AnimalService(IAnimalRepository animalRepository)
        {
            _animalRepository = animalRepository;
        }

        public async Task<IEnumerable<Animal>> GetAllAnimalsAsync()
        {
            return await _animalRepository.GetAllAnimalsAsync();
        }

        public async Task<Animal> GetAnimalByIdAsync(int id)
        {
            return await _animalRepository.GetAnimalByIdAsync(id);
        }

        public async Task AddAnimalAsync(Animal animal)
        {
            await _animalRepository.AddAnimalAsync(animal);
        }

        public async Task UpdateAnimalAsync(Animal animal)
        {
            await _animalRepository.UpdateAnimalAsync(animal);
        }

        public async Task DeleteAnimalAsync(int id)
        {
            await _animalRepository.DeleteAnimalAsync(id);
        }

    }
}
