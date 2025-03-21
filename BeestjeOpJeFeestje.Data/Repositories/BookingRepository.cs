using BeestjeOpJeFeestje.Data.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeestjeOpJeFeestje.Data.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DatabaseContext _context;
        public BookingRepository(DatabaseContext databaseContext)
        {
            _context = databaseContext;
        }

        public async Task AddAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Animal>> GetBookedAnimalsByDateAsync(DateTime selectedDate)
        {
            var bookings = _context.Bookings.Where(b => b.Date == selectedDate);
            var animals = await bookings.SelectMany(b => b.Animals).ToListAsync();
            return animals;
        }

        public Task<List<Booking>> GetBookingsByCustomerIdAsync(int id)
        {
            var bookings = _context.Bookings.Where(b => b.CustomerId == id).Include(b => b.Animals).ToListAsync();
            return bookings;
        }

        public async Task<Booking> GetByIdAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            return booking;
        }

        public Task UpdateAsync(Booking booking)
        {
            throw new NotImplementedException();
        }
    }
}
