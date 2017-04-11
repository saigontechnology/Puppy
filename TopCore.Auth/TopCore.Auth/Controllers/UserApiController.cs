using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TopCore.Auth.Controllers
{
    [Route("api/user")]
    public class UserApiController : ApiController
    {
        /// <summary>
        ///     Login 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(LoginModel))]
        public IActionResult Post([FromBody] LoginModel model)
        {
            return Ok(model);
        }

        public class LoginModel
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}