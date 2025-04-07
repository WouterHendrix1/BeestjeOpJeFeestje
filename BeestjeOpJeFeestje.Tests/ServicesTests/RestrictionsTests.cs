using BeestjeOpJeFeestje.Data;
using BeestjeOpJeFeestje.Data.DatabaseModels;
using BeestjeOpJeFeestje.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BeestjeOpJeFeestje.Tests.ServicesTests
{
    public class RestrictionsTests
    {
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly BookingService _bookingService;

        public RestrictionsTests()
        {
            _mockBookingService = new Mock<IBookingService>();
            _bookingService = new BookingService();
        }

        [Fact]
        public void GetAnimalRestrictions_ShouldReturnCorrectRestrictions_WhenCalled()
        {

            // Arrange
            Customer customer = new Customer { CustomerCard = CustomerCard.None };

            var booking = new BookingViewModel
            {
                animals = new List<Animal>
                {
                    new Animal { Type = AnimalType.Farm },
                    new Animal { Name = "Leeuw" }
                }
            };
            // Act
            var result = _bookingService.GetAnimalRestrictions(booking, customer);
            // Assert
            Assert.NotNull(result);
            Assert.Contains("Nom nom nom: Je mag geen beestje boeken met het type ‘Leeuw’ of ‘IJsbeer’ als je ook een beestje boekt van het type ‘Boerderijdier’.", result);
        }

        [Fact]
        public void GetAnimalRestrictions_ShouldReturnEmptyList_WhenNoRestrictions()
        {
            // Arrange
            var customer = new Customer { CustomerCard = CustomerCard.None };
            var booking = new BookingViewModel
            {
                animals = new List<Animal>
                {
                    new Animal { Type = AnimalType.Farm },
                    new Animal { Name = "Tiger" }
                }
            };
            // Act
            var result = _bookingService.GetAnimalRestrictions(booking, customer);
            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void CheckEmptyBooking_ShouldReturnError_WhenNoAnimals()
        {
            // Arrange
            var booking = new BookingViewModel { animals = new List<Animal>() };

            // Act
            var result = _bookingService.CheckEmptyBooking(booking);

            // Assert
            Assert.Equal("Je moet minimaal 1 beestje boeken.", result);
        }

        [Fact]
        public void CheckEmptyBooking_ShouldReturnNull_WhenAnimalsExist()
        {
            // Arrange
            var booking = new BookingViewModel { animals = new List<Animal> { new Animal() } };

            // Act
            var result = _bookingService.CheckEmptyBooking(booking);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CheckConflictingAnimals_ShouldReturnError_WhenFarmAndLionConflict()
        {
            // Arrange
            var booking = new BookingViewModel
            {
                animals = new List<Animal>
                {
                    new Animal { Type = AnimalType.Farm },
                    new Animal { Name = "Leeuw" }
                }
            };

            // Act
            var result = _bookingService.CheckConflictingAnimals(booking);

            // Assert
            Assert.Equal("Nom nom nom: Je mag geen beestje boeken met het type ‘Leeuw’ of ‘IJsbeer’ als je ook een beestje boekt van het type ‘Boerderijdier’.", result);
        }

        [Fact]
        public void CheckConflictingAnimals_ShouldReturnNull_WhenNoConflict()
        {
            // Arrange
            var booking = new BookingViewModel
            {
                animals = new List<Animal>
                {
                    new Animal { Type = AnimalType.Farm },
                    new Animal { Name = "Tiger" }
                }
            };

            // Act
            var result = _bookingService.CheckConflictingAnimals(booking);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CheckWeekendPenguinRestriction_ShouldReturnError_WhenWeekendAndPenguin()
        {
            // Arrange
            var booking = new BookingViewModel
            {
                animals = new List<Animal> { new Animal { Name = "Pinguïn" } },
                SelectedDate = new DateTime(2025, 4, 5) // Saturday
            };

            // Act
            var result = _bookingService.CheckWeekendPenguinRestriction(booking);

            // Assert
            Assert.Equal("Dieren in pak werken alleen doordeweeks: Je mag geen beestje boeken met de naam ‘Pinguïn’ in het weekend.", result);
        }

        [Fact]
        public void CheckWeekendPenguinRestriction_ShouldReturnNull_WhenNotWeekendOrNoPenguin()
        {
            // Arrange
            var booking = new BookingViewModel
            {
                animals = new List<Animal> { new Animal { Name = "Pinguïn" } },
                SelectedDate = new DateTime(2025, 4, 3) // Thursday
            };

            // Act
            var result = _bookingService.CheckWeekendPenguinRestriction(booking);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CheckDesertAnimalMonthRestriction_ShouldReturnError_WhenDesertAnimalAndWinterMonth()
        {
            // Arrange
            var booking = new BookingViewModel
            {
                animals = new List<Animal> { new Animal { Type = AnimalType.Desert } },
                SelectedDate = new DateTime(2025, 1, 15) // January
            };

            // Act
            var result = _bookingService.CheckDesertAnimalMonthRestriction(booking);

            // Assert
            Assert.Equal("Brrrr – Veelste koud: Je mag geen beestje boeken van het type ‘Woestijn’ in de maanden oktober t/m februari.", result);
        }

        [Fact]
        public void CheckDesertAnimalMonthRestriction_ShouldReturnNull_WhenNotDesertAnimalOrWrongMonth()
        {
            // Arrange
            var booking = new BookingViewModel
            {
                animals = new List<Animal> { new Animal { Type = AnimalType.Farm } },
                SelectedDate = new DateTime(2025, 5, 15) // May
            };

            // Act
            var result = _bookingService.CheckDesertAnimalMonthRestriction(booking);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CheckSnowAnimalMonthRestriction_ShouldReturnError_WhenSnowAnimalAndSummerMonth()
        {
            // Arrange
            var booking = new BookingViewModel
            {
                animals = new List<Animal> { new Animal { Type = AnimalType.Snow } },
                SelectedDate = new DateTime(2025, 7, 15) // July
            };

            // Act
            var result = _bookingService.CheckSnowAnimalMonthRestriction(booking);

            // Assert
            Assert.Equal("Some People Are Worth Melting For. ~ Olaf: Je mag geen beestje boeken van het type ‘Sneeuw’ in de maanden juni t/m augustus.", result);
        }

        [Fact]
        public void CheckSnowAnimalMonthRestriction_ShouldReturnNull_WhenNotSnowAnimalOrWrongMonth()
        {
            // Arrange
            var booking = new BookingViewModel
            {
                animals = new List<Animal> { new Animal { Type = AnimalType.Farm } },
                SelectedDate = new DateTime(2025, 3, 15) // March
            };

            // Act
            var result = _bookingService.CheckSnowAnimalMonthRestriction(booking);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CheckMaxAnimalsRestriction_ShouldReturnError_WhenExceedingMaxAnimals()
        {
            // Arrange
            var customer = new Customer { CustomerCard = CustomerCard.None };
            var booking = new BookingViewModel
            {
                animals = new List<Animal>
                {
                    new Animal(),
                    new Animal(),
                    new Animal(),
                    new Animal(),
                }
            };

            // Act
            var result = _bookingService.CheckMaxAnimalsRestriction(booking, customer);

            // Assert
            Assert.Equal("Je mag maximaal 3 dieren boeken.", result);
        }

        [Fact]
        public void CheckMaxAnimalsRestriction_ShouldReturnNull_WhenWithinMaxAnimals()
        {
            // Arrange
            var customer = new Customer { CustomerCard = CustomerCard.Silver };
            var booking = new BookingViewModel
            {
                animals = new List<Animal>
                {
                    new Animal(),
                    new Animal(),
                    new Animal()
                }
            };

            // Act
            var result = _bookingService.CheckMaxAnimalsRestriction(booking, customer);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CheckVipAnimalRestriction_ShouldReturnError_WhenNonPlatinumCustomerBooksVipAnimal()
        {
            // Arrange
            var customer = new Customer { CustomerCard = CustomerCard.Gold };
            var booking = new BookingViewModel
            {
                animals = new List<Animal> { new Animal { Type = AnimalType.VIP } }
            };

            // Act
            var result = _bookingService.CheckVipAnimalRestriction(booking, customer);

            // Assert
            Assert.Equal("Alleen klanten met een platina kaart mogen VIP dieren boeken.", result);
        }

        [Fact]
        public void CheckVipAnimalRestriction_ShouldReturnNull_WhenPlatinumCustomerBooksVipAnimal()
        {
            // Arrange
            var customer = new Customer { CustomerCard = CustomerCard.Platinum };
            var booking = new BookingViewModel
            {
                animals = new List<Animal> { new Animal { Type = AnimalType.VIP } }
            };

            // Act
            var result = _bookingService.CheckVipAnimalRestriction(booking, customer);

            // Assert
            Assert.Null(result);
        }
    }
}