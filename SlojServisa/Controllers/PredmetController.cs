using BibliotekaKlasa.TehnoloskeKlase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrezentacioniSloj.ViewModel;
using SlojPodataka.Kontekst;
using SlojPodataka.Model;
using SlojPodataka.Repozitorijum;
using SlojPoslovneLogike;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SlojServisa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredmetController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PredmetRepo _PredmetRepo;
        public PredmetController(AppDbContext context)
        {
            _context = context;
            _PredmetRepo = new PredmetRepo(new KonekcijaKlasa("Server=(localdb)\\mssqllocaldb;Database=SkolaTakmicenja;Trusted_Connection=True;MultipleActiveResultSets=true"));
        }
        //List
        // GET: api/Predmet
        [HttpGet]
        public async Task<IActionResult> DajSve()
        {
            List<PredmetModel> predmeti = _PredmetRepo.DajSve();
                
            List<PredmetViewModel> predmetModeli = predmeti.Select(u => new PredmetViewModel
            {
                ID = u.ID,
                NazivPredmeta = u.NazivPredmeta,
                
            }).ToList();
             /*   await _context.PredmetiModelObjektiDBSet
                .Select(p => new PredmetViewModel
                {
                    ID = p.ID,
                    NazivPredmeta = p.NazivPredmeta,  
                })
                .ToListAsync();
             */
            return Ok(predmeti);
        }

        // GET: api/Predmet/5
        [HttpGet("{id}")]
        public async Task<IActionResult> DajPoId(string id)
        {
            PredmetModel predmet = _PredmetRepo.DajPredmetPoID(id);

            if (predmet == null)
            {
                return NotFound();
            }


            PredmetViewModel predmetModel = new PredmetViewModel
            {
                ID = predmet.ID,
                NazivPredmeta = predmet.NazivPredmeta,
            };

            return Ok(predmetModel);
        }
        //Edit
        [HttpPut("{id}")]
        public async Task<IActionResult> Izmeni(string id, PredmetViewModel predmetModel)
        {

            Debug.WriteLine("id: " + id + "predmet id: "+predmetModel.ID);

            Debug.WriteLine($"Izmena Predmeta ID: {predmetModel.ID}, Naziv: {predmetModel.NazivPredmeta}");

            PredmetModel predmet = _PredmetRepo.DajPredmetPoID(id);

            Debug.WriteLine($"Pronadjen Predmet ID: {predmet?.ID}, Naziv: {predmet?.NazivPredmeta}");

            if (predmet == null)
            {
                return NotFound();
            } else
            {
                PredmetModel predmetSaIzmenama = new PredmetModel
                {
                    ID = predmetModel.ID,
                    NazivPredmeta = predmetModel.NazivPredmeta,

                };

                _PredmetRepo.Izmeni(id, predmetSaIzmenama);


            }

            /*try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Postoji(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }*/

            return Ok();
        }
        //Create
        [HttpPost]
        public async Task<IActionResult> Dodaj(PredmetViewModel predmetModel)
        {
            PredmetModel predmet = new PredmetModel
            {
                ID = predmetModel.ID,
                NazivPredmeta = predmetModel.NazivPredmeta,
            };

            _PredmetRepo.Dodaj(predmet);

            return Ok();
        }

        // DELETE: api/Predmet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Obrisi(string id)
        {
            var predmetModel = _PredmetRepo.DajPredmetPoID(id);
            if (predmetModel == null)
            {
                return NotFound();
            }

            _PredmetRepo.Obrisi(id);

            return Ok();
        }

        [HttpGet("ProveriNaziv")]
        public async Task<IActionResult> ProveriNaziv([FromQuery] string naziv)
        {

            bool postoji = _PredmetRepo.DajSve().Any(p => p.NazivPredmeta.ToLower() == naziv.ToLower());

            if (postoji)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }

        }

        [HttpGet("ProveriId")]
        public async Task<IActionResult> ProveriId([FromQuery] string id)
        {

            bool postoji = _PredmetRepo.DajSve().Any(p => p.ID.ToLower() == id.ToLower());

            if (postoji)
            {
                return Ok(true);
            } else
            {
                return Ok(false);
            }
            
        }

        private bool Postoji(string id)
        {
            return _PredmetRepo.DajSve().Any(e => e.ID == id);
        }
    }
}
