using BeestjeOpJeFeestje.Data;
using BeestjeOpJeFeestje.Data.DatabaseModels;
using BeestjeOpJeFeestje.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BeestjeOpJeFeestje.Tests.Repositories
{
    public class BookingRepositoryTests : IDisposable
    {
        private readonly DatabaseContext _context;
        private readonly BookingRepository _bookingRepository;

        public BookingRepositoryTests()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new DatabaseContext(options);
            _bookingRepository = new BookingRepository(_context);
        }

        // Test for AddAsync method
        [Fact]
        public async Task AddAsync_ShouldAddBooking()
        {
            // Arrange
            var customer = new Customer { Id = 99, Name = "John Doe" };
            var animal = new Animal { Name = "Lion", Type = AnimalType.Jungle, Price = 100 };
            var booking = new Booking
            {
                Date = DateTime.Now,
                CustomerId = customer.Id,
                TotalPrice = 100,
                Animals = new List<Animal> { animal }
            };

            _context.Customers.Add(customer);
            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();

            // Act
            await _bookingRepository.AddAsync(booking);
            var addedBooking = await _context.Bookings.FirstOrDefaultAsync(b => b.CustomerId == customer.Id);

            // Assert
            Assert.NotNull(addedBooking);
            Assert.Equal(booking.Date, addedBooking.Date);
            Assert.Equal(booking.TotalPrice, addedBooking.TotalPrice);
            Assert.Contains(addedBooking.Animals, a => a.Name == "Lion");

            // Clean up
            _context.Bookings.Remove(addedBooking);
            _context.Animals.Remove(animal);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

        }

        // Test for DeleteAsync method
        [Fact]
        public async Task DeleteAsync_ShouldDeleteBooking()
        {
            // Arrange
            var customer = new Customer { Id = 100, Name = "John Doe" };
            var booking = new Booking
            {
                Date = DateTime.Now,
                CustomerId = customer.Id,
                TotalPrice = 100
            };
            _context.Customers.Add(customer);
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act
            await _bookingRepository.DeleteAsync(booking.Id);
            var deletedBooking = await _context.Bookings.FindAsync(booking.Id);

            // Assert
            Assert.Null(deletedBooking);

            // Clean up
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

        }

        // Test for GetAllAsync method
        [Fact]
        public async Task GetAllAsync_ShouldReturnBookings()
        {

            // Arrange
            var customer = new Customer { Id = 98, Name = "John Doe" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var booking1 = new Booking
            {
                Date = DateTime.Now,
                CustomerId = 98,
                TotalPrice = 100
            };
            var booking2 = new Booking
            {
                Date = DateTime.Now.AddDays(1),
                CustomerId = 98,
                TotalPrice = 150
            };

            _context.Bookings.Add(booking1);
            _context.Bookings.Add(booking2);
            await _context.SaveChangesAsync();

            // Act
            var bookings = await _bookingRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, bookings.Count);

            // Clean up
            _context.Bookings.Remove(booking1);
            _context.Bookings.Remove(booking2);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        // Test for GetBookedAnimalsByDateAsync method
        [Fact]
        public async Task GetBookedAnimalsByDateAsync_ShouldReturnAnimals()
        {
            var customer = new Customer { Id = 97, Name = "John Doe" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            // Arrange
            var animal = new Animal { Name = "Lion", Type = AnimalType.Jungle, Price = 100 };
            var booking = new Booking
            {
                Date = DateTime.Now.Date,
                CustomerId = 97,
                TotalPrice = 100,
                Animals = new List<Animal> { animal }
            };
            _context.Animals.Add(animal);
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act
            var animals = await _bookingRepository.GetBookedAnimalsByDateAsync(DateTime.Now.Date);

            // Assert
            Assert.Single(animals);
            Assert.Equal("Lion", animals.First().Name);

            // Clean up
            _context.Bookings.Remove(booking);
            _context.Customers.Remove(customer);
            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();
        }

        // Test for GetBookingsByCustomerIdAsync method
        [Fact]
        public async Task GetBookingsByCustomerIdAsync_ShouldReturnBookingsForCustomer()
        {
            // Arrange
            var customer = new Customer { Id = 999, Name = "John Doe" };
            var booking1 = new Booking
            {
                Date = DateTime.Now,
                CustomerId = customer.Id,
                TotalPrice = 100
            };
            var booking2 = new Booking
            {
                Date = DateTime.Now.AddDays(1),
                CustomerId = customer.Id,
                TotalPrice = 150
            };

            _context.Customers.Add(customer);
            _context.Bookings.Add(booking1);
            _context.Bookings.Add(booking2);
            await _context.SaveChangesAsync();

            // Act
            var bookings = await _bookingRepository.GetBookingsByCustomerIdAsync(customer.Id);

            // Assert
            Assert.Equal(2, bookings.Count);

            // Clean up
            _context.Bookings.Remove(booking1);
            _context.Bookings.Remove(booking2);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        // Test for GetByIdAsync method
        [Fact]
        public async Task GetByIdAsync_ShouldReturnBooking()
        {
            // Arrange
            var customer = new Customer { Id = 102, Name = "John Doe" };
            var booking = new Booking
            {
                Date = DateTime.Now,
                CustomerId = customer.Id,
                TotalPrice = 100
            };
            _context.Customers.Add(customer);
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act
            var foundBooking = await _bookingRepository.GetByIdAsync(booking.Id);

            // Assert
            Assert.NotNull(foundBooking);
            Assert.Equal(booking.Id, foundBooking.Id);

            // Clean up
            _context.Bookings.Remove(foundBooking);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

        }

        // Dispose method to clean up after each test
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
