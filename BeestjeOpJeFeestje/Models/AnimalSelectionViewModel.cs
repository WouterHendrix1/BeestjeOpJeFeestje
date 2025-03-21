using BeestjeOpJeFeestje.Data;

namespace BeestjeOpJeFeestje.Models
{
    public class AnimalSelectionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AnimalType Type { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsBooked { get; set; }
        public bool IsSelected { get; set; }
    }
}
