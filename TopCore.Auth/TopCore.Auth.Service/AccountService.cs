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
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using TopCore.Auth.Domain.Entities;
using TopCore.Auth.Domain.Exceptions;
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
        private readonly IConfigurationRoot _configurationRoot;
        private readonly IRepository<OtpTracking> _otpTrackingRepository;

        public AccountService(UserManager<User> userManager, IRepository<User> userRepository, IConfigurationRoot configurationRoot, IRepository<OtpTracking> otpTrackingRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _configurationRoot = configurationRoot;
            _otpTrackingRepository = otpTrackingRepository;
        }

        public async Task SendOtp(string phoneOrEmail, string requestIpAddress, string clientId)
        {
            phoneOrEmail = phoneOrEmail?.Trim();
            string phoneOrEmailNormalize = StringHelper.Normalize(phoneOrEmail);

            bool isEmailOtp = false;
            string phoneNumber = null;
            string email = null;
            User user = null;

            // Get phone or email
            if (StringHelper.IsValidEmail(phoneOrEmail))
            {
                isEmailOtp = true;
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
                throw new TopCoreException(ErrorCode.BadRequest, $"{nameof(phoneOrEmail)} is no valid phone number or email", (nameof(phoneOrEmail), phoneOrEmail));
            }

            // Generate OTP as a pasword of user
            string otp = StringHelper.GetRandomNumber(_configurationRoot.GetValue<int>("UserSecurity:PasswordRequiredLength"));

            if (user == null)
            {
                // Create new user and set OTP as a password
                user = new User
                {
                    UserName = phoneOrEmail,
                    NormalizedUserName = phoneOrEmailNormalize,
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
                // Check if user already have otp not yes expire
                if (user.PasswordExpireTime > SystemUtils.GetSystemTimeNow())
                {
                    throw new TopCoreException(ErrorCode.AlreadyHaveOtpNotExpire);
                }

                // Set new OTP as a password
                var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, resetPasswordToken, otp);

                // Update password expire time and user name
                user.PasswordExpireTime = SystemUtils.GetSystemTimeNow().AddMinutes(2);

                if (string.CompareOrdinal(phoneOrEmailNormalize, user.NormalizedUserName) == 0)
                {
                    user.UserName = phoneOrEmail;
                    user.NormalizedUserName = phoneOrEmailNormalize;
                }

                await _userManager.UpdateAsync(user);
            }

            // Add tracking OTP
            OtpTracking otpTracking = new OtpTracking
            {
                UserId = user.Id,
                RequestIpAddress = requestIpAddress
            };

            _otpTrackingRepository.Add(otpTracking);
            _otpTrackingRepository.SaveChanges();

            Console.WriteLine("OTP: " + otp);

            // Send OTP
            if (isEmailOtp)
            {
                // TODO send otp via email
            }
            else
            {
                // TODO send otp via phone number
            }
        }
    }
}