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
    public class PredmetRepo
    {
        private KonekcijaKlasa konekcija { get; set; }
        public PredmetRepo(KonekcijaKlasa konekcija)
        {
            this.konekcija = konekcija;
        }

        public void Dodaj(PredmetModel predmetModel)
        {
            konekcija.OtvoriKonekciju();

            using SqlCommand komanda = new SqlCommand("DodajPredmet", konekcija.DajKonekciju());
            komanda.CommandType = System.Data.CommandType.StoredProcedure;
            komanda.Parameters.AddWithValue("@NazivPredmeta", predmetModel.NazivPredmeta);
            komanda.ExecuteNonQuery();

            // add check for the result of the stored procedure execution

            konekcija.ZatvoriKonekciju();
        }

        public void Obrisi(string id)
        {
            konekcija.OtvoriKonekciju();
            using SqlCommand komanda = new SqlCommand("ObrisiPredmet", konekcija.DajKonekciju());
            komanda.Parameters.AddWithValue("@ID", id);
            komanda.ExecuteNonQuery();
            konekcija.ZatvoriKonekciju();

            // add check for the result of the stored procedure execution

        }

        public void Izmeni(string id, PredmetModel predmetModel)
        {
            konekcija.OtvoriKonekciju();
            using SqlCommand komanda = new SqlCommand("IzmeniPredmet", konekcija.DajKonekciju());
            komanda.CommandType = System.Data.CommandType.StoredProcedure;
            komanda.Parameters.AddWithValue("@Id", id);
            komanda.Parameters.AddWithValue("@NazivPredmeta", predmetModel.NazivPredmeta);
            komanda.ExecuteNonQuery();
            konekcija.ZatvoriKonekciju();
            // add check for the result of the stored procedure execution
        }

        public List<PredmetModel> DajSve()
        {
            List<PredmetModel> predmeti = new List<PredmetModel>();

            konekcija.OtvoriKonekciju();
            using SqlCommand komanda = new SqlCommand("Select * from Predmet", konekcija.DajKonekciju());
            using SqlDataReader reader = komanda.ExecuteReader();
            while (reader.Read())
            {
                predmeti.Add(new PredmetModel
                {
                    ID = reader.GetString(0),
                    NazivPredmeta = reader.GetString(1)
                });

                string id = reader.GetString(0);
                string nazivPredmeta = reader.GetString(1);
                Console.WriteLine($"ID: {id}, Naziv predmeta: {nazivPredmeta}");
            }
            konekcija.ZatvoriKonekciju();

            return predmeti;
        }

        public PredmetModel DajPredmetPoID(string id)
        {
            PredmetModel predmet = null;
            konekcija.OtvoriKonekciju();
            using SqlCommand komanda = new SqlCommand("Select * from Predmet where ID = @ID", konekcija.DajKonekciju());
            komanda.Parameters.AddWithValue("@ID", id);
            using SqlDataReader reader = komanda.ExecuteReader();
            if (reader.Read())
            {
                predmet = new PredmetModel
                {
                    ID = reader.GetString(0),
                    NazivPredmeta = reader.GetString(1)
                };
            }
            konekcija.ZatvoriKonekciju();
            return predmet;
        }
    }
}
