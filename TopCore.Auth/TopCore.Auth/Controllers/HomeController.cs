using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TopCore.Auth.Controllers
{
    public class HomeController : Controller
    {
        readonly ILogger<HomeController> _log;

        public HomeController(ILogger<HomeController> log)
        {
            _log = log;
        }

        public IActionResult Index()
        {
            _log.LogInformation("LogInformation");
            _log.LogWarning("LogWarning");
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}