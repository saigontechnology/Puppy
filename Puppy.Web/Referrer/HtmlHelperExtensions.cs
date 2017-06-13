#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> HtmlHelperExtensions.cs </Name>
//         <Created> 07/06/2017 10:57:07 PM </Created>
//         <Key> 3ea09771-78ef-418e-b6a0-30f4cf9e445a </Key>
//     </File>
//     <Summary>
//         HtmlHelperExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Puppy.Web.Referrer
{
    /// <summary>
    ///     <see cref="IHtmlHelper" /> extension methods. 
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        ///     Creates a string containing the referrer meta tags. <see cref="ReferrerMode" /> for
        ///     more information.
        /// </summary>
        /// <param name="htmlHelper">   The HTML helper. </param>
        /// <param name="referrerMode"> The type of referrer allowed to be sent. </param>
        /// <returns> The referrer meta tag. </returns>
        public static HtmlString ReferrerMeta(this IHtmlHelper htmlHelper, ReferrerMode referrerMode)
        {
            if (referrerMode == ReferrerMode.NoneWhenDowngrade)
                return null;

            return new HtmlString("<meta name=\"referrer\" content=\"" + referrerMode.ToLowercaseString() + "\">");
        }
    }
}