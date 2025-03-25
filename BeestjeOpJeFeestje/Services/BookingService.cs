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
        List<string> restrictions = new List<string>();
        var animals = booking.animals;

        //Check for empty booking
        if (animals.Count == 0)
        {
            restrictions.Add("Je moet minimaal 1 beestje boeken.");
            return restrictions;
        }

        // Check for conflicting animal types
        bool hasFarmAnimal = animals.Any(a => a.Type == AnimalType.Farm);
        bool hasLionOrPolarBear = animals.Any(a => a.Name.Contains("Leeuw", StringComparison.OrdinalIgnoreCase) || a.Name.Contains("IJsbeer", StringComparison.OrdinalIgnoreCase));

        if (hasFarmAnimal && hasLionOrPolarBear)
        {
            restrictions.Add("Nom nom nom: Je mag geen beestje boeken met het type ‘Leeuw’ of ‘IJsbeer’ als je ook een beestje boekt van het type ‘Boerderijdier’.");
        }

        // Check for booking restrictions based on the day of the week
        bool isWeekend = booking.SelectedDate.DayOfWeek == DayOfWeek.Saturday || booking.SelectedDate.DayOfWeek == DayOfWeek.Sunday;
        bool hasPenguin = animals.Any(a => a.Name.Contains("Pinguïn", StringComparison.OrdinalIgnoreCase));

        if (isWeekend && hasPenguin)
        {
            restrictions.Add("Dieren in pak werken alleen doordeweeks: Je mag geen beestje boeken met de naam ‘Pinguïn’ in het weekend.");
        }

        // Check for booking restrictions based on the month
        int month = booking.SelectedDate.Month;
        bool hasDesertAnimal = animals.Any(a => a.Type == AnimalType.Desert);
        bool hasSnowAnimal = animals.Any(a => a.Type == AnimalType.Snow);

        if ((month >= 10 || month <= 2) && hasDesertAnimal)
        {
            restrictions.Add("Brrrr – Veelste koud: Je mag geen beestje boeken van het type ‘Woestijn’ in de maanden oktober t/m februari.");
        }

        if ((month >= 6 && month <= 8) && hasSnowAnimal)
        {
            restrictions.Add("Some People Are Worth Melting For. ~ Olaf: Je mag geen beestje boeken van het type ‘Sneeuw’ in de maanden juni t/m augustus.");
        }

        // Check for customer card restrictions
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
            restrictions.Add($"Je mag maximaal {maxAnimals} dieren boeken.");
        }

        if (customer.CustomerCard != CustomerCard.Platinum && animals.Any(a => a.Type == AnimalType.VIP))
        {
            restrictions.Add("Alleen klanten met een platina kaart mogen VIP dieren boeken.");
        }

        return restrictions;
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

