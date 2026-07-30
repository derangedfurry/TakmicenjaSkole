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
    public class DiplomaRepo
    {
        private KonekcijaKlasa konekcija {get; set; }

        public DiplomaRepo(KonekcijaKlasa konekcija)
        {
            this.konekcija = konekcija;
        }

        public bool Dodaj(DiplomaModel diplomaModel)
        {
            konekcija.OtvoriKonekciju();
            string upit = "INSERT INTO Diploma (UcenikId, TakmicenjeId) VALUES (@UcenikId, @TakmicenjeId)";

            using SqlCommand komanda = new SqlCommand(upit, konekcija.DajKonekciju());
            komanda.Parameters.AddWithValue("@UcenikId", diplomaModel.IDUcenika);
            komanda.Parameters.AddWithValue("@TakmicenjeId", diplomaModel.Nagrada);
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

        public bool Izmeni(DiplomaModel diplomaModel)
        {
            konekcija.OtvoriKonekciju();
            string upit = "UPDATE Diploma SET UcenikId = @UcenikId, TakmicenjeId = @TakmicenjeId WHERE ID = @ID";
            using SqlCommand komanda = new SqlCommand(upit, konekcija.DajKonekciju());
            komanda.Parameters.AddWithValue("@ID", diplomaModel.ID);
            komanda.Parameters.AddWithValue("@UcenikId", diplomaModel.IDUcenika);
            komanda.Parameters.AddWithValue("@TakmicenjeId", diplomaModel.Nagrada);
            komanda.ExecuteNonQuery();
            konekcija.ZatvoriKonekciju();
            return true;
        }

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
                Console.WriteLine($"ID: {id}, UcenikId: {ucenikId}, TakmicenjeId: {takmicenjeId}");
            }
            konekcija.ZatvoriKonekciju();
            return diplome;
        }

        public List<DiplomaModel> DajPoUceniku(int ucenikId)
        {
            List<DiplomaModel> diplome = new List<DiplomaModel>();
            konekcija.OtvoriKonekciju();
            string upit = "SELECT * FROM Diploma WHERE UcenikId = @UcenikId";
            using SqlCommand komanda = new SqlCommand(upit, konekcija.DajKonekciju());
            komanda.Parameters.AddWithValue("@UcenikId", ucenikId);
            using SqlDataReader reader = komanda.ExecuteReader();
            while (reader.Read())
            {
                diplome.Add(new DiplomaModel
                {
                    ID = reader.GetInt32(0),
                    IDUcenika = reader.GetInt32(1),
                    Nagrada = reader.GetInt32(2)
                });
            }
            konekcija.ZatvoriKonekciju();
            return diplome;
        }

    }
}
