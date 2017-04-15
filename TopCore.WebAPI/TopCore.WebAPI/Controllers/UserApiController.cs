using TopCore.WebAPI.Service;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;

namespace TopCore.WebAPI.Controllers
{
    [Route("api/user")]
    public class UserApiController : ApiController
    {
        /// <summary>
        ///     Create 
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userName">   </param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        public IActionResult Post([FromServices] IUserService userService, [FromBody]UserName userName)
        {
            userService.Add(userName.Value);
            return Created("", new {});
        }
    }

    public class UserName
    {
        public string Value { get; set; }
    }
}