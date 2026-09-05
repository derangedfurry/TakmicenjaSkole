using BibliotekaKlasa.TehnoloskeKlase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrezentacioniSloj.ViewModel;
using SlojPodataka.Kontekst;
using SlojPodataka.Model;
using SlojPodataka.Repozitorijum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlojServisa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TakmicenjeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PredmetRepo _PredmetRepo;
        private readonly TakmicenjeRepo _TakmicenjeRepo;
        public TakmicenjeController(AppDbContext context)
        {
            _context = context;
            _PredmetRepo = new PredmetRepo(new KonekcijaKlasa("Server=(localdb)\\mssqllocaldb;Database=SkolaTakmicenja;Trusted_Connection=True;MultipleActiveResultSets=true"));
            _TakmicenjeRepo = new TakmicenjeRepo(context);
        }

        // GET: api/Takmicenje
        [HttpGet]
        public async Task<IActionResult> DajSve()
        {
            var predmeti = _PredmetRepo.DajSve()
                .Select(p => new { p.ID, p.NazivPredmeta })
                .ToList();

            var predmetDict = predmeti.ToDictionary(p => p.ID, p => p.NazivPredmeta);

            var takmicenjaModel = _TakmicenjeRepo.DajSve()
                .Select(t => new
                {
                    t.ID,
                    t.NazivTakmicenja,
                    t.DatumTakmicenja,
                    t.LokacijaTakmicenja,
                    t.TipTakmicenja,
                    t.IDPredmeta,
                    t.KorisnikID
                })
                .ToList();

            var takmicenja = takmicenjaModel.Select(t => new TakmicenjeViewModel
            {
                ID = t.ID,
                NazivTakmicenja = t.NazivTakmicenja,
                DatumTakmicenja = t.DatumTakmicenja,
                LokacijaTakmicenja = t.LokacijaTakmicenja,
                TipTakmicenja = t.TipTakmicenja,
                NazivPredmetaTakmicenja = predmetDict.GetValueOrDefault(t.IDPredmeta),
                KorisnikID = t.KorisnikID
            }).ToList();

            return Ok(takmicenja);
        }
        // GET: api/Takmicenje/5
        [HttpGet("{id}")]
        public async Task<IActionResult> DajPoId(int id)
        {
            var takmicenjeModel = _TakmicenjeRepo.DajPoId(id);

            var predmet = _PredmetRepo.DajPredmetPoID(takmicenjeModel.IDPredmeta);

            if (takmicenjeModel == null)
            {
                return NotFound();
            }

            TakmicenjeViewModel takmicenjeViewModel = new TakmicenjeViewModel
            {
                ID = takmicenjeModel.ID,
                NazivTakmicenja = takmicenjeModel.NazivTakmicenja,
                NazivPredmetaTakmicenja = predmet?.NazivPredmeta,
                DatumTakmicenja = takmicenjeModel.DatumTakmicenja,
                LokacijaTakmicenja = takmicenjeModel.LokacijaTakmicenja,
                TipTakmicenja = takmicenjeModel.TipTakmicenja,
                KorisnikID = takmicenjeModel.KorisnikID,
            };

            return Ok(takmicenjeViewModel);
        }

        // PUT: api/Takmicenje/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Izmeni(int id, TakmicenjeViewModel takmicenjeModel)
        {

            TakmicenjeModel takmicenje = await _context.TakmicenjaModelObjektiDBSet.FindAsync(id);
            PredmetModel predmet = _PredmetRepo.DajPredmetPoNazivu(takmicenjeModel.NazivPredmetaTakmicenja);
            if (takmicenje == null)
            {
                return NotFound();
            } else
            {
                TakmicenjeModel takmicenjeSaIzmenama = new TakmicenjeModel
                {
                    ID = takmicenjeModel.ID,
                    IDPredmeta = predmet.ID,
                    NazivTakmicenja = takmicenjeModel.NazivTakmicenja,
                    DatumTakmicenja = takmicenjeModel.DatumTakmicenja,
                    LokacijaTakmicenja = takmicenjeModel.LokacijaTakmicenja,
                    TipTakmicenja = takmicenjeModel.TipTakmicenja,
                    KorisnikID = takmicenjeModel.KorisnikID
                };

                _TakmicenjeRepo.Izmeni(id, takmicenjeSaIzmenama);



            }

            return Ok();
        }

        // POST: api/Takmicenje
        [HttpPost]
        public async Task<IActionResult> Dodaj(TakmicenjeViewModel takmicenjeModel)
        {

            string IDpredmeta = await _context.PredmetiModelObjektiDBSet
                .Where(p => p.NazivPredmeta == takmicenjeModel.NazivPredmetaTakmicenja)
                .Select(p => p.ID)
                .FirstOrDefaultAsync();

            if (IDpredmeta == null)
            {
                return BadRequest("Predmet sa datim nazivom ne postoji.");
            }

            TakmicenjeModel takmicenje = new TakmicenjeModel
            {
                ID = takmicenjeModel.ID,
                IDPredmeta = IDpredmeta,
                NazivTakmicenja = takmicenjeModel.NazivTakmicenja,
                DatumTakmicenja = takmicenjeModel.DatumTakmicenja,
                LokacijaTakmicenja = takmicenjeModel.LokacijaTakmicenja,
                TipTakmicenja = takmicenjeModel.TipTakmicenja,
                KorisnikID = takmicenjeModel.KorisnikID,
            };

            _TakmicenjeRepo.Dodaj(takmicenje);

            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Takmicenje/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Obrisi(int id)
        {
            var takmicenjeModel = _TakmicenjeRepo.DajPoId(id);
            if (takmicenjeModel == null)
            {
                return NotFound();
            }

            _TakmicenjeRepo.Obrisi(id);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool Postoji(int id)
        {
            return _TakmicenjeRepo.DajSve().Any(e => e.ID == id);
        }
    }
}
