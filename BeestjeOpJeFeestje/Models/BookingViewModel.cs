using BeestjeOpJeFeestje.Data.DatabaseModels;

namespace BeestjeOpJeFeestje.Models
{
    public class BookingViewModel
    {
        public DateTime SelectedDate { get; set; }
        public List<int> AnimalIds { get; set; } = new List<int>();

        public List<Animal> animals { get; set; } = new List<Animal>();

        public PersonalDataViewModel? PersonalData { get; set; }

        public decimal TotalPrice { get; set; } = 0;

        public List<string> Discounts { get; set; } = new List<string>();  

    }
}
