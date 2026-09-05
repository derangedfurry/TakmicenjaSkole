using BibliotekaKlasa.TehnoloskeKlase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrezentacioniSloj.ViewModel;
using SlojPodataka.Kontekst;
using SlojPodataka.Model;
using SlojPodataka.Repozitorijum;
using SlojPoslovneLogike;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SlojServisa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiplomaController : ControllerBase
    {
        private readonly AppDbContext kontekst;
        private readonly DiplomaPoslovnaLogika _poslovnaLogika;
        private readonly UcenikRepo _UcenikRepo;
        private readonly DiplomaRepo _DiplomaRepo;

        public DiplomaController(AppDbContext context)
        {
            kontekst = context;
            _poslovnaLogika = new DiplomaPoslovnaLogika(kontekst);
            _UcenikRepo = new UcenikRepo(kontekst);
            _DiplomaRepo = new DiplomaRepo(new KonekcijaKlasa("Server=(localdb)\\mssqllocaldb;Database=SkolaTakmicenja;Trusted_Connection=True;MultipleActiveResultSets=true"));
        }

        // GET: api/DiplomaModels
        [HttpGet]
        public async Task<IActionResult> DajSve()
        {
            List<Nagrada> nagrade = _poslovnaLogika.DajSveNagrade();

            List<DiplomaModel> diplome = _DiplomaRepo.DajSve();
            List<UcenikModel> ucenici = await _UcenikRepo.DajSve();

            var diplomeUcenika = (from d in diplome
                                 join u in ucenici
                                 on d.IDUcenika equals u.ID
                                 select new DiplomaViewModel
                                {
                                    ID = d.ID,
                                    ImeUcenika = u.Ime,
                                    PrezimeUcenika = u.Prezime,
                                    Nagrada = d.Nagrada,
                                }
                                ).ToList();

            foreach (var diploma in diplomeUcenika)
            {
                diploma.NazivNagrade = nagrade
                    .FirstOrDefault(n => n.BrojNagrade == diploma.Nagrada)
                    ?.NazivNagrade;
            }

            foreach (DiplomaViewModel diploma in diplomeUcenika)
            {
                Debug.WriteLine($"Diploma ID = {diploma.ID} " +
                    $"Diploma Nagrada ID = {diploma.Nagrada}" +
                    $"Diploma Nagrada Naziv = {diploma.NazivNagrade}" +
                    $"Diploma ime ucenika = {diploma.ImeUcenika}" +
                    $"Diploma prezime ucenika = {diploma.PrezimeUcenika}");
            }

            return Ok(diplomeUcenika);
        }

        // GET: api/DiplomaModels/5
        [HttpGet("{id}")]
        public async Task<IActionResult> DajPoId(int id)
        {
            //var diplomaModel = await kontekst.DiplomaModelObjektiDBSet.FindAsync(id);
            DiplomaModel diploma = _DiplomaRepo.DajPoId(id);

            if (diploma == null)
            {
                return NotFound();
            }

            DiplomaViewModel diplomaViewModel = new DiplomaViewModel
            {
                ID = diploma.ID,
                Nagrada = diploma.Nagrada
            };

            return Ok(diplomaViewModel);
        }

        //Edit
        [HttpPut("{id}")]
        public async Task<IActionResult> Izmeni(int id, DiplomaViewModel diplomaModel)
        {

            DiplomaModel diploma = _DiplomaRepo.DajPoId(id);

            if (diploma == null)
            {
                return NotFound();
            } else
            {
                DiplomaModel diplomaSaIzmenama = new DiplomaModel
                {

                    ID = diplomaModel.ID,
                    IDUcenika = diplomaModel.IDUcenika,
                    Nagrada = diplomaModel.Nagrada

                };

                _DiplomaRepo.Izmeni(id, diplomaSaIzmenama);

            }

            return Ok();
        }

        //Create
        [HttpPost]
        public async Task<IActionResult> Dodaj(DiplomaViewModel diplomaModel)
        {
            DiplomaModel diploma = new DiplomaModel
            {
                ID = diplomaModel.ID,
                IDUcenika = diplomaModel.IDUcenika,
                Nagrada = diplomaModel.Nagrada
            };

            _DiplomaRepo.Dodaj(diploma);


            return Ok();
        }

        // DELETE: api/DiplomaModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Obrisi(int id)
        {
            var diplomaModel = _DiplomaRepo.DajPoId(id);
            if (diplomaModel == null)
            {
                return NotFound();
            }

            _DiplomaRepo.Obrisi(id);

            return Ok();
        }

        private bool Postoji(int id)
        {
            return _DiplomaRepo.Postoji(id);
        }
    }
}
