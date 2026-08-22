using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrezentacioniSloj.ViewModel
{
    public class UcenikViewModel
    {

        public int ID { get; set; }

        [Required(ErrorMessage = "Sifra ucenika je obavezna")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Sifra ucenika mora imati tacno 5 karaktera.")]
        public string SifraUcenika { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ime ucenika je obavezno")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime ucenika je obavezno")]
        public string Prezime { get; set; } = string.Empty;

        [Required]
        public int BrojBodova { get; set; } = 0;

        public int IDTakmicenja { get; set; }
    }
}
