using BeestjeOpJeFeestje.Data;
using BeestjeOpJeFeestje.Data.DatabaseModels;
using Xunit;

namespace BeestjeOpJeFeestje.Tests
{
    public class DiscountTests
    {
        private readonly BookingService _bookingService;

        public DiscountTests()
        {
            _bookingService = new BookingService();
        }

        [Fact]
        public void ApplySameTypeDiscount_ShouldApplyDiscount_WhenThreeOrMoreSameType()
        {
            // Arrange
            var animals = new List<Animal>
                {
                    new Animal { Type = AnimalType.Farm, Name = "Cow", Price = 100 },
                    new Animal { Type = AnimalType.Farm, Name = "Sheep", Price = 100 },
                    new Animal { Type = AnimalType.Farm, Name = "Goat", Price = 100 }
                };
            var discountDetails = new List<string>();

            // Act
            var discount = _bookingService.ApplySameTypeDiscount(animals, discountDetails);

            // Assert
            Assert.Equal(10m, discount);
            Assert.Contains("3 dieren van hetzelfde type: 10% korting", discountDetails);
        }

        [Fact]
        public void ApplyDuckDiscount_ShouldApplyDiscount_WhenDuckIsPresent()
        {
            // Arrange
            var animals = new List<Animal>
                {
                    new Animal { Name = "Eend", Price = 100 }
                };
            var discountDetails = new List<string>();

            // Act
            var discount = _bookingService.ApplyDuckDiscount(animals, discountDetails);

            // Assert
            Assert.True(discount == 0m || discount == 50m);
            if (discount == 50m)
            {
                Assert.Contains("Geluk! 50% korting voor 'Eend'", discountDetails);
            }
            else
            {
                Assert.Contains("Pech, geen korting voor 'Eend'!", discountDetails);
            }
        }

        [Fact]
        public void ApplyDayOfWeekDiscount_ShouldApplyDiscount_WhenBookingOnMondayOrTuesday()
        {
            // Arrange
            var selectedDate = new DateTime(2025, 3, 17); // Monday
            var discountDetails = new List<string>();

            // Act
            var discount = _bookingService.ApplyDayOfWeekDiscount(selectedDate, discountDetails);

            // Assert
            Assert.Equal(15m, discount);
            Assert.Contains("Boeking op maandag of dinsdag: 15% korting", discountDetails);
        }

        [Fact]
        public void ApplyLetterDiscount_ShouldApplyDiscount_WhenAnimalNameContainsAorB()
        {
            // Arrange
            var animals = new List<Animal>
                {
                    new Animal { Name = "ABCDEFG", Price = 100 },
                };
            var discountDetails = new List<string>();

            // Act
            var discount = _bookingService.ApplyLetterDiscount(animals, discountDetails);

            // Assert
            Assert.Equal(14m, discount);
            Assert.Contains("Dierenamen: 14", discountDetails);
        }

        [Fact]
        public void ApplyCustomerCardDiscount_ShouldApplyDiscount_WhenCustomerHasCard()
        {
            // Arrange
            var customer = new Customer { CustomerCard = CustomerCard.Gold };
            var discountDetails = new List<string>();

            // Act
            var discount = _bookingService.ApplyCustomerCardDiscount(customer, discountDetails);

            // Assert
            Assert.Equal(10m, discount);
            Assert.Contains("Klantenkaart: 10% korting", discountDetails);
        }
    }
}
