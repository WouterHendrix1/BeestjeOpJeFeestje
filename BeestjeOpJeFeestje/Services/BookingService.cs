using BeestjeOpJeFeestje.Models;
using BeestjeOpJeFeestje.Data.DatabaseModels;
using BeestjeOpJeFeestje.Data.Repositories;
using BeestjeOpJeFeestje.Data;

public class BookingService : IBookingService
{
    public (decimal finalPrice, List<string> discountDetails) CalculateDiscount(BookingViewModel booking, Customer customer)
    {
        decimal discountPercentage = 0m;
        List<string> discountDetails = new List<string>();

        var animals = booking.animals;
        decimal originalPrice = animals.Sum(a => a.Price);

        discountPercentage += ApplySameTypeDiscount(animals, discountDetails);
        discountPercentage += ApplyDuckDiscount(animals, discountDetails);
        discountPercentage += ApplyDayOfWeekDiscount(booking.SelectedDate, discountDetails);
        discountPercentage += ApplyLetterDiscount(animals, discountDetails);
        discountPercentage += ApplyCustomerCardDiscount(customer, discountDetails);

        if (discountPercentage > 60m)
        {
            discountPercentage = 60m;
            discountDetails.Add("Maximale korting bereikt: 60%");
        }

        decimal discountAmount = originalPrice * (discountPercentage / 100);
        decimal finalPrice = originalPrice - discountAmount;

        return (finalPrice, discountDetails);
    }

    public List<string> GetAnimalRestrictions(BookingViewModel booking, Customer customer)
    {
        var restrictions = new List<string>();

        string? result;

        result = CheckEmptyBooking(booking);
        if (result != null) restrictions.Add(result);

        result = CheckConflictingAnimals(booking);
        if (result != null) restrictions.Add(result);

        result = CheckWeekendPenguinRestriction(booking);
        if (result != null) restrictions.Add(result);

        result = CheckDesertAnimalMonthRestriction(booking);
        if (result != null) restrictions.Add(result);

        result = CheckSnowAnimalMonthRestriction(booking);
        if (result != null) restrictions.Add(result);

        result = CheckMaxAnimalsRestriction(booking, customer);
        if (result != null) restrictions.Add(result);

        result = CheckVipAnimalRestriction(booking, customer);
        if (result != null) restrictions.Add(result);

        return restrictions;
    }

    public string? CheckEmptyBooking(BookingViewModel booking)
    {
        return booking.animals.Count == 0
            ? "Je moet minimaal 1 beestje boeken."
            : null;
    }

    public string? CheckConflictingAnimals(BookingViewModel booking)
    {
        var animals = booking.animals;

        bool hasFarmAnimal = animals.Any(a => a.Type == AnimalType.Farm);
        bool hasLionOrPolarBear = animals.Any(a =>
            a.Name.Contains("Leeuw", StringComparison.OrdinalIgnoreCase) ||
            a.Name.Contains("IJsbeer", StringComparison.OrdinalIgnoreCase));

        if (hasFarmAnimal && hasLionOrPolarBear)
        {
            return "Nom nom nom: Je mag geen beestje boeken met het type ‘Leeuw’ of ‘IJsbeer’ als je ook een beestje boekt van het type ‘Boerderijdier’.";
        }

        return null;
    }

    public string? CheckWeekendPenguinRestriction(BookingViewModel booking)
    {
        var animals = booking.animals;
        bool isWeekend = booking.SelectedDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        bool hasPenguin = animals.Any(a =>
            a.Name.Contains("Pinguïn", StringComparison.OrdinalIgnoreCase));

        if (isWeekend && hasPenguin)
        {
            return "Dieren in pak werken alleen doordeweeks: Je mag geen beestje boeken met de naam ‘Pinguïn’ in het weekend.";
        }

        return null;
    }

    public string? CheckDesertAnimalMonthRestriction(BookingViewModel booking)
    {
        var animals = booking.animals;
        int month = booking.SelectedDate.Month;

        if ((month >= 10 || month <= 2) && animals.Any(a => a.Type == AnimalType.Desert))
        {
            return "Brrrr – Veelste koud: Je mag geen beestje boeken van het type ‘Woestijn’ in de maanden oktober t/m februari.";
        }

        return null;
    }

    public string? CheckSnowAnimalMonthRestriction(BookingViewModel booking)
    {
        var animals = booking.animals;
        int month = booking.SelectedDate.Month;

        if ((month >= 6 && month <= 8) && animals.Any(a => a.Type == AnimalType.Snow))
        {
            return "Some People Are Worth Melting For. ~ Olaf: Je mag geen beestje boeken van het type ‘Sneeuw’ in de maanden juni t/m augustus.";
        }

        return null;
    }

    public string? CheckMaxAnimalsRestriction(BookingViewModel booking, Customer customer)
    {
        var animals = booking.animals;

        int maxAnimals = customer.CustomerCard switch
        {
            CustomerCard.None => 3,
            CustomerCard.Silver => 4,
            CustomerCard.Gold => int.MaxValue,
            CustomerCard.Platinum => int.MaxValue,
            _ => 3
        };

        if (animals.Count > maxAnimals)
        {
            return $"Je mag maximaal {maxAnimals} dieren boeken.";
        }

        return null;
    }

    public string? CheckVipAnimalRestriction(BookingViewModel booking, Customer customer)
    {
        var animals = booking.animals;

        if (customer.CustomerCard != CustomerCard.Platinum &&
            animals.Any(a => a.Type == AnimalType.VIP))
        {
            return "Alleen klanten met een platina kaart mogen VIP dieren boeken.";
        }

        return null;
    }


    public decimal ApplySameTypeDiscount(List<Animal> animals, List<string> discountDetails)
    {
        if (animals.GroupBy(a => a.Type).Any(g => g.Count() >= 3))
        {
            discountDetails.Add("3 dieren van hetzelfde type: 10% korting");
            return 10m;
        }
        return 0m;
    }

    public decimal ApplyDuckDiscount(List<Animal> animals, List<string> discountDetails)
    {
        if (animals.Any(a => a.Name.Contains("Eend", StringComparison.OrdinalIgnoreCase)))
        {
            Random random = new Random();
            if (random.Next(1, 7) == 1)
            {
                discountDetails.Add("Geluk! 50% korting voor 'Eend'");
                return 50m;
            }
        }
        return 0m;
    }

    public decimal ApplyDayOfWeekDiscount(DateTime selectedDate, List<string> discountDetails)
    {
        if (selectedDate.DayOfWeek == DayOfWeek.Monday || selectedDate.DayOfWeek == DayOfWeek.Tuesday)
        {
            discountDetails.Add("Boeking op maandag of dinsdag: 15% korting");
            return 15m;
        }
        return 0m;
    }

    public decimal ApplyLetterDiscount(List<Animal> animals, List<string> discountDetails)
    {
        decimal discountPercentage = 0m;
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        foreach (var animal in animals)
        {
            foreach (char c in alphabet)
            {
                if (!animal.Name.Contains(c))
                {
                    break;
                }
                discountPercentage += 2m;
            }
        }
        if(discountPercentage > 0m)
        {
            discountDetails.Add("Dierenamen: " + discountPercentage);
        }

        return discountPercentage;
    }

    public decimal ApplyCustomerCardDiscount(Customer customer, List<string> discountDetails)
    {
        if (customer?.CustomerCard != CustomerCard.None)
        {
            discountDetails.Add("Klantenkaart: 10% korting");
            return 10m;
        }
        return 0m;
    }
}

