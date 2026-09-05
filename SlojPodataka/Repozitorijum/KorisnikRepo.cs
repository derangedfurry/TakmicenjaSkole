
using BibliotekaKlasa.TehnoloskeKlase;
using BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije;
using Microsoft.EntityFrameworkCore;
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
            //kontekst.SaveChanges();
        }

        public void Obrisi(int id)
        {
            var korisnik = kontekst.KorisnikModelObjektiDBSet.Find(id);
            if (korisnik == null) return;
            kontekst.KorisnikModelObjektiDBSet.Remove(korisnik);
            //kontekst.SaveChanges();
        }

        public void Izmeni(int id, KorisnikModel korisnikModel)
        {
            if (korisnikModel == null) return;
            var korisnik = kontekst.KorisnikModelObjektiDBSet.Find(id);
            if (korisnik == null) return;

            korisnik.KorisnickoIme = korisnikModel.KorisnickoIme;
            korisnik.Ime = korisnikModel.Ime;
            korisnik.Prezime = korisnikModel.Prezime;
            korisnik.Email = korisnikModel.Email;
            korisnik.Uloga = korisnikModel.Uloga;
            korisnik.PasswordHash = korisnikModel.PasswordHash;
            korisnik.PasswordSalt = korisnikModel.PasswordSalt;

            kontekst.KorisnikModelObjektiDBSet.Update(korisnik);

            //kontekst.SaveChanges();
        }

        public async Task<List<KorisnikModel>> DajSve()
        {
            List<KorisnikModel> korisnici = await kontekst.KorisnikModelObjektiDBSet
                .Select(k => new KorisnikModel
                {
                    ID = k.ID,
                    Email = k.Email,
                    Ime = k.Ime,
                    Prezime = k.Prezime,
                    KorisnickoIme = k.KorisnickoIme,
                    PasswordHash = k.PasswordHash,
                    PasswordSalt = k.PasswordSalt,
                    Uloga = k.Uloga,
                    
                })
                .ToListAsync();

            return korisnici;
        }

        public async Task<KorisnikModel> DajPoId(int id)
        {
            KorisnikModel korisnik = await kontekst.KorisnikModelObjektiDBSet.FindAsync(id);
            return korisnik;
        }

        public bool DaLiPostojiKorisnikSaEmailom(string email)
        {
            return kontekst.KorisnikModelObjektiDBSet.Any(k => k.Email == email);
        }

        public bool Postoji(int id)
        {
            return kontekst.KorisnikModelObjektiDBSet.Any(e => e.ID == id);
        }
    }
}
