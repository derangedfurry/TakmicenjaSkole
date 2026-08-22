using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrezentacioniSloj.ViewModel
{
    public class KorisnikViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Ime je obavezno")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno")]
        public string Prezime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Korisnicko ime je obavezno")]
        public string KorisnickoIme { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email je obavezan")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Lozinka mora biti duža od 6 karaktera")]
        public string Lozinka { get; set; } = string.Empty;

        public string Uloga { get; set; } = "Korisnik";

    }


}
