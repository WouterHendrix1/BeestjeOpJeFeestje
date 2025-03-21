namespace BeestjeOpJeFeestje.Models
{
    public class SelectAnimalsViewModel
    {
        public DateTime SelectedDate { get; set; }
        public List<AnimalSelectionViewModel> Animals { get; set; } = new List<AnimalSelectionViewModel>();
    }
}
