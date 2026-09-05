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
    public class UcenikController : ControllerBase
    {
        private readonly AppDbContext _kontekst;
        private readonly DiplomaPoslovnaLogika _poslovnaLogika;
        private readonly UcenikRepo _UcenikRepo;
        private readonly DiplomaRepo _DiplomaRepo;
        public UcenikController(AppDbContext kontekst)
        {
            _poslovnaLogika = new DiplomaPoslovnaLogika(kontekst);
            _kontekst = kontekst;
            _UcenikRepo = new UcenikRepo(kontekst);
            _DiplomaRepo = new DiplomaRepo(new KonekcijaKlasa("Server=(localdb)\\mssqllocaldb;Database=SkolaTakmicenja;Trusted_Connection=True;MultipleActiveResultSets=true"));
        }

        //Repo
        // GET: api/Ucenik
        [HttpGet]
        public async Task<IActionResult> DajSve()
        {
            List<DiplomaModel> diplome = _DiplomaRepo.DajSve();

            var diplomaDict = diplome
                .GroupBy(d => d.IDUcenika)
                .ToDictionary(g => g.Key, g => g.First().ID);

            List<UcenikModel> ucenici = await _UcenikRepo.DajSve();

            List<UcenikViewModel> uceniciViewModel = ucenici
                .Select(u => new UcenikViewModel
                {
                    ID = u.ID,
                    SifraUcenika = u.SifraUcenika,
                    Ime = u.Ime,
                    Prezime = u.Prezime,
                    BrojBodova = u.BrojBodova,
                    IDTakmicenja = u.IDTakmicenja,
                    
                })
                .ToList();



            foreach (var u in uceniciViewModel)
            {
                if (diplomaDict.TryGetValue(u.ID, out int diplomaId))
                    u.DiplomaID = diplomaId;
                else
                    u.DiplomaID = 0;

                Debug.WriteLine($"ucenik diploma ID = {u.DiplomaID}");

            }

            return Ok(uceniciViewModel);
        }


        //Repo
        // GET: api/Ucenik/5
        [HttpGet("{id}")]
        public async Task<IActionResult> DajPoId(int id)
        {
            UcenikModel ucenikModel = await _UcenikRepo.DajPoId(id);

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

        //Repo
        // PUT: api/Ucenik/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Izmeni(int id, UcenikViewModel ucenikModel)
        {

            UcenikModel ucenik = await _UcenikRepo.DajPoId(id);

            if (ucenik == null)
            {
                return NotFound();
            }
            else
            {
                UcenikModel ucenikSaIzmenama = new UcenikModel
                {
                    ID = ucenikModel.ID,
                    SifraUcenika = ucenikModel.SifraUcenika,
                    Ime = ucenikModel.Ime,
                    Prezime = ucenikModel.Prezime,
                    BrojBodova = ucenikModel.BrojBodova,
                    IDTakmicenja = ucenikModel.IDTakmicenja,
                };

                _UcenikRepo.Izmeni(ucenik, ucenikSaIzmenama);

                _kontekst.Entry(ucenik).State = EntityState.Modified;
            }

            try
            {
                await _kontekst.SaveChangesAsync();
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

            await _poslovnaLogika.ProveriDiplome();

            return Ok();
        }

        //Repo
        // POST: api/Ucenik
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

            _UcenikRepo.Dodaj(ucenik);
            await _kontekst.SaveChangesAsync();


            await _poslovnaLogika.ProveriDiplome();

            return Ok();
        }

        //Repo
        // Obrisi: api/Ucenik/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Obrisi(int id)
        {
            UcenikModel ucenik = await _UcenikRepo.DajPoId(id);
            if (ucenik == null)
            {
                return NotFound();
            }

            _UcenikRepo.Obrisi(ucenik);
            await _kontekst.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("ProveriSifru")]
        public async Task<IActionResult> ProveriSifru([FromQuery] string sifra)
        {
            if (string.IsNullOrWhiteSpace(sifra))
                return Ok(new { exists = false });

            bool postoji = await _kontekst.UcenikModelObjektiDBSet
                .AnyAsync(u => u.SifraUcenika.ToLower() == sifra.ToLower());

            if (postoji)
            {
                return Ok(true);
            } else
            {
                return Ok(false);
            }
        }

        private bool Postoji(int id)
        {
            return _UcenikRepo.Postoji(id);
        }


        //Repo
        [HttpGet("PoTakmicenju/{id}")]
        public async Task<IActionResult> DajPoTakmicenjuId(int id)
        {

            List<DiplomaModel> diplome = _DiplomaRepo.DajSve();


            var diplomaDict = diplome
                .GroupBy(d => d.IDUcenika)
                .ToDictionary(g => g.Key, g => g.First().ID);

            List<UcenikModel> ucenici = await _UcenikRepo.DajPoTakmicenjuId(id);


            List<UcenikViewModel> uceniciViewModel = ucenici
                .Where(u => u.IDTakmicenja == id)
                .Select(u => new UcenikViewModel
                {
                    ID = u.ID,
                    SifraUcenika = u.SifraUcenika,
                    Ime = u.Ime,
                    Prezime = u.Prezime,
                    BrojBodova = u.BrojBodova,
                    IDTakmicenja = u.IDTakmicenja

                })
                .ToList();


            foreach (var u in uceniciViewModel)
            {
                if (diplomaDict.TryGetValue(u.ID, out int diplomaId))
                    u.DiplomaID = diplomaId;
                else
                    u.DiplomaID = 0;

                Debug.WriteLine($"ucenik diploma ID = {u.DiplomaID}");
            }

            return Ok(uceniciViewModel);
        }
    }
}
