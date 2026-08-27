using SlojPodataka.Kontekst;
using SlojPodataka.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojPodataka.Repozitorijum
{
    public class TakmicenjeRepo
    {
        private readonly AppDbContext kontekst;

        public TakmicenjeRepo(AppDbContext kontekst)
        {
            this.kontekst = kontekst;
        }

        public void Dodaj(TakmicenjeModel takmicenjeModel)
        {
            if (takmicenjeModel == null) return;
            kontekst.TakmicenjaModelObjektiDBSet.Add(takmicenjeModel);
            //kontekst.SaveChanges();
        }

        public void Izmeni(int id, TakmicenjeModel takmicenjeModel)
        {
            if (takmicenjeModel == null) return;
            var takmicenje = kontekst.TakmicenjaModelObjektiDBSet.Find(id);
            if (takmicenje == null) return;
            takmicenje.DatumTakmicenja = takmicenjeModel.DatumTakmicenja;
            takmicenje.IDPredmeta = takmicenjeModel.IDPredmeta;
            takmicenje.NazivTakmicenja = takmicenjeModel.NazivTakmicenja;
            takmicenje.LokacijaTakmicenja = takmicenjeModel.LokacijaTakmicenja;
            takmicenje.TipTakmicenja = takmicenjeModel.TipTakmicenja;
            takmicenje.KorisnikID = takmicenjeModel.KorisnikID;
            //kontekst.SaveChanges();
        }

        public void Obrisi(int id)
        {
            var takmicenje = kontekst.TakmicenjaModelObjektiDBSet.Find(id);
            if (takmicenje == null) return;
            kontekst.TakmicenjaModelObjektiDBSet.Remove(takmicenje);
            //kontekst.SaveChanges();
        }

        public List<TakmicenjeModel> DajSvePoDatumu()
        {
            return kontekst.TakmicenjaModelObjektiDBSet.OrderBy(t => t.DatumTakmicenja).ToList();
        }

        public List<TakmicenjeModel> DajSve()
        {
            return kontekst.TakmicenjaModelObjektiDBSet.ToList();
        }

        public List<TakmicenjeModel> DajSvePoPredmetu(string idPredmeta)
        {
            return kontekst.TakmicenjaModelObjektiDBSet.Where(t => t.IDPredmeta == idPredmeta).ToList();
        }
        public TakmicenjeModel DajPoId(int id)
        {
            return kontekst.TakmicenjaModelObjektiDBSet.Find(id);
        }
    }
}
