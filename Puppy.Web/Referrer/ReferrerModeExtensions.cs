#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ReferrerModeExtensions.cs </Name>
//         <Created> 07/06/2017 10:54:48 PM </Created>
//         <Key> c70c1b2a-1394-4106-af14-e38780ac484a </Key>
//     </File>
//     <Summary>
//         ReferrerModeExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

namespace Puppy.Web.Referrer
{
    /// <summary>
    ///     <see cref="ReferrerMode" /> extension methods. 
    /// </summary>
    internal static class ReferrerModeExtensions
    {
        /// <summary>
        ///     Returns the lower-case <see cref="string" /> representation of the <see cref="ReferrerMode" />. 
        /// </summary>
        /// <param name="referrerMode"> The referrer mode. </param>
        /// <returns> The lower-case <see cref="string" /> representation of the <see cref="ReferrerMode" />. </returns>
        public static string ToLowercaseString(this ReferrerMode referrerMode)
        {
            switch (referrerMode)
            {
                case ReferrerMode.None:
                    return "none";

                case ReferrerMode.NoneWhenDowngrade:
                    return "none-when-downgrade";

                case ReferrerMode.Origin:
                    return "origin";

                case ReferrerMode.OriginWhenCrossOrigin:
                    return "origin-when-crossorigin";

                case ReferrerMode.UnsafeUrl:
                    return "unsafe-url";

                default:
                    return string.Empty;
            }
        }
    }
}