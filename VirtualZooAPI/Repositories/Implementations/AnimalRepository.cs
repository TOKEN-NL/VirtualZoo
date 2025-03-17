using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualZooAPI.Data;
using VirtualZooShared.Models;
using VirtualZooAPI.Repositories.Interfaces;

namespace VirtualZooAPI.Repositories.Implementations
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly ApplicationDbContext _context;

        public AnimalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Animal>> GetAllAnimalsAsync()
        {
            return await _context.Animals.Include(a => a.Category).Include(a => a.Enclosure).ToListAsync();
        }

        public async Task<Animal> GetAnimalByIdAsync(int id)
        {
            return await _context.Animals.Include(a => a.Category).Include(a => a.Enclosure)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAnimalAsync(Animal animal)
        {
            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAnimalAsync(Animal animal)
        {
            _context.Animals.Update(animal);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnimalAsync(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal != null)
            {
                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync();
            }
        }
    }
}
