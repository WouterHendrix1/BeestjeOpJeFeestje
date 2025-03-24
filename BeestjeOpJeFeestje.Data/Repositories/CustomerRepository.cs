using BeestjeOpJeFeestje.Data.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeestjeOpJeFeestje.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomerRepository(DatabaseContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers.OrderBy(b => b.Name).ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.Include(a => a.Bookings).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Customer?> GetCustomerCompleteAsync(int id)
        {
            return await _context.Customers
                .Include(a => a.Bookings)
                .ThenInclude(b => b.Animals)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddCustomerAndIdentityAsync(Customer customer, IdentityUser user, String password)
        {
            await _userManager.CreateAsync(user, password);
            await _userManager.AddToRoleAsync(user, "Customer");
            customer.User = user;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Customers.AnyAsync(a => a.Id == id);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<Customer> GetCustomerByUserIdAsync(string userId)
        {
         
            return await _context.Customers.FirstAsync(x => x.UserId == userId);
        }
    }
}
