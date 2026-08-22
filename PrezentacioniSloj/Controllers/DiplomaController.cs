using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrezentacioniSloj.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace PrezentacioniSloj.Controllers
{
    public class DiplomaController : Controller
    {
        private readonly HttpClient _httpKlient;

        public DiplomaController(IHttpClientFactory httpClientFactory)
        {
            _httpKlient = httpClientFactory.CreateClient("TakmicenjeAPIKlijent");
        }

        // GET: Diploma
        public async Task<IActionResult> Index()
        {
            var diplome = await _httpKlient.GetFromJsonAsync<List<DiplomaViewModel>>("api/Diploma");
            return View(diplome);
        }

        public async Task<IActionResult> Lista()
        {
            var diplome = await _httpKlient.GetFromJsonAsync<List<DiplomaViewModel>>("api/Diploma");
            return View(diplome);
        }

        // GET: Diploma/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var diplomaModel = await _httpKlient.GetFromJsonAsync<DiplomaViewModel>($"api/Diploma/{id}");
            ;
            if (diplomaModel == null)
            {
                return NotFound();
            }

            return View(diplomaModel);
        }

        // GET: Diploma/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Diploma/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Nagrada,NazivNagrade,ImeUcenika,PrezimeUcenika")] DiplomaViewModel diplomaModel)
        {

            if (ModelState.IsValid)
            {
                await _httpKlient.PostAsJsonAsync("api/Diploma", diplomaModel);
                return RedirectToAction(nameof(Index));
            }
            return View(diplomaModel);
        }

        // GET: Diploma/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diplomaModel = await _httpKlient.GetFromJsonAsync<DiplomaViewModel>($"api/Diploma/{id}");


            if (diplomaModel == null)
            {
                return NotFound();
            }
            return View(diplomaModel);
        }

        // POST: Diploma/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Nagrada,NazivNagrade,ImeUcenika,PrezimeUcenika")] DiplomaViewModel diplomaModel)
        {
            if (id != diplomaModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _httpKlient.PutAsJsonAsync($"api/Diploma/{id}", diplomaModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiplomaModelExists(diplomaModel.ID))
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
            return View(diplomaModel);
        }

        // GET: Diploma/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diplomaModel = await _httpKlient.GetFromJsonAsync<DiplomaViewModel>($"api/Diploma/{id}");
            if (diplomaModel == null)
            {
                return NotFound();
            }

            return View(diplomaModel);
        }

        // POST: Diploma/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diplomaModel = await _httpKlient.GetFromJsonAsync<DiplomaViewModel>($"api/Diploma/{id}");
            if (diplomaModel != null)
            {
                await _httpKlient.DeleteFromJsonAsync<DiplomaViewModel>($"api/Diploma/{id}");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DiplomaModelExists(int id)
        {
            return _httpKlient.GetFromJsonAsync<List<DiplomaViewModel>>("api/Diploma").Result != null;
        }
    }
}
