using BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrezentacioniSloj.ViewModel;
using SlojPodataka.Kontekst;
using SlojPodataka.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojServisa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KorisnikController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KorisnikController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Korisnik
        [HttpGet]
        public async Task<IActionResult> DajSve()
        {
            List<KorisnikViewModel> korisnici = await _context.KorisnikModelObjektiDBSet
                .Select(k => new KorisnikViewModel
                {
                    ID = k.ID,
                    KorisnickoIme = k.KorisnickoIme,
                    Ime = k.Ime,
                    Prezime = k.Prezime,
                    Email = k.Email,
                    Uloga = k.Uloga
                })
                .ToListAsync();

            return Ok(korisnici);
        }

        // GET: api/Korisnik/5
        [HttpGet("{id}")]
        public async Task<IActionResult> DajPoId(int id)
        {
            var korisnikModel = await _context.KorisnikModelObjektiDBSet.FindAsync(id);

            if (korisnikModel == null)
            {
                return NotFound();
            }

            KorisnikViewModel korisnikViewModel = new KorisnikViewModel
            {
                ID = korisnikModel.ID,
                KorisnickoIme = korisnikModel.KorisnickoIme,
                Ime = korisnikModel.Ime,
                Prezime = korisnikModel.Prezime,
                Email = korisnikModel.Email,
                Uloga = korisnikModel.Uloga
            };

            return Ok(korisnikModel);
        }


        //Prijava korisnika
        [HttpPost("Prijava")]
        public async Task<IActionResult> Prijava(PrijavaViewModel prijavaViewModel)
        {
            KorisnikModel korisnik = await _context.KorisnikModelObjektiDBSet
                .FirstOrDefaultAsync(
                k => k.Email == prijavaViewModel.EmailIliKorisnickoIme || 
                k.KorisnickoIme == prijavaViewModel.EmailIliKorisnickoIme);

            //KorisnikModel korisnik = await _context.KorisnikModelObjektiDBSet.FirstOrDefaultAsync(k => k.Email == email);

            if (korisnik == null)
            {
                return NotFound("Korisnik sa datim emailom i lozinkom ne postoji.");

            } else
            {
                KorisnikViewModel korisnikViewModel = new KorisnikViewModel
                {
                    ID = korisnik.ID,
                    KorisnickoIme = korisnik.KorisnickoIme,
                    Ime = korisnik.Ime,
                    Prezime = korisnik.Prezime,
                    Email = korisnik.Email,
                    Uloga = korisnik.Uloga
                };

                return Ok(korisnikViewModel);
            }
        }

        // PUT: api/Korisnik/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Izmeni(int id, KorisnikViewModel korisnikModel)
        {

            KorisnikModel korisnik = await _context.KorisnikModelObjektiDBSet.FindAsync(id);

            if(korisnik == null)
            {
                return NotFound();
            } else
            {
                korisnik.ID = korisnikModel.ID;
                korisnik.KorisnickoIme = korisnikModel.KorisnickoIme;
                korisnik.Ime = korisnikModel.Ime;
                korisnik.Prezime = korisnikModel.Prezime;
                korisnik.Email = korisnikModel.Email;
                
                if(!FunkcijeLozinke.VerifikujLozinku(korisnikModel.Lozinka, korisnik.PasswordHash, korisnik.PasswordSalt))
                {
                    byte[] LozinkaSalt;
                    byte[] LozinkaHash;
                    FunkcijeLozinke.KreirajHash(korisnikModel.Lozinka, out LozinkaHash, out LozinkaSalt);
                    korisnik.PasswordHash = LozinkaHash;
                    korisnik.PasswordSalt = LozinkaSalt;
                } else
                {
                    return BadRequest("Lozinka nije validna.");
                }

                _context.Entry(korisnik).State = EntityState.Modified;
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

        // DELETE: api/Korisnik/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Obrisi(int id)
        {
            var korisnikModel = await _context.KorisnikModelObjektiDBSet.FindAsync(id);
            if (korisnikModel == null)
            {
                return NotFound();
            }

            _context.KorisnikModelObjektiDBSet.Remove(korisnikModel);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool Postoji(int id)
        {
            return _context.KorisnikModelObjektiDBSet.Any(e => e.ID == id);
        }

        [HttpPost("Registracija")]
        public async Task<IActionResult> Registruj(RegistracijaViewModel registracijaViewModel)
        {

            if (registracijaViewModel == null)
            {
                return NotFound();
            }

            byte[] LozinkaSalt;
            byte[] LozinkaHash;

            FunkcijeLozinke.KreirajHash(registracijaViewModel.Lozinka, out LozinkaHash, out LozinkaSalt);

            if (FunkcijeLozinke.VerifikujLozinku(registracijaViewModel.Lozinka, LozinkaHash, LozinkaSalt))
            {
                Debug.WriteLine("Lozinka je validna.");


                KorisnikModel korisnikModel = new KorisnikModel
                {
                    Ime = registracijaViewModel.Ime,
                    Prezime = registracijaViewModel.Prezime,
                    KorisnickoIme = registracijaViewModel.KorisnickoIme,
                    Email = registracijaViewModel.Email,
                    PasswordHash = LozinkaHash,
                    PasswordSalt = LozinkaSalt,
                    //Uloga je korisnik
                };

                KorisnikViewModel korisnikViewModel = new KorisnikViewModel
                {
                    ID = korisnikModel.ID,
                    Ime = korisnikModel.Ime,
                    Prezime = korisnikModel.Prezime,
                    KorisnickoIme = korisnikModel.KorisnickoIme,
                    Email = korisnikModel.Email,
                    Uloga = korisnikModel.Uloga
                };


                _context.KorisnikModelObjektiDBSet.Add(korisnikModel);
                await _context.SaveChangesAsync();

                return Ok(korisnikViewModel);
                //return CreatedAtAction("GetKorisnikModel", new { id = korisnikModel.ID }, korisnikModel);
            } else
            {
                Debug.WriteLine("Lozinka nije validna.");
                return BadRequest("Lozinka nije validna.");
            }

        }
    }
}
