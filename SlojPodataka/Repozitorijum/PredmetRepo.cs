using BibliotekaKlasa.TehnoloskeKlase;
using Microsoft.Data.SqlClient;
using SlojPodataka.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            komanda.Parameters.AddWithValue("@ID", predmetModel.ID);

            komanda.Parameters.AddWithValue("@NazivPredmeta", predmetModel.NazivPredmeta);
            komanda.ExecuteNonQuery();


            konekcija.ZatvoriKonekciju();
        }

        public void Obrisi(string id)
        {
            konekcija.OtvoriKonekciju();
            using SqlCommand komanda = new SqlCommand("ObrisiPredmet", konekcija.DajKonekciju());
            komanda.CommandType = System.Data.CommandType.StoredProcedure;
            komanda.Parameters.AddWithValue("@Id", id);
            komanda.ExecuteNonQuery();
            konekcija.ZatvoriKonekciju();


        }

        public void Izmeni(string id, PredmetModel predmetModel)
        {
            Debug.WriteLine($"IDpredmeta za izmenu = {id}+ predmetModelID {predmetModel.ID} + predmet naziv {predmetModel.NazivPredmeta}");
            konekcija.OtvoriKonekciju();
            using SqlCommand komanda = new SqlCommand("IzmeniPredmet", konekcija.DajKonekciju());
            komanda.CommandType = System.Data.CommandType.StoredProcedure;
            komanda.Parameters.AddWithValue("@Id", id);
            komanda.Parameters.AddWithValue("@NazivPredmeta", predmetModel.NazivPredmeta);
            int odgovor = komanda.ExecuteNonQuery();
            if(odgovor > 0)
            {
                Debug.WriteLine("uspesno izmenjen predmet");
            } else
            {
                Debug.WriteLine("predmet nije pronadjen");
            }
                konekcija.ZatvoriKonekciju();
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
        public PredmetModel DajPredmetPoNazivu(string naziv)
        {
            PredmetModel predmet = null;
            konekcija.OtvoriKonekciju();
            using SqlCommand komanda = new SqlCommand("DajPredmetPoNazivu", konekcija.DajKonekciju());
            komanda.CommandType = System.Data.CommandType.StoredProcedure;
            komanda.Parameters.AddWithValue("@NazivPredmeta", naziv);
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

        public bool Postoji(string id)
        {
            konekcija.OtvoriKonekciju();
            using SqlCommand komanda = new SqlCommand("Select count(*) from Predmet where ID = @ID", konekcija.DajKonekciju());
            komanda.Parameters.AddWithValue("@ID", id);
            int count = (int)komanda.ExecuteScalar();
            konekcija.ZatvoriKonekciju();
            return count > 0;
        }
    }
}
