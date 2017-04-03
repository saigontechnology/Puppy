using IdentityModel;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TopCore.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;

        public UserController(UserManager<IdentityUser> userManager, IIdentityServerInteractionService interaction, IClientStore clientStore)
        {
            _userManager = userManager;
            _interaction = interaction;
            _clientStore = clientStore;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.FindByNameAsync(model.Username).ConfigureAwait(true);

                if (identityUser != null && await _userManager.CheckPasswordAsync(identityUser, model.Password).ConfigureAwait(true))
                {
                    AuthenticationProperties props = null;

                    if (model.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1)
                        };
                    };

                    await HttpContext.Authentication.SignInAsync(identityUser.Id, identityUser.UserName).ConfigureAwait(true);

                    if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return Redirect("~/");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var info = await HttpContext.Authentication.GetAuthenticateInfoAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme).ConfigureAwait(true);
            var tempUser = info?.Principal;
            if (tempUser == null)
            {
                throw new Exception("External authentication error");
            }

            var claims = tempUser.Claims.ToList();

            var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userid");
            }

            claims.Remove(userIdClaim);
            var provider = info.Properties.Items["scheme"];
            var userId = userIdClaim.Value;

            var user = await _userManager.FindByLoginAsync(provider, userId).ConfigureAwait(true);
            if (user == null)
            {
                user = new IdentityUser { UserName = Guid.NewGuid().ToString() };
                await _userManager.CreateAsync(user).ConfigureAwait(true);
                await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, userId, provider)).ConfigureAwait(true);
            }

            var additionalClaims = new List<Claim>();

            var sid = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            await HttpContext.Authentication
                .SignInAsync(user.Id, user.UserName, provider, additionalClaims.ToArray()).ConfigureAwait(true);

            await HttpContext.Authentication
                .SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme).ConfigureAwait(true);

            return Redirect(_interaction.IsValidReturnUrl(returnUrl) ? returnUrl : "~/");
        }
    }

    public class LoginInputModel
    {
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}