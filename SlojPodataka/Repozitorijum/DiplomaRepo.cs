using BibliotekaKlasa.TehnoloskeKlase;
using SlojPodataka.Model;
using System.Data;

namespace SlojPodataka.Repozitorijum
{
    public class DiplomaRepo : TabelaKlasa
    {
        private readonly KonekcijaKlasa _konekcija;

        public DiplomaRepo(KonekcijaKlasa konekcija) : base(konekcija, "Diploma")
        {
            _konekcija = konekcija;
        }

        public bool Dodaj(DiplomaModel diplomaModel)
        {
            string upit =
                $"INSERT INTO Diploma (IDUcenika, Nagrada) " +
                $"VALUES ({diplomaModel.IDUcenika}, {diplomaModel.Nagrada})";

            return IzvrsiAzuriranje(upit);
        }

        public bool Obrisi(int id)
        {
            string upit = $"DELETE FROM Diploma WHERE ID = {id}";
            return IzvrsiAzuriranje(upit);
        }

        public bool Izmeni(int id, DiplomaModel diplomaModel)
        {
            string upit =
                $"UPDATE Diploma SET " +
                $"IDUcenika = {diplomaModel.IDUcenika}, " +
                $"Nagrada = {diplomaModel.Nagrada} " +
                $"WHERE ID = {id}";

            return IzvrsiAzuriranje(upit);
        }

        public List<DiplomaModel> DajSve()
        {
            List<DiplomaModel> diplome = new List<DiplomaModel>();

            DataSet ds = DajPodatke("SELECT ID, IDUcenika, Nagrada FROM Diploma");
            DataTable tabela = ds.Tables[0];

            foreach (DataRow red in tabela.Rows)
            {
                diplome.Add(MapirajRed(red));
            }

            return diplome;
        }

        public DiplomaModel? DajPoId(int id)
        {
            DataSet ds = DajPodatke(
                $"SELECT ID, IDUcenika, Nagrada FROM Diploma WHERE ID = {id}");

            DataTable tabela = ds.Tables[0];
            if (tabela.Rows.Count == 0)
                return null;

            return MapirajRed(tabela.Rows[0]);
        }

        public DiplomaModel? DajPoUceniku(int ucenikId)
        {
            DataSet ds = DajPodatke(
                $"SELECT ID, IDUcenika, Nagrada FROM Diploma WHERE IDUcenika = {ucenikId}");

            DataTable tabela = ds.Tables[0];
            if (tabela.Rows.Count == 0)
                return null;

            return MapirajRed(tabela.Rows[0]);
        }

        public bool Postoji(int id)
        {
            return DajPoId(id) != null;
        }

        private static DiplomaModel MapirajRed(DataRow red)
        {
            return new DiplomaModel
            {
                ID = Convert.ToInt32(red["ID"]),
                IDUcenika = Convert.ToInt32(red["IDUcenika"]),
                Nagrada = Convert.ToInt32(red["Nagrada"])
            };
        }
    }
}