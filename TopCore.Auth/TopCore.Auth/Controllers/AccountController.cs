using System;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TopCore.Auth.Domain.Entities;

namespace TopCore.Auth.Controllers
{
    public class AccountController : MvcController
    {
        private readonly UserManager<User> _userManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;

        public AccountController(UserManager<User> userManager, IIdentityServerInteractionService interaction, IClientStore clientStore)
        {
            _userManager = userManager;
            _interaction = interaction;
            _clientStore = clientStore;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string userName, string password, bool rememberLogin, string returnUrl)
        {
            var identityUser = await _userManager.FindByNameAsync(userName);

            if (identityUser != null && await _userManager.CheckPasswordAsync(identityUser, password))
            {
                AuthenticationProperties props = null;

                if (rememberLogin)
                {
                    props = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                    };
                };

                await HttpContext.Authentication.SignInAsync(identityUser.Id, identityUser.UserName, props);

                if (_interaction.IsValidReturnUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return Redirect("~/");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View();
        }
    }
}