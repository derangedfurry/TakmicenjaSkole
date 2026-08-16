using BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PrezentacioniSloj.ViewModel;
using SlojPodataka.Kontekst;
using SlojPodataka.Model;

namespace SlojServisa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PoslovnaLogikaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PoslovnaLogikaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PoslovnaLogika/Datum
       /* [HttpGet("Datum")]
        public async Task<ActionResult<bool>> ProveriDatume(DateTime pocetak, DateTime Kraj)
        {


        }

        // GET: api/PoslovnaLogika/Ogranicenja
        [HttpGet("Ogranicenja")]
        public async Task<ActionResult<bool>> ProveriOgranicenja()
        {
       
        }*/
    }
}
