using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrezentacioniSloj.ViewModel
{
    public class TakmicenjeViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Datum takmicenja je obavezan")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DatumTakmicenja { get; set; }

        [Required]
        public string NazivPredmetaTakmicenja { get; set; } = string.Empty;

        [Required]
        public string NazivTakmicenja { get; set; } = string.Empty;

        [Required]
        public string TipTakmicenja { get; set; } = string.Empty;

        [Required]
        public string LokacijaTakmicenja { get; set; } = string.Empty;

    }
}
