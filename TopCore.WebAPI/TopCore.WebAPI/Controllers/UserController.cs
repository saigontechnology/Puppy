using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TopCore.Service;

namespace TopCore.WebAPI.Controllers
{
    [Route("api/User")]
    [Produces("application/json")]
    public class UserAPIController : Controller
    {
        /// <summary>
        ///     GET: api/User 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        ///     GET: api/User/GetUserName 
        /// </summary>
        /// <param name="userService"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("UserName")]
        public string GetUserName([FromServices] IUserService userService)
        {
            return userService.GetUserName();
        }

        /// <summary>
        ///     POST: api/User 
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        /// <summary>
        ///     PUT: api/User/5 
        /// </summary>
        /// <param name="id">    Id </param>
        /// <param name="value"> Value </param>
        [HttpPut("{id}")]
        public void Put([FromQuery]int id, [FromBody]string value)
        {
        }

        /// <summary>
        ///     api/ApiWithActions/5 
        /// </summary>
        /// <param name="id"> Id </param>
        [HttpDelete("{id}")]
        public void Delete([FromQuery]int id)
        {
        }
    }
}