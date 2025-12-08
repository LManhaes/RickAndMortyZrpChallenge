using Microsoft.AspNetCore.Mvc;

namespace RickAndMortyZrpChallenge.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
