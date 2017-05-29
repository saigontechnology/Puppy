#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Domain.Exceptions </Project>
//     <File>
//         <Name> ExceptionCode </Name>
//         <Created> 12/04/2017 09:05:31 AM </Created>
//         <Key> f183ec48-2edb-4c90-b029-6ee78f6b6cdd </Key>
//     </File>
//     <Summary>
//         ExceptionCode
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TopCore.Auth.Domain.Exceptions
{
    /// <summary>
    ///     Error code for whole system, 
    ///     <code>
    ///  DisplayAttribute.Name
    ///     </code>
    ///     as a Group/Module, 
    ///     <code>
    ///  DescriptionAttribute
    ///     </code>
    ///     as a string format 
    /// </summary>
    public enum ErrorCode
    {
        // Global
        [Display(Name = "Global")]
        [Description("Bad Request")]
        BadRequest = 400,

        [Display(Name = "Global")]
        [Description("Un-Authenticate")]
        UnAuthenticated = 401,

        [Display(Name = "Global")]
        [Description("Forbidden, this feature for 18+ :))")]
        Unauthorized = 403,

        [Display(Name = "Global")]
        [Description("Awesome, You break the system :o. You know what they say, you get what you pay for... The features do not write themselves, you know. Now, just god and you know what happen.")]
        Unknown = 500,

        // User
        [Display(Name = "User")]
        [Description("OTP is expired.")]
        OtpExpired = 1001,

        [Display(Name = "User")]
        [Description("Subject Id is invalid.")]
        InvalidSubjectId = 1002,

        /// <summary>
        /// [Description("User with id: {0} not found.")]
        /// </summary>
        [Display(Name = "User")]
        [Description("User is not found.")]
        UserNotfound = 1003,
    }
}