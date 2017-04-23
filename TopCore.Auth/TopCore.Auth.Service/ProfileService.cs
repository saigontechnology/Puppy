#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Business.Logic </Project>
//     <File>
//         <Name> ProfileBusiness </Name>
//         <Created> 04 Apr 17 11:51:03 PM </Created>
//         <Key> b0dbd5eb-fb25-4fad-b888-3e206bcd726b </Key>
//     </File>
//     <Summary>
//         ProfileBusiness
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TopCore.Auth.Domain.Entities;
using TopCore.Auth.Domain.Interfaces.Data;
using TopCore.Auth.Domain.Interfaces.Services;
using TopCore.Framework.DependencyInjection.Attributes;

namespace TopCore.Auth.Service
{
    /// <summary>
    ///     Identity Server call flow: Check IsActiveAsync then GetProfileDataAsync 
    /// </summary>
    [PerRequestDependency(ServiceType = typeof(IProfileService))]
    public class ProfileService : IProfileService, IdentityServer4.Services.IProfileService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<User> _userRepository;

        public ProfileService(UserManager<User> userManager, IRepository<User> userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject;
            if (subject == null) throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(subjectId).ConfigureAwait(true);

            context.IsActive = false;
            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var securityStamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value).SingleOrDefault();
                    if (securityStamp != null)
                    {
                        var dbSecurityStamp = await _userManager.GetSecurityStampAsync(user).ConfigureAwait(true);
                        if (dbSecurityStamp != securityStamp)
                        {
                            return;
                        }
                    }
                }

                bool isLockout = !(!user.LockoutEnabled || !user.LockoutEnd.HasValue || user.LockoutEnd <= DateTime.UtcNow);
                if (isLockout)
                {
                    return;
                }

                context.IsActive = true;
            }
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject;
            if (subject == null)
            {
                throw new ArgumentNullException(nameof(context.Subject));
            }

            var subjectId = subject.GetSubjectId();

            var user = await _userManager.FindByIdAsync(subjectId).ConfigureAwait(true);
            if (user == null)
            {
                throw new ArgumentException("Invalid subject identifier");
            }

            var claims = await GetClaimsFromUser(user).ConfigureAwait(true);

            var requestedClaimTypes = context.RequestedClaimTypes;
            claims = requestedClaimTypes != null ? claims.Where(c => requestedClaimTypes.Contains(c.Type)) : claims.Take(0);
            context.IssuedClaims = claims.ToList();
        }

        private async Task<IEnumerable<Claim>> GetClaimsFromUser(User user)
        {
            User userData = _userRepository.Get(x => x.Id == user.Id).FirstOrDefault();

            if (userData == null)
            {
                throw new NullReferenceException($"User with id: {user.Id} not found.");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName)
            };

            if (_userManager.SupportsUserEmail)
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.Email, user.Email),
                    new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }

            if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                    new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }

            if (_userManager.SupportsUserClaim)
            {
                claims.AddRange(await _userManager.GetClaimsAsync(user).ConfigureAwait(true));
            }

            if (_userManager.SupportsUserRole)
            {
                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(true);
                claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
            }

            return claims;
        }
    }
}