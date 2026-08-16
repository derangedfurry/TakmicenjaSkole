using BibliotekaKlasa.TehnoloskeKlase;
using SlojPodataka.Repozitorijum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SlojPoslovneLogike
{
    public class DiplomaPoslovnaLogika
    {
        private readonly string putanja = "D:\\fakultet\\VISESLOJNI\\Projekat2\\TakmicenjaSkole\\SlojPoslovneLogike\\Ogranicenja\\Ogranicenja.xml";

        private List<Nagrada> nagrade;
        private DiplomaRepo diplomaRepo;
        private KonekcijaKlasa konekcija;

        public DiplomaPoslovnaLogika(KonekcijaKlasa konekcija)
        {
            this.nagrade = new List<Nagrada>();
            this.konekcija = konekcija;

            this.diplomaRepo = new DiplomaRepo(konekcija);
            UcitajXMLOgranicenja();
        }

        private void UcitajXMLOgranicenja()
        {
            XDocument doc = XDocument.Load(putanja);

            doc.Descendants("Nagrade")
                .ToList()
                .ForEach(x =>
                {
                    Nagrada nagrada = new Nagrada
                    {
                        BrojNagrade = int.Parse(x.Element("Broj")?.Value ?? "0"),
                        NazivNagrade = x.Element("Naziv")?.Value,
                        MaksimumBrojBodova = int.Parse(x.Element("Maksimum")?.Value ?? "0"),
                        MinimumBrojBodova = int.Parse(x.Element("Minimum")?.Value ?? "0")
                    };

                    int BrojNagrade = int.Parse(x.Element("Broj")?.Value ?? "0");
                    string NazivNagrade = x.Element("Naziv")?.Value;
                    int MaksimumBrojBodova = int.Parse(x.Element("Maksimum")?.Value ?? "0");
                    int MinimumBrojBodova = int.Parse(x.Element("Minimum")?.Value ?? "0");

                    Debug.Write($"Broj nagrade: {BrojNagrade}, Naziv nagrade: {NazivNagrade}, Maksimum bodova: {MaksimumBrojBodova}, Minimum bodova: {MinimumBrojBodova}");
                });

        }

        public void GenerisiDiplomu(int brojBodova)
        {
            var nagrada = nagrade.FirstOrDefault(n => brojBodova >= n.MinimumBrojBodova && brojBodova <= n.MaksimumBrojBodova);
            if (nagrada != null)
            {
                Debug.WriteLine($"Ucenik je osvojio nagradu: {nagrada.NazivNagrade}");
                diplomaRepo.Dodaj(new SlojPodataka.Model.DiplomaModel
                {
                    IDUcenika = 1, // Primer ID učenika
                    Nagrada = nagrada.BrojNagrade
                });
            }
            else
            {
                Debug.WriteLine("Ucenik nije osvojio nagradu.");
            }
        }
    }



    public class Nagrada
    {
        public int BrojNagrade { get; set; }
        public string NazivNagrade { get; set; }
        public int MaksimumBrojBodova { get; set; }
        public int MinimumBrojBodova { get; set; }
    }
}
