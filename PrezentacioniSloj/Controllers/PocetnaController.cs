using Microsoft.AspNetCore.Mvc;

namespace PrezentacioniSloj.Controllers
{
    public class PocetnaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
