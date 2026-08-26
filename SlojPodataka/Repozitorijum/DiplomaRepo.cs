using BibliotekaKlasa.TehnoloskeKlase;
using Microsoft.Data.SqlClient;
using SlojPodataka.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojPodataka.Repozitorijum
{
    public class DiplomaRepo : TabelaKlasa
    {
        private KonekcijaKlasa konekcija {get; set; }

        public DiplomaRepo(KonekcijaKlasa konekcija) : base(konekcija, "Diploma")
        {
            this.konekcija = konekcija;
        }

        public bool Dodaj(DiplomaModel diplomaModel)
        {
            konekcija.OtvoriKonekciju();
            string upit = "INSERT INTO Diploma (IDUcenika, Nagrada) VALUES (@IDUcenika, @Nagrada)";

            using SqlCommand komanda = new SqlCommand(upit, konekcija.DajKonekciju());
            komanda.Parameters.AddWithValue("@IDUcenika", diplomaModel.IDUcenika);
            komanda.Parameters.AddWithValue("@Nagrada", diplomaModel.Nagrada);
            komanda.ExecuteNonQuery();
            konekcija.ZatvoriKonekciju();
            return true;
        }

        public bool Obrisi(int id)
        {
            konekcija.OtvoriKonekciju();
            string upit = "DELETE FROM Diploma WHERE ID = @ID";
            using SqlCommand komanda = new SqlCommand(upit, konekcija.DajKonekciju());
            komanda.Parameters.AddWithValue("@ID", id);
            komanda.ExecuteNonQuery();
            konekcija.ZatvoriKonekciju();
            return true;
        }

        public bool Izmeni(int id,DiplomaModel diplomaModel)
        {
            konekcija.OtvoriKonekciju();
            string upit = "UPDATE Diploma SET IDUcenika = @IDUcenika, Nagrada = @Nagrada WHERE ID = @ID";
            using SqlCommand komanda = new SqlCommand(upit, konekcija.DajKonekciju());
            komanda.Parameters.AddWithValue("@ID", id);
            komanda.Parameters.AddWithValue("@IDUcenika", diplomaModel.IDUcenika);
            komanda.Parameters.AddWithValue("@Nagrada", diplomaModel.Nagrada);
            komanda.ExecuteNonQuery();
            konekcija.ZatvoriKonekciju();
            return true;
        }

        /*var diplome = await _context.DiplomaModelObjektiDBSet
    .Select(d => new { d.ID, d.IDUcenika, d.Nagrada })
    .ToListAsync();*/
        public List<DiplomaModel> DajSve()
        {
            List<DiplomaModel> diplome = new List<DiplomaModel>();

            konekcija.OtvoriKonekciju();
            string upit = "SELECT * FROM Diploma";
            using SqlCommand komanda = new SqlCommand(upit, konekcija.DajKonekciju());
            using SqlDataReader reader = komanda.ExecuteReader();
            while (reader.Read())
            {
                diplome.Add(new DiplomaModel
                {
                    ID = reader.GetInt32(0),
                    IDUcenika = reader.GetInt32(1),
                    Nagrada = reader.GetInt32(2)
                });

                int id = reader.GetInt32(0);
                int ucenikId = reader.GetInt32(1);
                int takmicenjeId = reader.GetInt32(2);
                Console.WriteLine($"ID: {id}, IDUcenika: {ucenikId}, Nagrada: {takmicenjeId}");
            }
            konekcija.ZatvoriKonekciju();
            return diplome;
        }


        public DiplomaModel DajPoId(int id)
        {
            DiplomaModel diploma = new DiplomaModel();
            konekcija.OtvoriKonekciju();
            string upit = "SELECT * FROM Diploma WHERE ID = @ID";
            using SqlCommand komanda = new SqlCommand(upit, konekcija.DajKonekciju());
            komanda.Parameters.AddWithValue("@ID", id);
            using SqlDataReader reader = komanda.ExecuteReader();
            while (reader.Read())
            {
                diploma.ID = reader.GetInt32(0);
                diploma.IDUcenika = reader.GetInt32(1);
                diploma.Nagrada = reader.GetInt32(2);
            }
            konekcija.ZatvoriKonekciju();
            return diploma;
        }

        public DiplomaModel DajPoUceniku(int ucenikId)
        {
            DiplomaModel diploma = new DiplomaModel();
            konekcija.OtvoriKonekciju();
            string upit = "SELECT * FROM Diploma WHERE IDUcenika = @IDUcenika";
            using SqlCommand komanda = new SqlCommand(upit, konekcija.DajKonekciju());
            komanda.Parameters.AddWithValue("@IDUcenika", ucenikId);
            using SqlDataReader reader = komanda.ExecuteReader();
            while (reader.Read())
            {
                diploma.ID = reader.GetInt32(0);
                diploma.IDUcenika = reader.GetInt32(1);
                diploma.Nagrada = reader.GetInt32(2);
            }
            konekcija.ZatvoriKonekciju();
            return diploma;
        }

        public bool Postoji(int id)
        {
            DiplomaModel diploma = DajPoId(id);

            if (diploma != null)
            {
                return true;
            }
            else return false;

        }

    }
}
