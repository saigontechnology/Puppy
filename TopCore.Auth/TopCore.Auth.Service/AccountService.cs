#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI </Project>
//     <File>
//         <Name> OtpService.cs </Name>
//         <Created> 29/05/2017 10:47:11 AM </Created>
//         <Key> 13c25a43-a5e5-45c5-bac1-000dd1f303c8 </Key>
//     </File>
//     <Summary>
//         OtpService.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TopCore.Auth.Domain.Entities;
using TopCore.Auth.Domain.Interfaces.Data;
using TopCore.Auth.Domain.Interfaces.Services;
using TopCore.Auth.Domain.Utils;
using TopCore.Framework.Core.StringUtils;
using TopCore.Framework.DependencyInjection.Attributes;

namespace TopCore.Auth.Service
{
    [PerRequestDependency(ServiceType = typeof(IAccountService))]
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<User> _userRepository;

        public AccountService(UserManager<User> userManager, IRepository<User> userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task<string> GetOtp(string phoneOrEmail)
        {
            bool isValidEmail = StringHelper.IsValidEmail(phoneOrEmail);
            string phoneNumber = null;
            string email = null;

            User user;
            if (isValidEmail)
            {
                email = phoneOrEmail;
                user = await _userManager.FindByEmailAsync(email);
            }
            else
            {
                phoneNumber = phoneOrEmail;
                user = _userRepository.Get(x => x.PhoneNumber == phoneNumber).FirstOrDefault();
            }
            // Generate OTP as a pasword of user
            string otp = StringHelper.GetRandomNumber(4);

            if (user == null)
            {
                // Create new user and set OTP as a password
                user = new User
                {
                    UserName = phoneNumber,
                    NormalizedUserName = phoneNumber,
                    Email = email,
                    NormalizedEmail = StringHelper.Normalize(email),
                    EmailConfirmed = true,
                    PhoneNumber = phoneNumber,
                    PasswordExpireTime = SystemUtils.GetSystemTimeNow().AddMinutes(2),
                    PhoneNumberConfirmed = false,
                    Claims =
                    {
                        new IdentityUserClaim<string>
                        {
                            ClaimType = JwtClaimTypes.Name,
                            ClaimValue = phoneOrEmail,
                        }
                    }
                };

                await _userManager.CreateAsync(user, otp);
            }
            else
            {
                // Set new OTP as a password
                var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, resetPasswordToken, otp);
            }
            return otp;
        }
    }
}