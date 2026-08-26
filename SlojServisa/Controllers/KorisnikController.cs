using BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrezentacioniSloj.ViewModel;
using SlojPodataka.Kontekst;
using SlojPodataka.Model;
using SlojPodataka.Repozitorijum;
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
        private readonly KorisnikRepo _KorisnikRepo;
        public KorisnikController(AppDbContext context)
        {
            _context = context;
            _KorisnikRepo = new KorisnikRepo(context);
        }

        // GET: api/Korisnik
        [HttpGet]
        public async Task<IActionResult> DajSve()
        {
            List<KorisnikModel> korsicnici = await _KorisnikRepo.DajSve();

            List<KorisnikViewModel> korisniciModel = korsicnici
                .Select(k => new KorisnikViewModel
                {
                    ID = k.ID,
                    KorisnickoIme = k.KorisnickoIme,
                    Ime = k.Ime,
                    Prezime = k.Prezime,
                    Email = k.Email,
                    Uloga = k.Uloga
                })
                .ToList();

            return Ok(korisniciModel);
        }

        // GET: api/Korisnik/5
        [HttpGet("{id}")]
        public async Task<IActionResult> DajPoId(int id)
        {
            KorisnikModel korisnik = await _KorisnikRepo.DajPoId(id);

            if (korisnik == null)
            {
                return NotFound();
            }

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

        // PUT: api/Korisnik/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Izmeni(int id, KorisnikViewModel korisnikModel)
        {

            KorisnikModel korisnik = await _KorisnikRepo.DajPoId(id);

            if(korisnik == null)
            {
                return NotFound();
            } else
            {
                KorisnikModel korisnikSaIzmenama = new KorisnikModel
                {
                    ID = korisnikModel.ID,
                    KorisnickoIme = korisnikModel.KorisnickoIme,
                    Ime = korisnikModel.Ime,
                    Prezime = korisnikModel.Prezime,
                    Email = korisnikModel.Email,
                    Uloga = korisnikModel.Uloga
                };

                
                if(!FunkcijeLozinke.VerifikujLozinku(korisnikModel.Lozinka, korisnik.PasswordHash, korisnik.PasswordSalt))
                {
                    byte[] LozinkaSalt;
                    byte[] LozinkaHash;
                    FunkcijeLozinke.KreirajHash(korisnikModel.Lozinka, out LozinkaHash, out LozinkaSalt);
                    korisnikSaIzmenama.PasswordHash = LozinkaHash;
                    korisnikSaIzmenama.PasswordSalt = LozinkaSalt;
                } else
                {
                    return BadRequest("Lozinka nije validna.");
                }

                _KorisnikRepo.Izmeni(id, korisnikSaIzmenama);

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
            var korisnikModel = await _KorisnikRepo.DajPoId(id);
            if (korisnikModel == null)
            {
                return NotFound();
            }

            _KorisnikRepo.Obrisi(id);
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

                _KorisnikRepo.Dodaj(korisnikModel);


                await _context.SaveChangesAsync();

                return Ok(korisnikViewModel);
                //return CreatedAtAction("GetKorisnikModel", new { id = korisnikModel.ID }, korisnikModel);
            } else
            {
                Debug.WriteLine("Lozinka nije validna.");
                return BadRequest("Lozinka nije validna.");
            }

        }

        //Prijava korisnika
        [HttpPost("Prijava")]
        public async Task<IActionResult> Prijava(PrijavaViewModel prijavaViewModel)
        {
            List<KorisnikModel> korisnici = await _KorisnikRepo.DajSve();
            KorisnikModel korisnik = korisnici
                .FirstOrDefault(
                k => k.Email == prijavaViewModel.EmailIliKorisnickoIme ||
                k.KorisnickoIme == prijavaViewModel.EmailIliKorisnickoIme);

            if (korisnik == null)
            {
                Debug.WriteLine("Nije pronadjen korisnik");
                return NotFound("Korisnik sa datim emailom i lozinkom ne postoji.");

            }
            else
            {

                if (FunkcijeLozinke.VerifikujLozinku(prijavaViewModel.Lozinka, korisnik.PasswordHash, korisnik.PasswordSalt))
                {
                    Debug.WriteLine("Korisnik lozinka = " + prijavaViewModel.Lozinka);
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
                else
                {
                    Debug.WriteLine("Korisnik lozinka = " + prijavaViewModel.Lozinka);
                    Debug.WriteLine("Korisnik hash = " + korisnik.PasswordHash.Length);
                    Debug.WriteLine("Korisnik salt = " + korisnik.PasswordSalt.Length);
                    return NotFound("Korisnik sa datim emailom i lozinkom ne postoji.");
                }

            }
        }
    }
}
