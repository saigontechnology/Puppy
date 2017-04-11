using Microsoft.AspNetCore.Mvc;

namespace TopCore.WebAPI.Controllers
{
    public class HomeController : MvcController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}