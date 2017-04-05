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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using TopCore.Auth.Domain.Entities;
using TopCore.Auth.Domain.Services;
using TopCore.Framework.DependencyInjection.Attributes;

namespace TopCore.Auth.Service
{
    [PerRequestDependency(ServiceType = typeof(IProfileService))]
    public class ProfileService : IProfileService, IdentityServer4.Services.IProfileService
    {
        private readonly UserManager<UserEntity> _userManager;

        public ProfileService(UserManager<UserEntity> userManager)
        {
            _userManager = userManager;
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
                    var security_stamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _userManager.GetSecurityStampAsync(user).ConfigureAwait(true);
                        if (db_security_stamp != security_stamp)
                            return;
                    }
                }

                context.IsActive = !user.LockoutEnabled || !user.LockoutEnd.HasValue || user.LockoutEnd <= DateTime.UtcNow;
            }
        }

        private async Task<IEnumerable<Claim>> GetClaimsFromUser(UserEntity userEntity)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, userEntity.Id),
                new Claim(JwtClaimTypes.PreferredUserName, userEntity.UserName)
            };

            if (_userManager.SupportsUserEmail)
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.Email, userEntity.Email),
                    new Claim(JwtClaimTypes.EmailVerified, userEntity.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }

            if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(userEntity.PhoneNumber))
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.PhoneNumber, userEntity.PhoneNumber),
                    new Claim(JwtClaimTypes.PhoneNumberVerified, userEntity.PhoneNumberConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }

            if (_userManager.SupportsUserClaim)
            {
                claims.AddRange(await _userManager.GetClaimsAsync(userEntity).ConfigureAwait(true));
            }

            if (_userManager.SupportsUserRole)
            {
                var roles = await _userManager.GetRolesAsync(userEntity).ConfigureAwait(true);
                claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
            }

            return claims;
        }
    }
}