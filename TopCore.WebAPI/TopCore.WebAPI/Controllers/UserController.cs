using Microsoft.AspNetCore.Mvc;

namespace TopCore.WebAPI.Controllers
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