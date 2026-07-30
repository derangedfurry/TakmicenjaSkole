using SlojPodataka.Kontekst;
using System;
using System.Collections.Generic;
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

        public void Dodaj(SlojPodataka.Model.UcenikModel ucenikModel)
        {
            if (ucenikModel == null) return;
            kontekst.UcenikModelObjektiDBSet.Add(ucenikModel);
            kontekst.SaveChanges();
        }

        public void Izmeni(SlojPodataka.Model.UcenikModel ucenikModel)
        {
            if (ucenikModel == null) return;
            var ucenik = kontekst.UcenikModelObjektiDBSet.Find(ucenikModel.ID);
            if (ucenik == null) return;
            ucenik.Ime = ucenikModel.Ime;
            ucenik.Prezime = ucenikModel.Prezime;
            ucenik.BrojBodova = ucenikModel.BrojBodova;
            ucenik.IDTakmicenja = ucenikModel.IDTakmicenja;
            kontekst.SaveChanges();
        }

        public void Obrisi(int id)
        {
            var ucenik = kontekst.UcenikModelObjektiDBSet.Find(id);
            if (ucenik == null) return;
            kontekst.UcenikModelObjektiDBSet.Remove(ucenik);
            kontekst.SaveChanges();
        }

        public List<SlojPodataka.Model.UcenikModel> DajSveUcenike()
        {
            return kontekst.UcenikModelObjektiDBSet.ToList();
        }

        public List<SlojPodataka.Model.UcenikModel> DajSveUcenikePoTakmicenju(int idTakmicenja)
        {
            return kontekst.UcenikModelObjektiDBSet.Where(u => u.IDTakmicenja == idTakmicenja).ToList();
        }

    }
}
