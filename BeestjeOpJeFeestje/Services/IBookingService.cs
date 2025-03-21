using BeestjeOpJeFeestje.Models;
using BeestjeOpJeFeestje.Data.DatabaseModels;

public interface IBookingService
{
    (decimal finalPrice, List<string> discountDetails) CalculateDiscount(BookingViewModel booking, Customer customer);
    List<string> GetAnimalRestrictions(BookingViewModel booking, Customer customer);
}
