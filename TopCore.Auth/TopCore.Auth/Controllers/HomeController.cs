using Microsoft.AspNetCore.Mvc;

namespace TopCore.Auth.Controllers
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