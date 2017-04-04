using IdentityServer4.Services;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TopCore.SSO.Identity;
using TopCore.SSO.ViewModels;

namespace TopCore.SSO.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly UserManager<TopCoreIdentityUser> _userManager;

        public UserController(UserManager<TopCoreIdentityUser> userManager,
            IIdentityServerInteractionService interaction)
        {
            _userManager = userManager;
            _interaction = interaction;
        }

        /// <summary>
        ///     Login 
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.FindByNameAsync(loginViewModel.UserName).ConfigureAwait(true);

                if (identityUser != null && await _userManager.CheckPasswordAsync(identityUser, loginViewModel.Password).ConfigureAwait(true))
                {
                    AuthenticationProperties properties = null;
                    if (loginViewModel.RememberMe)
                    {
                        properties = new AuthenticationProperties
                        {
                            IsPersistent = true
                        };
                    }

                    await HttpContext.Authentication.SignInAsync(identityUser.Id, identityUser.UserName).ConfigureAwait(true);

                    return Ok(properties);
                }

                return BadRequest("Invalid username or password.");
            }

            return BadRequest(ModelState);
        }
    }
}