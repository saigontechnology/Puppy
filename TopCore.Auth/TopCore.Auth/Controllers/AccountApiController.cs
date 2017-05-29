using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountApiController(IAccountService accountSeeService, IHttpContextAccessor httpContextAccessor)
        {
            _accountSeeService = accountSeeService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        ///     Send OTP 
        /// </summary>
        /// <param name="phoneOrEmail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("otp/{phoneOrEmail}")]
        [SwaggerResponse((int)HttpStatusCode.OK, null, "OTP Sent")]
        public async Task<IActionResult> SendOtp([FromRoute]string phoneOrEmail)
        {
            // TODO verify Client id, client secret with permission
            var requestIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            string clientId = "client id"; // TODO get it from identity server by request
            await _accountSeeService.SendOtp(phoneOrEmail, requestIpAddress, clientId);
            return Ok();
        }
    }
}