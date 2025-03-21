using BeestjeOpJeFeestje.Data.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeestjeOpJeFeestje.Data.Seeders
{
    public static class UserAndRoleSeeder
    {
        public static void SeedData(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ICustomerRepository customerRepository)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager, customerRepository);
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Admin"
                };
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Customer").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Customer"
                };
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
        }

        private static void SeedUsers(UserManager<IdentityUser> userManager, ICustomerRepository customerRepository)
        {
            if (userManager.FindByEmailAsync("admin@admin.com").Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                };
                IdentityResult result = userManager.CreateAsync(user, "Admin123!").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
            if (userManager.FindByEmailAsync("klant@klant1.com").Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = "klant@klant1.com",
                    Email = "klant@klant1.com",
                };

                Customer customer = new Customer
                {
                    Name = "Klant1",
                    Email = user.Email,
                    Address = "Test Adres",
                    PhoneNumber = "0612345678",
                    CustomerCard = CustomerCard.None
                };
                customerRepository.AddCustomerAndIdentityAsync(customer, user, "Klant123!").Wait();
                
            }
        }
    }
}
