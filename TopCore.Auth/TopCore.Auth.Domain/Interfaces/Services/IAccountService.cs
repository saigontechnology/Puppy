#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Topcore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Topcore.WebAPI → Interface </Project>
//     <File>
//         <Name> IAccountService.cs </Name>
//         <Created> 29/05/2017 10:47:51 AM </Created>
//         <Key> 1f989e59-4958-49db-abff-510fccf16650 </Key>
//     </File>
//     <Summary>
//         IAccountService.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.Threading.Tasks;

namespace TopCore.Auth.Domain.Interfaces.Services
{
    public interface IAccountService
    {
        /// <summary>
        ///     Generate and send OTP as a password for user. Add user if not exist 
        /// </summary>
        Task SendOtp(string phoneOrEmail, string requestIpAddress, string clientId);
    }
}