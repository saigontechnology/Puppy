using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace TopCore.WebAPI.Controllers
{
    [Route("api/User")]
    [Produces("application/json")]
    public class UserAPIController : Controller
    {
        /// <summary>
        /// GET: api/User
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        ///  GET: api/User/5
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "Get")]
        public string Get([FromQuery]int id)
        {
            return "value";
        }

        /// <summary>
        /// POST: api/User
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        /// <summary>
        /// PUT: api/User/5
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="value">Value</param>
        [HttpPut("{id}")]
        public void Put([FromQuery]int id, [FromBody]string value)
        {
        }

        /// <summary>
        /// api/ApiWithActions/5
        /// </summary>
        /// <param name="id">Id</param>
        [HttpDelete("{id}")]
        public void Delete([FromQuery]int id)
        {
        }
    }
}
