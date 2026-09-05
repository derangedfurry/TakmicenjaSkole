using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrezentacioniSloj.ViewModel;

namespace PrezentacioniSloj.Controllers
{
    public class UcenikController : Controller
    {
        private readonly HttpClient _httpKlient;

        public UcenikController(IHttpClientFactory httpClientFactory)
        {
            _httpKlient = httpClientFactory.CreateClient("TakmicenjeAPIKlijent");
        }

        // GET: Ucenik
        public async Task<IActionResult> Index()
        {
            var ucenici = await _httpKlient.GetFromJsonAsync<List<UcenikViewModel>>("api/Ucenik");
            return View(ucenici);
        }

        // GET: Ucenik/Details/5
        public async Task<IActionResult> Detalji(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ucenikModel = await _httpKlient.GetFromJsonAsync<UcenikViewModel>($"api/Ucenik/{id}");
            if (ucenikModel == null)
            {
                return NotFound();
            }

            return View(ucenikModel);
        }

        // GET: Ucenik/Create
        public IActionResult Dodaj()
        {
            DodajUcenikaViewModel model = new DodajUcenikaViewModel();

            model.Takmicenje = _httpKlient.GetFromJsonAsync<List<TakmicenjeViewModel>>("api/Takmicenje").Result;

            return View(model);
        }

        // POST: Ucenik/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(DodajUcenikaViewModel ucenikModel)
        {

            Debug.WriteLine("Dodavanje učenika: " + ucenikModel.Ucenik.Ime + " " + ucenikModel.Ucenik.Prezime);

            if (ModelState.IsValid)
            {
                var proveraSifre = await _httpKlient.GetFromJsonAsync<bool>($"api/Ucenik/ProveriSifru?sifra={Uri.EscapeDataString(ucenikModel.Ucenik.SifraUcenika ?? "")}");

                if (proveraSifre)
                {
                    ModelState.AddModelError(string.Empty, $"Učenik sa šifrom {ucenikModel.Ucenik.SifraUcenika} već postoji.");
                    return View(ucenikModel);
                }

                var odgovor = await _httpKlient.PostAsJsonAsync("api/Ucenik", ucenikModel.Ucenik);

                Debug.WriteLine("HTTP odgovor: " + odgovor.StatusCode);

                if (odgovor.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Učenik uspešno dodat: " + ucenikModel.Ucenik.Ime + " " + ucenikModel.Ucenik.Prezime);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Došlo je do greške prilikom dodavanja učenika.");
                }
            }
            return View(ucenikModel);
        }

        // GET: Ucenik/Edit/5
        public async Task<IActionResult> Izmeni(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ucenikModel = await _httpKlient.GetFromJsonAsync<UcenikViewModel>($"api/Ucenik/{id}");
            if (ucenikModel == null)
            {
                return NotFound();
            }
            return View(ucenikModel);
        }

        // POST: Ucenik/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Izmeni(int id, [Bind("ID,SifraUcenika,Ime,Prezime,BrojBodova,IDTakmicenja")] UcenikViewModel ucenikModel)
        {
            if (id != ucenikModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                UcenikViewModel ucenik = await _httpKlient.GetFromJsonAsync<UcenikViewModel>($"api/Ucenik/{id}");

                if (!(ucenik.SifraUcenika == ucenikModel.SifraUcenika))
                {
                    var proveraSifre = await _httpKlient.GetFromJsonAsync<bool>
                    ($"api/Ucenik/ProveriSifru?sifra={Uri.EscapeDataString(ucenikModel.SifraUcenika ?? "")}");
                    Debug.WriteLine("Provera sifre ucenika");
                    if (proveraSifre)
                    {
                        
                        ModelState.AddModelError(string.Empty, $"Učenik sa šifrom {ucenikModel.SifraUcenika} već postoji.");
                        return View(ucenikModel);
                    }

                }


                try
                {
                    await _httpKlient.PutAsJsonAsync($"api/Ucenik/{id}", ucenikModel);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Postoji(ucenikModel.ID))
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
            return View(ucenikModel);
        }

        // GET: Ucenik/Delete/5
        public async Task<IActionResult> Obrisi(int id)
        {
            if (!Postoji(id))
            {
                return NotFound();
            }

            var ucenikModel = await _httpKlient.GetFromJsonAsync<UcenikViewModel>($"api/Ucenik/{id}");

            if (ucenikModel == null)
            {
                return NotFound();
            }

            return View(ucenikModel);
        }

        // POST: Ucenik/Delete/5
        [HttpPost, ActionName("Obrisi")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ObrisiPotvrda(int id)
        {
            var ucenikModel = await _httpKlient.GetFromJsonAsync<UcenikViewModel>($"api/Ucenik/{id}");
            if (ucenikModel != null)
            {
                await _httpKlient.DeleteAsync($"api/Ucenik/{id}");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool Postoji(int id)
        {
            return _httpKlient.GetFromJsonAsync<UcenikViewModel>($"api/Ucenik/{id}").Result != null;
        }
    }
}
