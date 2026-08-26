namespace PrezentacioniSloj.ViewModel
{
    public class DiplomaViewModel
    {
        public int ID { get; set; }
        public int Nagrada { get; set; }
        public string NazivNagrade { get; set; } = string.Empty;
        public string ImeUcenika { get; set; } = string.Empty;
        public string PrezimeUcenika { get; set; } = string.Empty;
        public int brojBodova { get; set; }
        public int IDUcenika { get; set; }
    }
}
