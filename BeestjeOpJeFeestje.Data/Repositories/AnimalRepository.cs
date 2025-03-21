using BeestjeOpJeFeestje.Data.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeestjeOpJeFeestje.Data.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly DatabaseContext _context;

        public AnimalRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Animal>> GetAllAsync()
        {
            return await _context.Animals.OrderBy(a => a.Type).ThenBy(b => b.Name).ToListAsync();
        }

        public async Task<Animal?> GetByIdAsync(int id)
        {
            return await _context.Animals.Include(a => a.Bookings).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Animal>> GetAnimalsByIdsAsync(List<int> ids)
        {
            return await _context.Animals.Where(a => ids.Contains(a.Id)).ToListAsync();
        }

        public async Task<Animal?> GetAnimalCompleteAsync(int id)
        {
            return await _context.Animals
                .Include(a => a.Bookings)
                .ThenInclude(b => b.Customer)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Animal animal)
        {
            await _context.Animals.AddAsync(animal);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Animal animal)
        {
            _context.Animals.Update(animal);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal != null)
            {
                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Animals.AnyAsync(a => a.Id == id);
        }
    }
}
