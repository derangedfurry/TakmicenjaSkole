
using BibliotekaKlasa.TehnoloskeKlase;
using BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije;
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

            if(korisnik.PasswordHash != korisnikModel.PasswordHash || korisnik.PasswordSalt != korisnikModel.PasswordSalt)
            {
                korisnik.PasswordHash = korisnikModel.PasswordHash;
                korisnik.PasswordSalt = korisnikModel.PasswordSalt;
            }

            kontekst.KorisnikModelObjektiDBSet.Update(korisnik);

            kontekst.SaveChanges();
        }

        public List<KorisnikModel> DajSveKorisnike()
        {
            return kontekst.KorisnikModelObjektiDBSet.ToList();
        }

        /*public KorisnikModel DajKorisnikaPoEmailuISifri(string email, string sifra)
        {
            if (!DaLiPostojiKorisnikSaEmailom(email))
                return null;

            KorisnikModel korisnik = kontekst.KorisnikModelObjektiDBSet.First(k => k.Email == email);

                bool provera = FunkcijeLozinke.ProveriLozinku(sifra, korisnik.PasswordSalt.ToString(), korisnik.PasswordHash.ToString());

                if (provera)
                {
                    return korisnik;
                }
                else
                {
                    return null;
                }
        }*/

        public bool DaLiPostojiKorisnikSaEmailom(string email)
        {
            return kontekst.KorisnikModelObjektiDBSet.Any(k => k.Email == email);
        }

    }
}
