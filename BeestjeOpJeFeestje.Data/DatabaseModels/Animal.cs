using System.ComponentModel.DataAnnotations;

namespace BeestjeOpJeFeestje.Data.DatabaseModels
{
    public class Animal
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public AnimalType Type { get; set; }
        [Required]
        public Decimal Price { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public string? ImageUrl { get; set; }
    }
}
