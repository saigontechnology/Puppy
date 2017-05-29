#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI </Project>
//     <File>
//         <Name> ResourceOwnerPasswordService.cs </Name>
//         <Created> 29/05/2017 4:50:49 PM </Created>
//         <Key> 1fc51bdb-847d-4c00-affc-7d556a39d8c1 </Key>
//     </File>
//     <Summary>
//         ResourceOwnerPasswordService.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopCore.Auth.Domain.Entities;
using TopCore.Auth.Domain.Exceptions;
using TopCore.Auth.Domain.Interfaces.Data;
using TopCore.Auth.Domain.Utils;
using TopCore.Auth.Domain.ViewModels;
using TopCore.Framework.Core.StringUtils;

namespace TopCore.Auth.Service
{
    public class ResourceOwnerPasswordService : IResourceOwnerPasswordValidator
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IRepository<User> _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ResourceOwnerPasswordService> _logger;

        public ResourceOwnerPasswordService(IRepository<User> userRepository, UserManager<User> userManager, SignInManager<User> signInManager, ILogger<ResourceOwnerPasswordService> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public virtual async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            string phoneOrEmail = context.UserName;
            phoneOrEmail = phoneOrEmail?.Trim();
            string phoneNumber = null;
            string email = null;
            User user = null;

            // Get phone or email
            if (StringHelper.IsValidEmail(phoneOrEmail))
            {
                email = phoneOrEmail;
                user = await _userManager.FindByEmailAsync(email);
            }
            else
            {
                if (StringHelper.IsValidPhoneNumber(phoneOrEmail))
                {
                    phoneNumber = phoneOrEmail;
                    string userId = _userRepository.Get(x => x.PhoneNumber == phoneNumber).Select(x => x.Id).FirstOrDefault();
                    user = await _userManager.FindByIdAsync(userId);
                }
            }

            // Validate
            if (email == null && phoneNumber == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidClient);
                return;
            }

            if (user != null)
            {
                SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, context.Password, true);
                if (result.Succeeded)
                {
                    // Check token expire
                    DateTimeOffset systemTimeNow = SystemUtils.GetSystemTimeNow();
                    if (user.PasswordExpireTime < systemTimeNow)
                    {
                        ApiErrorViewModel errorViewModel = new ApiErrorViewModel(ErrorCode.OtpExpired);
                        string errorJson = JsonConvert.SerializeObject(errorViewModel, new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            NullValueHandling = NullValueHandling.Ignore,
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
                        Dictionary<string, object> errorDictionary =
                            JsonConvert.DeserializeObject<Dictionary<string, object>>(errorJson);

                        context.Result = new GrantValidationResult(errorDictionary);
                        return;
                    }

                    // Token is valid, set token expired
                    user.PasswordExpireTime = systemTimeNow;

                    if (!user.EmailConfirmed && user.UserName == user.Email)
                    {
                        user.EmailConfirmed = true;
                    }

                    if (!user.PhoneNumberConfirmed && user.UserName == user.PhoneNumber)
                    {
                        user.PhoneNumberConfirmed = true;
                    }
                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation("Credentials validated for username: {username}", context.UserName);
                    string userIdAsync = await _userManager.GetUserIdAsync(user);
                    context.Result = new GrantValidationResult(userIdAsync, "pwd");

                    return;
                }
                if (result.IsLockedOut)
                    _logger.LogInformation("Authentication failed for username: {username},  reason: locked out", context.UserName);
                else if (result.IsNotAllowed)
                    _logger.LogInformation("Authentication failed for username: {username},  reason: not allowed", context.UserName);
                else
                    _logger.LogInformation("Authentication failed for username: {username},  reason: invalid credentials", context.UserName);
            }
            else
                _logger.LogInformation("No user found matching username: {username}", context.UserName);
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
        }
    }
}