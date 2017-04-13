using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TopCore.WebAPI.Controllers
{
    [Route("api/user")]
    public class UserApiController : ApiController
    {
        private readonly ILogger<UserApiController> _logger;

        public UserApiController(ILogger<UserApiController> logger)
        {
            _logger = logger;
        }

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
            int.Parse("aaa");
            return Ok(model);
        }

        public class LoginModel
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}