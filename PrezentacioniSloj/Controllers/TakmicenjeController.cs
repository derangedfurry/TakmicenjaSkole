using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class TakmicenjeController : Controller
    {
        private readonly HttpClient _httpKlient;

        public TakmicenjeController(IHttpClientFactory httpClientFactory)
        {
            _httpKlient = httpClientFactory.CreateClient("TakmicenjeAPIKlijent");
        }

        // GET: Takmicenje
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? odDatuma, DateTime? doDatuma)
        {
            var lista = await _httpKlient
                .GetFromJsonAsync<List<TakmicenjeViewModel>>("api/Takmicenje")
                ?? new List<TakmicenjeViewModel>();


            if (odDatuma.HasValue)
            {
                lista = lista
                    .Where(t => t.DatumTakmicenja.Date >= odDatuma.Value.Date)
                    .ToList();
            }

            if (doDatuma.HasValue)
            {
                lista = lista
                    .Where(t => t.DatumTakmicenja.Date <= doDatuma.Value.Date)
                    .ToList();
            }

            return View(lista);
        }

        public async Task<IActionResult> Lista()
        {
            var takmicenja = await _httpKlient.GetFromJsonAsync<List<TakmicenjeViewModel>>("api/Takmicenje");
            return View(takmicenja);
        }

        // GET: Takmicenje/Details/5
        public async Task<IActionResult> Detalji(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var takmicenjeModel = await _httpKlient.GetFromJsonAsync<TakmicenjeViewModel>($"api/Takmicenje/{id}");

            if (takmicenjeModel == null)
            {
                return NotFound();
            }

            return View(takmicenjeModel);
        }

        // GET: Takmicenje/Create
        public IActionResult Dodaj()
        {
            return View();
        }

        // POST: Takmicenje/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj([Bind("ID,DatumTakmicenja,NazivPredmetaTakmicenja,NazivTakmicenja,TipTakmicenja,LokacijaTakmicenja,KorisnikID")] TakmicenjeViewModel takmicenjeModel)
        {
            if (ModelState.IsValid)
            {
                await _httpKlient.PostAsJsonAsync("api/Takmicenje", takmicenjeModel);
                return RedirectToAction(nameof(Lista));
            }
            return View(takmicenjeModel);
        }


        [HttpGet]
        public IActionResult DodajRezultat()
        {
            var model = new DodajTakmicenjeViewModel();
            model.Ucenik.Add(new UcenikViewModel());

            model.Predmet = _httpKlient.GetFromJsonAsync<List<PredmetViewModel>>("api/Predmet").Result;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajRezultat(DodajTakmicenjeViewModel model)
        {
            Debug.WriteLine("DodajRezultat POST called");
            Debug.WriteLine("Selektovan predmet Takmicenje : " + model.Takmicenje.NazivPredmetaTakmicenja);
            Debug.WriteLine("KorisnikId = ", model.Takmicenje.KorisnikID);
            if(model.Takmicenje.NazivPredmetaTakmicenja == null)
            {
                ModelState.AddModelError("selektovanPredmet", "Morate izabrati predmet.");
                return View(model);
            }


            if (!ModelState.IsValid)
            {
                Debug.WriteLine("ModelState is invalid. Errors:");
                Debug.WriteLine(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(model);
            }
            


            var takmicenjeResponse = await _httpKlient.PostAsJsonAsync("api/Takmicenje", model.Takmicenje);
            if (!takmicenjeResponse.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Error creating Takmicenje: {takmicenjeResponse.StatusCode}");
                ModelState.AddModelError("", "Greška pri čuvanju takmičenja.");
                return View(model);
            }


            foreach (var ucenik in model.Ucenik)
            {
                ucenik.IDTakmicenja = _httpKlient.GetFromJsonAsync<List<TakmicenjeViewModel>>("api/Takmicenje").Result.LastOrDefault()?.ID ?? 0;
                // ucenik.IDTakmicenja = created.ID;
                var odgovor = await _httpKlient.PostAsJsonAsync("api/Ucenik", ucenik);

                if (odgovor.IsSuccessStatusCode)
                    {
                    Debug.WriteLine($"Učenik {ucenik.Ime} {ucenik.Prezime} uspešno dodat.");
                }
                else
                {
                    Debug.WriteLine($"Greška prilikom dodavanja učenika {ucenik.Ime} {ucenik.Prezime}: {odgovor.StatusCode}");
                    ModelState.AddModelError("", $"Greška prilikom dodavanja učenika {ucenik.Ime} {ucenik.Prezime}.");
                    return View(model);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Rezultati(
        DateTime? odDatuma,
        DateTime? doDatuma,
        string? predmet,
        string? tip)
        {
            List<RezultatViewModel> rezultati = new();

            var takmicenja = _httpKlient.GetFromJsonAsync<List<TakmicenjeViewModel>>("api/Takmicenje").Result;

            foreach (var takmicenje in takmicenja)
            {
                var ucenici = _httpKlient.GetFromJsonAsync<List<UcenikViewModel>>($"api/Ucenik/PoTakmicenju/{takmicenje.ID}").Result;
                
                foreach(var ucenik in ucenici)
                {
                    Debug.WriteLine($"Učenik: {ucenik.Ime} {ucenik.Prezime}, Broj bodova: {ucenik.BrojBodova}, Takmičenje ID: {ucenik.IDTakmicenja} DiplomaID: {ucenik.DiplomaID}");
                }
                var rezultat = new RezultatViewModel
                {
                    Takmicenje = takmicenje,
                    Ucenik = ucenici
                };

                rezultati.Add(rezultat);
            }



            rezultati = Filtriraj(rezultati, odDatuma, doDatuma, predmet, tip);
            return View(rezultati);
        }

        [HttpGet]
        public async Task<IActionResult> Stampaj(
            DateTime? odDatuma,
            DateTime? doDatuma,
            string? predmet,
            string? tip)
        {
            List<RezultatViewModel> rezultati = new();

            var takmicenja = _httpKlient.GetFromJsonAsync<List<TakmicenjeViewModel>>("api/Takmicenje").Result;

            foreach (var takmicenje in takmicenja)
            {
                var ucenici = _httpKlient.GetFromJsonAsync<List<UcenikViewModel>>($"api/Ucenik/PoTakmicenju/{takmicenje.ID}").Result;
                var rezultat = new RezultatViewModel
                {
                    Takmicenje = takmicenje,
                    Ucenik = ucenici
                };
                rezultati.Add(rezultat);
            }

            rezultati = Filtriraj(rezultati, odDatuma, doDatuma, predmet, tip);
            return View(rezultati); 
        }

        private List<RezultatViewModel> Filtriraj(
            List<RezultatViewModel> lista,
            DateTime? odDatuma,
            DateTime? doDatuma,
            string? predmet,
            string? tip)
        {
            if (odDatuma.HasValue)
                lista = lista.Where(r => r.Takmicenje.DatumTakmicenja.Date >= odDatuma.Value.Date).ToList();

            if (doDatuma.HasValue)
                lista = lista.Where(r => r.Takmicenje.DatumTakmicenja.Date <= doDatuma.Value.Date).ToList();

            if (!string.IsNullOrWhiteSpace(predmet))
                lista = lista.Where(r => r.Takmicenje.NazivPredmetaTakmicenja != null &&
                                         r.Takmicenje.NazivPredmetaTakmicenja
                                            .Contains(predmet, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(tip))
                lista = lista.Where(r => r.Takmicenje.TipTakmicenja != null &&
                                         r.Takmicenje.TipTakmicenja
                                            .Contains(tip, StringComparison.OrdinalIgnoreCase)).ToList();

            return lista;
        }


        // GET: Takmicenje/Edit/5
        public async Task<IActionResult> Izmeni(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var takmicenjeModel = await _httpKlient.GetFromJsonAsync<TakmicenjeViewModel>($"api/Takmicenje/{id}");
            if (takmicenjeModel == null)
            {
                return NotFound();
            }

            ViewBag.Predmeti = await _httpKlient
            .GetFromJsonAsync<List<PredmetViewModel>>("api/Predmet")
            ?? new List<PredmetViewModel>();

            ViewBag.SelektovanPredmet = takmicenjeModel.NazivPredmetaTakmicenja;

            return View(takmicenjeModel);
        }

        // POST: Takmicenje/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Izmeni(int id, [Bind("ID,DatumTakmicenja,NazivPredmetaTakmicenja,NazivTakmicenja,TipTakmicenja,LokacijaTakmicenja,KorisnikID")] TakmicenjeViewModel takmicenjeModel)
        {
            if (id != takmicenjeModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _httpKlient.PutAsJsonAsync($"api/Takmicenje/{id}", takmicenjeModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Postoji(takmicenjeModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Lista));
            }
            return View(takmicenjeModel);
        }

        // GET: Takmicenje/Delete/5
        public async Task<IActionResult> Obrisi(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var takmicenjeModel = await _httpKlient.GetFromJsonAsync<TakmicenjeViewModel>($"api/Takmicenje/{id}");

            if (takmicenjeModel == null)
            {
                return NotFound();
            }

            return View(takmicenjeModel);
        }

        // POST: Takmicenje/Delete/5
        [HttpPost, ActionName("Obrisi")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ObrisiPotvrda(int id)
        {
            var takmicenjeModel = await _httpKlient.GetFromJsonAsync<TakmicenjeViewModel>($"api/Takmicenje/{id}");
            if (takmicenjeModel != null)
            {
                await _httpKlient.DeleteAsync($"api/Takmicenje/{id}");
            }

            return RedirectToAction(nameof(Lista));
        }

        private bool Postoji(int id)
        {
            return _httpKlient.GetFromJsonAsync<TakmicenjeViewModel>($"api/Takmicenje/{id}").Result != null;    
        }
    }
}
