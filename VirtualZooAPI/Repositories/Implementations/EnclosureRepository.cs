using Microsoft.EntityFrameworkCore;
using VirtualZooAPI.Data;
using VirtualZooAPI.Repositories.Interfaces;
using VirtualZooShared.Data;
using VirtualZooShared.Models;

namespace VirtualZooAPI.Repositories.Implementations
{
    public class EnclosureRepository : IEnclosureRepository
    {
        private readonly ApplicationDbContext _context;

        public EnclosureRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Enclosure>> GetAllEnclosuresAsync()
        {
            return await _context.Enclosures.Include(e => e.Animals).ToListAsync();
        }

        public async Task<Enclosure?> GetEnclosureByIdAsync(int id)
        {
            return await _context.Enclosures.Include(e => e.Animals).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddEnclosureAsync(Enclosure enclosure)
        {
            _context.Enclosures.Add(enclosure);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEnclosureAsync(Enclosure enclosure)
        {
            _context.Enclosures.Update(enclosure);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEnclosureAsync(int id)
        {
            var enclosure = await _context.Enclosures.FindAsync(id);
            if (enclosure != null)
            {
                _context.Enclosures.Remove(enclosure);
                await _context.SaveChangesAsync();
            }
        }
    }
}
