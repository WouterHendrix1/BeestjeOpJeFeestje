using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeestjeOpJeFeestje.Data.DatabaseModels
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; } // Feestdatum

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public ICollection<Animal> Animals { get; set; } = new List<Animal>();

        [Required]
        [Range(0.01, 5000.00)]
        public decimal TotalPrice { get; set; }

        public bool IsConfirmed { get; set; } = false; // Stap-voor-stap bevestigen van boeking
    }

}
