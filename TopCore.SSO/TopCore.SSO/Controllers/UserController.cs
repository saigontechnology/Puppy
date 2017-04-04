using Microsoft.AspNetCore.Mvc;

namespace TopCore.SSO.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : Controller
    {
        /// <summary>
        ///     GET: api/User 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new[] { "value1", "value2" });
        }
    }
}