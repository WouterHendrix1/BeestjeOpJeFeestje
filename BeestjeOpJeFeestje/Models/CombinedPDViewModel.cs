namespace BeestjeOpJeFeestje.Models
{
    public class CombinedPDViewModel
    {       
        public PersonalDataViewModel PersonalDataViewModel { get; set; } = new();

        public LoginViewModel LoginViewModel { get; set; } = new();
    }
}
