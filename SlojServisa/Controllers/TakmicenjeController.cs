using System;
using System.Collections.Generic;
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
    public class TakmicenjeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TakmicenjeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Takmicenje
        [HttpGet]
        public async Task<IActionResult> DajSve()
        {
            List<TakmicenjeViewModel> takmicenja = await _context.TakmicenjaModelObjektiDBSet
                .Select(t => new TakmicenjeViewModel
                {
                    ID = t.ID,
                    NazivTakmicenja = t.NazivTakmicenja,
                    DatumTakmicenja = t.DatumTakmicenja,
                    LokacijaTakmicenja = t.LokacijaTakmicenja,
                    TipTakmicenja = t.TipTakmicenja
                })
                .ToListAsync();

            return Ok(takmicenja);
        }

        // GET: api/Takmicenje/5
        [HttpGet("{id}")]
        public async Task<IActionResult> DajPoId(int id)
        {
            var takmicenjeModel = await _context.TakmicenjaModelObjektiDBSet.FindAsync(id);

            if (takmicenjeModel == null)
            {
                return NotFound();
            }

            TakmicenjeViewModel takmicenjeViewModel = new TakmicenjeViewModel
            {
                ID = takmicenjeModel.ID,
                NazivTakmicenja = takmicenjeModel.NazivTakmicenja,
                DatumTakmicenja = takmicenjeModel.DatumTakmicenja,
                LokacijaTakmicenja = takmicenjeModel.LokacijaTakmicenja,
                TipTakmicenja = takmicenjeModel.TipTakmicenja
            };

            return Ok(takmicenjeViewModel);
        }

        // PUT: api/Takmicenje/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Izmeni(int id, TakmicenjeViewModel takmicenjeModel)
        {

            TakmicenjeModel takmicenje = await _context.TakmicenjaModelObjektiDBSet.FindAsync(id);

            if (takmicenje == null)
            {
                return NotFound();
            } else
            {
                takmicenje.ID = takmicenjeModel.ID;
                takmicenje.NazivTakmicenja = takmicenjeModel.NazivTakmicenja;
                takmicenje.DatumTakmicenja = takmicenjeModel.DatumTakmicenja;
                takmicenje.LokacijaTakmicenja = takmicenjeModel.LokacijaTakmicenja;
                takmicenje.TipTakmicenja = takmicenjeModel.TipTakmicenja;

                _context.Entry(takmicenje).State = EntityState.Modified;
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

        // POST: api/Takmicenje
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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
                TipTakmicenja = takmicenjeModel.TipTakmicenja
            };

            _context.TakmicenjaModelObjektiDBSet.Add(takmicenje);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Takmicenje/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Obrisi(int id)
        {
            var takmicenjeModel = await _context.TakmicenjaModelObjektiDBSet.FindAsync(id);
            if (takmicenjeModel == null)
            {
                return NotFound();
            }

            _context.TakmicenjaModelObjektiDBSet.Remove(takmicenjeModel);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool Postoji(int id)
        {
            return _context.TakmicenjaModelObjektiDBSet.Any(e => e.ID == id);
        }
    }
}
