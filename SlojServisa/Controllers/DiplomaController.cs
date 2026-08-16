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
    public class DiplomaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiplomaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/DiplomaModels
        [HttpGet]
        public async Task<IActionResult> DajSve()
        {
            List<DiplomaViewModel> diplome = await _context.DiplomaModelObjektiDBSet
                .Select(d => new DiplomaViewModel
                {
                    ID = d.ID,
                    Nagrada = d.Nagrada
                })
                .ToListAsync();
            return Ok(diplome);
        }

        // GET: api/DiplomaModels/5
        [HttpGet("{id}")]
        public async Task<IActionResult> DajPoId(int id)
        {
            var diplomaModel = await _context.DiplomaModelObjektiDBSet.FindAsync(id);

            if (diplomaModel == null)
            {
                return NotFound();
            }

            DiplomaViewModel diplomaViewModel = new DiplomaViewModel
            {
                ID = diplomaModel.ID,
                Nagrada = diplomaModel.Nagrada
            };

            return Ok(diplomaViewModel);
        }

        //Edit
        // PUT: api/DiplomaModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Izmeni(int id, DiplomaModel diplomaModel)
        {

            DiplomaModel diploma = await _context.DiplomaModelObjektiDBSet.FindAsync(id);

            if (diploma == null)
            {
                return NotFound();
            } else
            {
                _context.Entry(diploma).State = EntityState.Modified;
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
        // POST: api/DiplomaModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> Dodaj(DiplomaViewModel diplomaModel)
        {
            DiplomaModel diploma = new DiplomaModel
            {
                ID = diplomaModel.ID,
                IDUcenika = diplomaModel.IDUcenika,
                Nagrada = diplomaModel.Nagrada
            };
            _context.DiplomaModelObjektiDBSet.Add(diploma);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/DiplomaModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Obrisi(int id)
        {
            var diplomaModel = await _context.DiplomaModelObjektiDBSet.FindAsync(id);
            if (diplomaModel == null)
            {
                return NotFound();
            }

            _context.DiplomaModelObjektiDBSet.Remove(diplomaModel);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool Postoji(int id)
        {
            return _context.DiplomaModelObjektiDBSet.Any(e => e.ID == id);
        }
    }
}
