using System.ComponentModel.DataAnnotations;

namespace PrezentacioniSloj.ViewModel
{
    public class RegistracijaViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Ime je obavezno")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno")]
        public string Prezime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Korisnicko ime je obavezno")]
        public string KorisnickoIme { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", ErrorMessage = "Email nije u ispravnom formatu")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Lozinka mora biti duža od 6 karaktera")]
        public string Lozinka { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrdite lozinku")]
        [DataType(DataType.Password)]
        [Compare("Lozinka", ErrorMessage = "Lozinka se ne podudara")]
        public string LozinkaPotvrda { get; set; } = string.Empty;

    }
}
