using Microsoft.AspNetCore.Mvc;
using TopCore.Service;

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
        [Route("")]
        public string Get([FromServices] IUserService userService)
        {
            return userService.GetUserName();
        }
    }
}