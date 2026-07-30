
using BibliotekaKlasa.TehnoloskeKlase;
using SlojPodataka.Kontekst;
using SlojPodataka.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojPodataka.Repozitorijum
{
    public class KorisnikRepo
    {
        private readonly AppDbContext kontekst;
        public KorisnikRepo(AppDbContext kontekst)
        {
            this.kontekst = kontekst;
        }

        public void Dodaj(KorisnikModel korisnikModel)
        {
            if (korisnikModel == null) return;

            kontekst.KorisnikModelObjektiDBSet.Add(korisnikModel);
            kontekst.SaveChanges();
        }

        public void Obrisi(int id)
        {
            var korisnik = kontekst.KorisnikModelObjektiDBSet.Find(id);
            if (korisnik == null) return;
            kontekst.KorisnikModelObjektiDBSet.Remove(korisnik);
            kontekst.SaveChanges();
        }

        public void Izmeni(KorisnikModel korisnikModel)
        {
            if (korisnikModel == null) return;
            var korisnik = kontekst.KorisnikModelObjektiDBSet.Find(korisnikModel.ID);
            if (korisnik == null) return;
            korisnik.KorisnickoIme = korisnikModel.KorisnickoIme;
           // korisnik.Lozinka = korisnikModel.Lozinka;
            kontekst.SaveChanges();
        }

        public List<KorisnikModel> DajSveKorisnike()
        {
            return kontekst.KorisnikModelObjektiDBSet.ToList();
        }

        /*public KorisnikModel DajKorisnikaPoEmailuISifri(string email, string sifra)
        {
            return kontekst.KorisnikModelObjektiDBSet.FirstOrDefault(k => k.KorisnickoIme == email && k. == sifra);
        }*/



    }
}
