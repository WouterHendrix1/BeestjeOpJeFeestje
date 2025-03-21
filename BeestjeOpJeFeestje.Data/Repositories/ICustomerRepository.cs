using BeestjeOpJeFeestje.Data.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetCustomerCompleteAsync(int id);
    Task AddCustomerAndIdentityAsync(Customer customer, IdentityUser user, string password);
    Task AddCustomerAsync(Customer customer);
    Task<Customer> GetCustomerByUserIdAsync(string userId);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
