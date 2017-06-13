#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> HtmlHelperExtensions.cs </Name>
//         <Created> 07/06/2017 10:17:27 PM </Created>
//         <Key> 77a262cc-b93b-4589-8b10-9808f16b8421 </Key>
//     </File>
//     <Summary>
//         HtmlHelperExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Puppy.Web.SEO.Twitter
{
    /// <summary>
    ///     Creates Twitter card meta tags. <see cref="TwitterCard" /> for more information. 
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        ///     Creates a string containing the Twitter card meta tags. <see cref="TwitterCard" />
        ///     for more information.
        /// </summary>
        /// <param name="htmlHelper">  The HTML helper. </param>
        /// <param name="twitterCard"> The Twitter card metadata. </param>
        /// <returns> The Twitter card's HTML meta tags. </returns>
        public static HtmlString TwitterCard(this IHtmlHelper htmlHelper, TwitterCard twitterCard)
        {
            return new HtmlString(twitterCard.ToString());
        }
    }
}