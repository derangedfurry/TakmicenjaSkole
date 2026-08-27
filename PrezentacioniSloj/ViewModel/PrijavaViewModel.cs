using System.ComponentModel.DataAnnotations;

namespace PrezentacioniSloj.ViewModel
{
    public class PrijavaViewModel
    {
        [Required(ErrorMessage = "Polje nije popunjeno")]
        public string EmailIliKorisnickoIme { get; set; } = string.Empty;

        [Required(ErrorMessage = "Polje lozinke nije popunjeno")]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = string.Empty;
    }
}
