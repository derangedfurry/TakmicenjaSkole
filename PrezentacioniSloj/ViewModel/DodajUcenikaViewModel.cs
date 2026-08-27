namespace PrezentacioniSloj.ViewModel
{
    public class DodajUcenikaViewModel
    {
        public UcenikViewModel Ucenik { get; set; } = new();
        public List<TakmicenjeViewModel> Takmicenje { get; set; } = new();

    }
}
