using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PrezentacioniSloj.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PrezentacioniSloj.Controllers
{
    public class PredmetController : Controller
    {

        private readonly HttpClient _httpClient;

        public PredmetController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("TakmicenjeAPIKlijent");
        }

        // GET: Predmet
        public async Task<IActionResult> Index()
        {
            List<PredmetViewModel> Predmeti = await _httpClient.GetFromJsonAsync<List<PredmetViewModel>>("api/Predmet");

            foreach (var predmet in Predmeti)
            {
                Debug.WriteLine($"Predmet ID: {predmet.ID}, Naziv: {predmet.NazivPredmeta}");
            }

            return View(Predmeti);
            // return View(await _context.PredmetiModelObjektiDBSet.ToListAsync());
        }

        // GET: Predmet/Details/5
        public async Task<IActionResult> Detalji(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var predmetModel = await _httpClient.GetFromJsonAsync<PredmetViewModel>($"api/Predmet/{id}");

            if (predmetModel == null)
            {
                return NotFound();
            }

            return View(predmetModel);
        }

        // GET: Predmet/Create
        public IActionResult Dodaj()
        {
            return View();
        }

        // POST: Predmet/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj([Bind("ID,NazivPredmeta")] PredmetViewModel predmetModel)
        {

            if (ModelState.IsValid)
            {
                bool postojiNaziv = await _httpClient.GetFromJsonAsync<bool>(
            $"api/Predmet/ProveriNaziv?naziv={Uri.EscapeDataString(predmetModel.NazivPredmeta)}");
                bool postojiId = await _httpClient.GetFromJsonAsync<bool>(
            $"api/Predmet/ProveriId?id={Uri.EscapeDataString(predmetModel.ID)}");

 
                if (postojiNaziv){
                    ModelState.AddModelError(string.Empty, $"Predmet sa nazivom {predmetModel.NazivPredmeta} već postoji.");
                    return View(predmetModel);
                }

                if (postojiId)
                {
                    ModelState.AddModelError(string.Empty, $"Predmet sa šifrom {predmetModel.ID} već postoji.");
                    return View(predmetModel);
                }

                var status = await _httpClient.PostAsJsonAsync("api/Predmet", predmetModel);
                //await _context.SaveChangesAsync();
                if(!status.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Greška prilikom kreiranja predmeta.");
                    return View(predmetModel);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(predmetModel);
        }

        // GET: Predmet/Edit/5
        public async Task<IActionResult> Izmeni(string id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var predmetModel = await _httpClient.GetFromJsonAsync<PredmetViewModel>($"api/Predmet/{id}");
            if (predmetModel == null)
            {
                return NotFound();
            }
            return View(predmetModel);
        }

        // POST: Predmet/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Izmeni(string id, [Bind("ID,NazivPredmeta")] PredmetViewModel predmetModel)
        {
            Debug.WriteLine($"Izmena Predmeta ID: {predmetModel.ID}, Novi Id: {id}");

            if (ModelState.IsValid)
            {
                try
                {
                    bool postojiNaziv = await _httpClient.GetFromJsonAsync<bool>(
                $"api/Predmet/ProveriNaziv?naziv={Uri.EscapeDataString(predmetModel.NazivPredmeta)}");
                    bool postojiId = await _httpClient.GetFromJsonAsync<bool>(
                $"api/Predmet/ProveriId?id={Uri.EscapeDataString(predmetModel.ID)}");

                    if (postojiNaziv)
                    {
                        ModelState.AddModelError(string.Empty, $"Predmet sa nazivom {predmetModel.NazivPredmeta} već postoji.");
                        return View(predmetModel);
                    }

                    var odgovor = await _httpClient.PutAsJsonAsync($"api/Predmet/{id}", predmetModel);
                
                    Debug.WriteLine($"Izmena Predmeta ID: {predmetModel.ID}, Naziv: {predmetModel.NazivPredmeta}, Status: {odgovor.StatusCode}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Postoji(predmetModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            } else
            {
                ModelState.AddModelError(string.Empty, "Greška prilikom izmene predmeta.");
                return View(predmetModel);
            }
        }

        // GET: Predmet/Delete/5
        public async Task<IActionResult> Obrisi(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var predmetModel = await _httpClient.GetFromJsonAsync<PredmetViewModel>($"api/Predmet/{id}");
            
            if (predmetModel == null)
            {
                return NotFound();
            }

            return View(predmetModel);
        }

        // POST: Predmet/Delete/5
        [HttpPost, ActionName("Obrisi")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ObrisiPotvrda(string id)
        {
            Debug.WriteLine($"Brisanje Predmeta ID: {id}");

            if (id == null)
            {
                return NotFound();
            }

            var predmetModel = await _httpClient.GetFromJsonAsync<PredmetViewModel>($"api/Predmet/{id}");
            
            if (predmetModel != null)
            {
                await _httpClient.DeleteAsync($"api/Predmet/{id}");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool Postoji(string id)
        {
            return _httpClient.GetFromJsonAsync<PredmetViewModel>($"api/Predmet/{id}").Result != null;
        }
    }
}
