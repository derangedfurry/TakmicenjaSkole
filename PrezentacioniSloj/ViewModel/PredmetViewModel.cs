using System.ComponentModel.DataAnnotations;

namespace PrezentacioniSloj.ViewModel
{
    public class PredmetViewModel
    {
        [Required(ErrorMessage = "Id predmeta je obavezan")]
        [MaxLength(5, ErrorMessage = "ID predmeta ne sme biti duži od 5 karaktera")]
        [MinLength(5, ErrorMessage = "ID predmeta ne sme biti kraći od 5 karaktera")]
        public string ID { get; set; } = string.Empty;

        [Required(ErrorMessage = "Naziv predmeta je obavezan")]
        public string NazivPredmeta { get; set; } = string.Empty;
    }
}
