using Microsoft.EntityFrameworkCore;
using SlojPodataka.Kontekst;
using SlojPodataka.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojPodataka.Repozitorijum
{
    public class UcenikRepo
    {
        private readonly AppDbContext kontekst;

        public UcenikRepo(AppDbContext kontekst)
        {
            this.kontekst = kontekst;
        }

        public async Task<List<UcenikModel>> DajSve()
        {

            List<UcenikModel> ucenici = await kontekst.UcenikModelObjektiDBSet
                .Select(u => new UcenikModel
                {
                    ID = u.ID,
                    SifraUcenika = u.SifraUcenika,
                    Ime = u.Ime,
                    Prezime = u.Prezime,
                    BrojBodova = u.BrojBodova,
                    IDTakmicenja = u.IDTakmicenja,
                    
                })
                .ToListAsync();

            return ucenici;
        }


        public async Task<UcenikModel> DajPoId(int id)
        {
            UcenikModel ucenikModel = await kontekst.UcenikModelObjektiDBSet.FindAsync(id);

            return ucenikModel;
        }


        public async void Dodaj(UcenikModel ucenikModel)
        {
            Debug.WriteLine("Dodavanje ucenika: " + ucenikModel.Ime + " " + ucenikModel.Prezime);

            UcenikModel ucenik = new UcenikModel
            {
                SifraUcenika = ucenikModel.SifraUcenika,
                Ime = ucenikModel.Ime,
                Prezime = ucenikModel.Prezime,
                BrojBodova = ucenikModel.BrojBodova,
                IDTakmicenja = ucenikModel.IDTakmicenja
            };


            kontekst.UcenikModelObjektiDBSet.Add(ucenik);
        }

        public async void Izmeni(UcenikModel ucenik, UcenikModel ucenikSaIzmenama)
        {
            ucenik.ID = ucenikSaIzmenama.ID;
            ucenik.SifraUcenika = ucenikSaIzmenama.SifraUcenika;
            ucenik.Ime = ucenikSaIzmenama.Ime;
            ucenik.Prezime = ucenikSaIzmenama.Prezime;
            ucenik.BrojBodova = ucenikSaIzmenama.BrojBodova;
            ucenik.IDTakmicenja = ucenikSaIzmenama.IDTakmicenja;
        }

        public async void Obrisi(UcenikModel ucenik)
        {

            kontekst.UcenikModelObjektiDBSet.Remove(ucenik);
        }

        public async Task<List<UcenikModel>> DajPoTakmicenjuId(int id)
        {
            List<UcenikModel> ucenici = await kontekst.UcenikModelObjektiDBSet
                .Where(u => u.IDTakmicenja == id)
                .Select(u => new UcenikModel
                {
                    ID = u.ID,
                    SifraUcenika = u.SifraUcenika,
                    Ime = u.Ime,
                    Prezime = u.Prezime,
                    BrojBodova = u.BrojBodova,
                    IDTakmicenja = u.IDTakmicenja
                })
                .ToListAsync();

            return ucenici;
        }
        public bool Postoji(int id)
        {
            return kontekst.UcenikModelObjektiDBSet.Any(e => e.ID == id);
        }
    }
}
