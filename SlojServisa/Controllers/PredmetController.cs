using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrezentacioniSloj.ViewModel;
using SlojPodataka.Kontekst;
using SlojPodataka.Model;

namespace SlojServisa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredmetController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PredmetController(AppDbContext context)
        {
            _context = context;
        }
        //List
        // GET: api/Predmet
        [HttpGet]
        public async Task<IActionResult> DajSve()
        {
            List<PredmetViewModel> predmeti = await _context.PredmetiModelObjektiDBSet
                .Select(p => new PredmetViewModel
                {
                    ID = p.ID,
                    NazivPredmeta = p.NazivPredmeta,  
                })
                .ToListAsync();

            return Ok(predmeti);
        }
        //Single
        // GET: api/Predmet/5
        [HttpGet("{id}")]
        public async Task<IActionResult> DajPoId(string id)
        {
            var predmetModel = await _context.PredmetiModelObjektiDBSet.FindAsync(id);



            if (predmetModel == null)
            {
                return NotFound();
            }


            PredmetViewModel predmet = new PredmetViewModel
            {
                ID = predmetModel.ID,
                NazivPredmeta = predmetModel.NazivPredmeta,
            };

            return Ok(predmet);
        }
        //Edit
        // PUT: api/Predmet/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Izmeni(string id, PredmetViewModel predmetModel)
        {

            Debug.WriteLine("id: " + id + "predmet id: "+predmetModel.ID);

            Debug.WriteLine($"Izmena Predmeta ID: {predmetModel.ID}, Naziv: {predmetModel.NazivPredmeta}");

            PredmetModel predmet = await _context.PredmetiModelObjektiDBSet.FindAsync(id);

            Debug.WriteLine($"Pronadjen Predmet ID: {predmet?.ID}, Naziv: {predmet?.NazivPredmeta}");

            if (predmet == null)
            {
                return NotFound();
            } else
            {
                predmet.ID = predmetModel.ID;
                predmet.NazivPredmeta = predmetModel.NazivPredmeta;

                _context.Entry(predmet).State = EntityState.Modified;
            }

            try
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
            }

            return Ok();
        }
        //Create
        // POST: api/Predmet
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> Dodaj(PredmetViewModel predmetModel)
        {
            PredmetModel predmet = new PredmetModel
            {
                ID = predmetModel.ID,
                NazivPredmeta = predmetModel.NazivPredmeta,
            };

            _context.PredmetiModelObjektiDBSet.Add(predmet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Postoji(predmetModel.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // DELETE: api/Predmet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Obrisi(string id)
        {
            var predmetModel = await _context.PredmetiModelObjektiDBSet.FindAsync(id);
            if (predmetModel == null)
            {
                return NotFound();
            }

            _context.PredmetiModelObjektiDBSet.Remove(predmetModel);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool Postoji(string id)
        {
            return _context.PredmetiModelObjektiDBSet.Any(e => e.ID == id);
        }
    }
}
