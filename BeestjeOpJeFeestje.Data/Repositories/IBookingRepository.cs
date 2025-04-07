using BeestjeOpJeFeestje.Data.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeestjeOpJeFeestje.Data.Repositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking> GetByIdAsync(int id);
        Task AddAsync(Booking booking);
        Task DeleteAsync(int id);
        Task<IEnumerable<Animal>> GetBookedAnimalsByDateAsync(DateTime selectedDate);
        Task<List<Booking>> GetBookingsByCustomerIdAsync(int id);
    }
}
