using BeestjeOpJeFeestje.Models;
using BeestjeOpJeFeestje.Data.DatabaseModels;

public interface IBookingService
{
    (decimal finalPrice, List<string> discountDetails) CalculateDiscount(BookingViewModel booking, Customer customer);
    List<string> GetAnimalRestrictions(BookingViewModel booking, Customer customer);
    string? CheckEmptyBooking(BookingViewModel booking);
    string? CheckConflictingAnimals(BookingViewModel booking);
    string? CheckWeekendPenguinRestriction(BookingViewModel booking);
    string? CheckDesertAnimalMonthRestriction(BookingViewModel booking);
    string? CheckSnowAnimalMonthRestriction(BookingViewModel booking);
    string? CheckMaxAnimalsRestriction(BookingViewModel booking, Customer customer);
    string? CheckVipAnimalRestriction(BookingViewModel booking, Customer customer);
}
