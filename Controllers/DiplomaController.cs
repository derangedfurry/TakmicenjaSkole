using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlojPodataka.Kontekst;
using SlojPodataka.Model;

namespace TakmicenjaSkole.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiplomaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiplomaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/diploma
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiplomaModel>>> GetAll()
        {
            return await _context.DiplomaModelObjektiDBSet.ToListAsync();
        }

        // GET: api/diploma/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DiplomaModel>> GetById(int id)
        {
            var diploma = await _context.DiplomaModelObjektiDBSet.FindAsync(id);
            if (diploma == null) return NotFound();
            return diploma;
        }

        // GET: api/diploma/ucenik/3
        [HttpGet("ucenik/{ucenikId:int}")]
        public async Task<ActionResult<IEnumerable<DiplomaModel>>> GetByUcenik(int ucenikId)
        {
            var diplomas = await _context.DiplomaModelObjektiDBSet
                .Where(d => d.IDUcenika == ucenikId)
                .ToListAsync();

            return diplomas;
        }

        // POST: api/diploma
        [HttpPost]
        public async Task<ActionResult<DiplomaModel>> Create(DiplomaModel diploma)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.DiplomaModelObjektiDBSet.Add(diploma);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = diploma.ID }, diploma);
        }

        // PUT: api/diploma/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, DiplomaModel diploma)
        {
            if (id != diploma.ID) return BadRequest("ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _context.DiplomaModelObjektiDBSet.FindAsync(id);
            if (existing == null) return NotFound();

            // Update fields explicitly
            existing.IDUcenika = diploma.IDUcenika;
            existing.Nagrada = diploma.Nagrada;

            _context.Entry(existing).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/diploma/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var diploma = await _context.DiplomaModelObjektiDBSet.FindAsync(id);
            if (diploma == null) return NotFound();

            _context.DiplomaModelObjektiDBSet.Remove(diploma);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}