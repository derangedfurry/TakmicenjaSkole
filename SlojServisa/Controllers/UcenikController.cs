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
    public class UcenikController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UcenikController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Ucenik
        [HttpGet]
        public async Task<IActionResult> DajSve()
        {
            List<UcenikViewModel> ucenici = await _context.UcenikModelObjektiDBSet
                .Select(u => new UcenikViewModel
                {
                    ID = u.ID,
                    SifraUcenika = u.SifraUcenika,
                    Ime = u.Ime,
                    Prezime = u.Prezime,
                    BrojBodova = u.BrojBodova,
                    IDTakmicenja = u.IDTakmicenja
                })
                .ToListAsync();

            return Ok(ucenici);
        }

        // GET: api/Ucenik/5
        [HttpGet("{id}")]
        public async Task<IActionResult> DajPoId(int id)
        {
            var ucenikModel = await _context.UcenikModelObjektiDBSet.FindAsync(id);

            if (ucenikModel == null)
            {
                return NotFound();
            }

            UcenikViewModel ucenikViewModel = new UcenikViewModel
            {
                ID = ucenikModel.ID,
                SifraUcenika = ucenikModel.SifraUcenika,
                Ime = ucenikModel.Ime,
                Prezime = ucenikModel.Prezime,
                BrojBodova = ucenikModel.BrojBodova,
                IDTakmicenja = ucenikModel.IDTakmicenja
            };

            return Ok(ucenikViewModel);
        }

        // PUT: api/Ucenik/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Izmeni(int id, UcenikViewModel ucenikModel)
        {

            UcenikModel ucenik = await _context.UcenikModelObjektiDBSet.FindAsync(id);

            if (ucenik == null)
            {
                return NotFound();
            }
            else
            {
                ucenik.Ime = ucenikModel.Ime;
                ucenik.SifraUcenika = ucenikModel.SifraUcenika;
                ucenik.Prezime = ucenikModel.Prezime;
                ucenik.BrojBodova = ucenikModel.BrojBodova;
                ucenik.IDTakmicenja = ucenikModel.IDTakmicenja;
                _context.Entry(ucenikModel).State = EntityState.Modified;
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

        // POST: api/Ucenik
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> Dodaj(UcenikViewModel ucenikModel)
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

            _context.UcenikModelObjektiDBSet.Add(ucenik);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Ucenik/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Obrisi(int id)
        {
            var ucenikModel = await _context.UcenikModelObjektiDBSet.FindAsync(id);
            if (ucenikModel == null)
            {
                return NotFound();
            }

            _context.UcenikModelObjektiDBSet.Remove(ucenikModel);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool Postoji(int id)
        {
            return _context.UcenikModelObjektiDBSet.Any(e => e.ID == id);
        }
    }
}
