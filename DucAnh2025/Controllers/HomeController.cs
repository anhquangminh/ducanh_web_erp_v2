using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
       
        public IActionResult Privacy()
        {
            return View();
        }


        [Route("Home/Error/{code?}")]
        [AllowAnonymous]
        public IActionResult Error(int? code)
        {
            ViewBag.StatusCode = code ?? 500;
            return View("Error");
        }
    }
}
