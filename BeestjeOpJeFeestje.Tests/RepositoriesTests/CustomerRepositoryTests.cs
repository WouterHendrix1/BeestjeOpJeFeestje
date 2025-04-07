using BeestjeOpJeFeestje.Data;
using BeestjeOpJeFeestje.Data.DatabaseModels;
using BeestjeOpJeFeestje.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeestjeOpJeFeestje.Tests.Repositories
{
    public class CustomerRepositoryTests : IDisposable
    {
        private readonly DatabaseContext _context;
        private readonly CustomerRepository _customerRepository;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;

        public CustomerRepositoryTests()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new DatabaseContext(options);

            // Mocking UserManager
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                new Mock<IUserStore<IdentityUser>>().Object,
                null, null, null, null, null, null, null, null
            );

            _customerRepository = new CustomerRepository(_context, _mockUserManager.Object);
        }

        // Test for AddCustomerAsync method
        [Fact]
        public async Task AddCustomerAsync_ShouldAddCustomer()
        {
            // Arrange
            var customer = new Customer { Name = "John Doe", Email = "john.doe@example.com" };

            // Act
            await _customerRepository.AddCustomerAsync(customer);
            var addedCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == customer.Email);

            // Assert
            Assert.NotNull(addedCustomer);
            Assert.Equal(customer.Name, addedCustomer.Name);
            Assert.Equal(customer.Email, addedCustomer.Email);

            // Clean up
            _context.Customers.Remove(addedCustomer);
            await _context.SaveChangesAsync();

        }

        // Test for GetAllAsync method
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCustomers()
        {
            // Arrange
            var customer1 = new Customer { Name = "John Doe", Email = "john.doe@example.com" };
            var customer2 = new Customer { Name = "Jane Smith", Email = "jane.smith@example.com" };

            _context.Customers.Add(customer1);
            _context.Customers.Add(customer2);
            await _context.SaveChangesAsync();

            // Act
            var customers = await _customerRepository.GetAllAsync();

            // Assert
            Assert.Equal(3, customers.Count()); // 1 from seeding + 2 added
            Assert.Contains(customers, c => c.Name == "John Doe");
            Assert.Contains(customers, c => c.Name == "Jane Smith");

            // Clean up
            _context.Customers.Remove(customer1);
            _context.Customers.Remove(customer2);
            await _context.SaveChangesAsync();
        }

        // Test for GetByIdAsync method
        [Fact]
        public async Task GetByIdAsync_ShouldReturnCustomerById()
        {
            // Arrange
            var customer = new Customer { Name = "John Doe", Email = "john.doe@example.com" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var foundCustomer = await _customerRepository.GetByIdAsync(customer.Id);

            // Assert
            Assert.NotNull(foundCustomer);
            Assert.Equal(customer.Id, foundCustomer.Id);

            // Clean up
            _context.Customers.Remove(foundCustomer);
            await _context.SaveChangesAsync();
        }

        // Test for GetCustomerCompleteAsync method
        [Fact]
        public async Task GetCustomerCompleteAsync_ShouldReturnCustomerWithBookingsAndAnimals()
        {
            // Arrange
            var customer = new Customer { Name = "John Doe", Email = "john.doe@example.com" };
            var booking = new Booking
            {
                Date = DateTime.Now,
                TotalPrice = 100,
                IsConfirmed = true,
                CustomerId = customer.Id
            };
            var animal = new Animal { Name = "Lion", Type = AnimalType.Jungle, Price = 100 };

            customer.Bookings = new List<Booking> { booking };
            booking.Animals = new List<Animal> { animal };

            _context.Customers.Add(customer);
            _context.Animals.Add(animal);
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act
            var foundCustomer = await _customerRepository.GetCustomerCompleteAsync(customer.Id);

            // Assert
            Assert.NotNull(foundCustomer);
            Assert.Contains(foundCustomer.Bookings, b => b.Date == booking.Date);
            Assert.Contains(foundCustomer.Bookings.SelectMany(b => b.Animals), a => a.Name == "Lion");

            // Clean up
            _context.Customers.Remove(foundCustomer);
            _context.Animals.Remove(animal);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }

        // Test for UpdateAsync method
        [Fact]
        public async Task UpdateAsync_ShouldUpdateCustomer()
        {
            // Arrange
            var customer = new Customer { Name = "John Doe", Email = "john.doe@example.com" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Update the customer
            customer.Name = "John Updated";
            await _customerRepository.UpdateAsync(customer);

            // Act
            var updatedCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id);

            // Assert
            Assert.NotNull(updatedCustomer);
            Assert.Equal("John Updated", updatedCustomer.Name);

            // Clean up
            _context.Customers.Remove(updatedCustomer);
            await _context.SaveChangesAsync();
        }

        // Test for DeleteAsync method
        [Fact]
        public async Task DeleteAsync_ShouldDeleteCustomer()
        {
            // Arrange
            var customer = new Customer { Name = "John Doe", Email = "john.doe@example.com" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            await _customerRepository.DeleteAsync(customer.Id);
            var deletedCustomer = await _context.Customers.FindAsync(customer.Id);

            // Assert
            Assert.Null(deletedCustomer);
        }

        // Test for GetCustomerByUserIdAsync method
        [Fact]
        public async Task GetCustomerByUserIdAsync_ShouldReturnCustomerByUserId()
        {
            // Arrange
            var customer = new Customer { Name = "John Doe", Email = "john.doe@example.com", UserId = "user123" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var foundCustomer = await _customerRepository.GetCustomerByUserIdAsync("user123");

            // Assert
            Assert.NotNull(foundCustomer);
            Assert.Equal("user123", foundCustomer.UserId);

            // Clean up
            _context.Customers.Remove(foundCustomer);
            await _context.SaveChangesAsync();
        }

        // Dispose method to clean up after each test
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
