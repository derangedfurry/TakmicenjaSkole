using BibliotekaKlasa.TehnoloskeKlase;
using SlojPodataka.Kontekst;
using SlojPodataka.Model;
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
        private UcenikRepo ucenikRepo;
        private KonekcijaKlasa konekcija;

        public DiplomaPoslovnaLogika(KonekcijaKlasa konekcija, AppDbContext appDbContext)
        {
            this.nagrade = new List<Nagrada>();
            this.konekcija = konekcija;

            this.diplomaRepo = new DiplomaRepo(konekcija);
            this.ucenikRepo = new UcenikRepo(appDbContext);
            UcitajXMLOgranicenja();
        }

        public DiplomaPoslovnaLogika(AppDbContext appDbContext)
        {
            KonekcijaKlasa konekcija = new KonekcijaKlasa("Server=(localdb)\\mssqllocaldb;Database=SkolaTakmicenja;Trusted_Connection=True;MultipleActiveResultSets=true");
            this.nagrade = new List<Nagrada>();
            this.konekcija = konekcija;
            this.ucenikRepo = new UcenikRepo(appDbContext);
            this.diplomaRepo = new DiplomaRepo(konekcija);
            UcitajXMLOgranicenja();
        }

        private void UcitajXMLOgranicenja()
        {
            XDocument doc = XDocument.Load(putanja);

            Debug.WriteLine(doc.ToString());

            doc.Descendants("Nagrada")
                .ToList()
                .ForEach(x =>
                {
                    Nagrada nagrada = new Nagrada
                    {
                        BrojNagrade = int.Parse(x.Element("ID")?.Value ?? "0"),
                        NazivNagrade = x.Element("Naziv")?.Value,
                        MaksimumBrojBodova = int.Parse(x.Element("Maksimum")?.Value ?? "0"),
                        MinimumBrojBodova = int.Parse(x.Element("Minimum")?.Value ?? "0")
                    };


                    /*Debug.WriteLine($"Broj nagrade: {nagrada.BrojNagrade}, " +
                        $"Naziv nagrade: {nagrada.NazivNagrade}" +
                        $", Maksimum bodova: {nagrada.MaksimumBrojBodova}, " +
                        $"Minimum bodova: {nagrada.MinimumBrojBodova}");
                    */
                    nagrade.Add(nagrada);
                });
        }

        public void GenerisiDiplomu(int brojBodova,int IDucenika)
        {
            Debug.WriteLine("Generiasnje  diploma");
            Nagrada nagrada = nagrade.FirstOrDefault(n => n.MinimumBrojBodova <= brojBodova && n.MaksimumBrojBodova >= brojBodova);
            if (nagrada != null)
            {
                Debug.WriteLine($"Ucenik je osvojio nagradu: {nagrada.NazivNagrade}");
                Debug.WriteLine("UcenikID = " + IDucenika);
                diplomaRepo.Dodaj(new DiplomaModel
                {
                    IDUcenika = IDucenika,
                    Nagrada = nagrada.BrojNagrade
                   
                });
            }
            else
            {
                Debug.WriteLine("Ucenik nije osvojio nagradu.");
            }
        }
        public async Task ProveriDiplome()
        {
            Debug.WriteLine("Provera diploma");

            List<UcenikModel> ucenici = await ucenikRepo.DajSve();
            Debug.WriteLine($"Učitano učenika: {ucenici.Count}");

            foreach (UcenikModel ucenik in ucenici)
            {
                // Find matching award for this score (inclusive range)
                Nagrada? nagrada = nagrade.FirstOrDefault(n =>
                    ucenik.BrojBodova >= n.MinimumBrojBodova &&
                    ucenik.BrojBodova <= n.MaksimumBrojBodova);

                if (nagrada == null)
                {
                    Debug.WriteLine($"Učenik {ucenik.ID} ({ucenik.BrojBodova} bodova) – nema nagrade.");
                    continue;
                }

                // Check if diploma already exists for this učenik
                DiplomaModel postojeca = diplomaRepo.DajSve().FirstOrDefault(d => d.IDUcenika == ucenik.ID);
                // If you don't have DajPoUceniku, use something like:


                if (postojeca != null)
                {
                    // Update existing diploma
                    if (postojeca.Nagrada != nagrada.BrojNagrade)
                    {
                        postojeca.Nagrada = nagrada.BrojNagrade;
                        diplomaRepo.Izmeni(postojeca);   // or Update / Save

                        Debug.WriteLine(
                            $"Ažurirana diploma ID={postojeca.ID}: Učenik={ucenik.ID}, " +
                            $"nova nagrada={nagrada.BrojNagrade} ({nagrada.NazivNagrade})");
                    }
                    else
                    {
                        Debug.WriteLine(
                            $"Diploma za učenika {ucenik.ID} već postoji sa istom nagradom ({nagrada.NazivNagrade}).");
                    }
                }
                else
                {
                    // Create new diploma
                    diplomaRepo.Dodaj(new DiplomaModel
                    {
                        IDUcenika = ucenik.ID,
                        Nagrada = nagrada.BrojNagrade
                    });

                    Debug.WriteLine(
                        $"Dodata diploma: Učenik={ucenik.ID}, nagrada={nagrada.BrojNagrade} ({nagrada.NazivNagrade})");
                }
            }
        }

        public List<Nagrada> DajSveNagrade()
        {
            return nagrade;
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
