using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Threading.Tasks;
using TopCore.Auth.Domain.Interfaces.Services;

namespace TopCore.Auth.Controllers
{
    [Route("api/account")]
    public class AccountApiController : ApiController
    {
        private readonly IAccountService _accountSeeService;

        public AccountApiController(IAccountService accountSeeService)
        {
            _accountSeeService = accountSeeService;
        }

        /// <summary>
        ///     Get OTP 
        /// </summary>
        /// <param name="phoneOrEmail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("otp/{phoneOrEmail}")]
        [SwaggerResponse((int)HttpStatusCode.OK, null, "OTP")]
        public async Task<IActionResult> GetOtp([FromRoute]string phoneOrEmail)
        {
            string otp = await _accountSeeService.GetOtp(phoneOrEmail);
            return Ok(otp);
        }
    }
}