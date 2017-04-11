using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TopCore.Auth.Controllers
{
    [Authorize]
    public class DashboardController : MvcController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}