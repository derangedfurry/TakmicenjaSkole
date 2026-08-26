using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using PrezentacioniSloj.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PrezentacioniSloj.Controllers
{
    public class KorisnikController : Controller
    {

        private readonly HttpClient _httpKlient;
        public KorisnikController(IHttpClientFactory httpClientFactory)
        {
            _httpKlient = httpClientFactory.CreateClient("TakmicenjeAPIKlijent");
        }

        // GET: KorisnikViewModels
        public async Task<IActionResult> Index()
        {
            var korisnici = await _httpKlient.GetFromJsonAsync<List<KorisnikViewModel>>("api/Korisnik");
            return View(korisnici);
        }

        public async Task<IActionResult> Lista()
        {
            var korisnici = await _httpKlient.GetFromJsonAsync<List<KorisnikViewModel>>("api/Korisnik");
            return View(korisnici);
        }

        // GET: KorisnikViewModels/Details/5
        public async Task<IActionResult> Detalji(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnikModel = await _httpKlient.GetFromJsonAsync<KorisnikViewModel>($"api/Korisnik/{id}");

            if (korisnikModel == null)
            {
                return NotFound();
            }

            return View(korisnikModel);
        }

        // GET: KorisnikViewModels/Create
        public IActionResult Dodaj()
        {
            return View();
        }

        // POST: KorisnikViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Ime,Prezime,KorisnickoIme,Email,PasswordSalt,PasswordHash,Uloga")] KorisnikViewModel korisnikModel)
        {

            if (ModelState.IsValid)
            {
                byte[] Salt = await _httpKlient.GetFromJsonAsync<byte[]>("api/PoslovnaLogika/Lozinka/GenerisiSalt");
                byte[] Hash = await _httpKlient.GetFromJsonAsync<byte[]>($"api/PoslovnaLogika/Lozinka/GenerisiHash?lozinka={korisnikModel.Lozinka}&salt={Salt.ToString()}");
                var korisnik = new KorisnikViewModel
                {
                    ID = korisnikModel.ID,
                    Ime = korisnikModel.Ime,
                    Prezime = korisnikModel.Prezime,
                    KorisnickoIme = korisnikModel.KorisnickoIme,
                    Email = korisnikModel.Email,
                    PasswordSalt = Salt,
                    PasswordHash = Hash,
                    Uloga = korisnikModel.Uloga
                };

                await _httpKlient.PostAsJsonAsync("api/Korisnik", korisnikModel);

                return RedirectToAction(nameof(Index));
            }
            return View(korisnikModel);
        }*/

        // GET: KorisnikViewModels/Edit/5
        public async Task<IActionResult> Izmeni(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnikModel = await _httpKlient.GetFromJsonAsync<KorisnikViewModel>($"api/Korisnik/{id}");

            if (korisnikModel == null)
            {
                return NotFound();
            }
            return View(korisnikModel);
        }

        // POST: KorisnikViewModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Izmeni(int id, [Bind("ID,Ime,Prezime,KorisnickoIme,Email,Lozinka,Uloga")] KorisnikViewModel korisnikModel)
        {
            if (id != korisnikModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    await _httpKlient.PutAsJsonAsync($"api/Korisnik/{id}", korisnikModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KorisnikViewModelExists(korisnikModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(korisnikModel);
        }

        // GET: KorisnikViewModels/Delete/5
        public async Task<IActionResult> Obrisi(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnikModel = await _httpKlient.GetFromJsonAsync<KorisnikViewModel>($"api/Korisnik/{id}");

            if (korisnikModel == null)
            {
                return NotFound();
            }

            return View(korisnikModel);
        }

        // POST: KorisnikViewModels/Delete/5
        [HttpPost, ActionName("Obrisi")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ObrisiPotvrda(int id)
        {
            var korisnikModel = await _httpKlient.GetFromJsonAsync<KorisnikViewModel>($"api/Korisnik/{id}");
            if (korisnikModel != null)
            {
                await _httpKlient.DeleteFromJsonAsync<KorisnikViewModel>($"api/Korisnik/{id}");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool KorisnikViewModelExists(int id)
        {
            return _httpKlient.GetFromJsonAsync<KorisnikViewModel>($"api/Korisnik/{id}").Result != null;
        }

        public IActionResult Registracija()
        {
            return View();
        }

        // POST: KorisnikViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registracija([Bind("ID,Ime,Prezime,KorisnickoIme,Email,Lozinka,LozinkaPotvrda,Uloga")] RegistracijaViewModel registracijaViewModel)
        {
            KorisnikViewModel korisnikModel = null;

            if (ModelState.IsValid)
            {

                var odgovor = await _httpKlient.PostAsJsonAsync("api/Korisnik/Registracija", registracijaViewModel);



                if (odgovor.IsSuccessStatusCode)
                {
                    korisnikModel = await odgovor.Content.ReadFromJsonAsync<KorisnikViewModel>();

                    HttpContext.Session.SetInt32("KorisnikId", korisnikModel.ID);
                    HttpContext.Session.SetString("KorisnikUloga", korisnikModel.Uloga);
                    return RedirectToAction(nameof(Index),"Pocetna");
                } else
                {
                    ModelState.AddModelError(string.Empty, "Došlo je do greške prilikom registracije. Molimo pokušajte ponovo.");
                }

            } else
            {
                ModelState.AddModelError(string.Empty, "Došlo je do greške prilikom registracije. Molimo pokušajte ponovo.");
            }
                return View(korisnikModel);
        }

        [HttpGet]
        public IActionResult Prijava()
        {
            return View();
        }

        // POST: KorisnikViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Prijava(PrijavaViewModel prijavaViewModel)
        {

            if (ModelState.IsValid)
            {

                var odgovor = await _httpKlient.PostAsJsonAsync("api/Korisnik/Prijava", prijavaViewModel);

                if (odgovor.IsSuccessStatusCode)
                {
                    KorisnikViewModel korisnik = await odgovor.Content.ReadFromJsonAsync<KorisnikViewModel>();

                    Debug.WriteLine("KorisnikID = " + korisnik.ID);
                    Debug.WriteLine("Korisnik Uloga = " + korisnik.Uloga);

                    HttpContext.Session.SetInt32("KorisnikId", korisnik.ID);
                    HttpContext.Session.SetString("KorisnikUloga", korisnik.Uloga);
                    return RedirectToAction(nameof(Index),"Pocetna");
                } else
                {
                    return BadRequest("Pogrešan email ili lozinka.");
                }
            }
            else return BadRequest("Pogrešan email ili lozinka.");
        }

        [HttpGet]
        public async Task<IActionResult> Odjava()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Pocetna");
        }

    }
}
