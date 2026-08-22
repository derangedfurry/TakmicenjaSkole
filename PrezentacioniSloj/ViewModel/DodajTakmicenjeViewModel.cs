using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PrezentacioniSloj.ViewModel
{
    public class DodajTakmicenjeViewModel
    {
        public TakmicenjeViewModel Takmicenje { get; set; } = new();

        [ValidateNever]
        public List<PredmetViewModel> Predmet { get; set; } = new();

        public List<UcenikViewModel> Ucenik { get; set; } = new();

        [ValidateNever]
        public string selektovanPredmet { get; set; } = string.Empty;
    }
}
