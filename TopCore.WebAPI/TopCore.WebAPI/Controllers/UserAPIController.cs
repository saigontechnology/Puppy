using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TopCore.WebAPI.Controllers.API
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserAPIController : Controller
    {
        // GET: api/User
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public string Get([FromQuery]int id)
        {
            return "value";
        }

        // POST: api/User
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public void Put([FromQuery]int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete([FromQuery]int id)
        {
        }
    }
}
