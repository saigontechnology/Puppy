#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> CardSummaryLargeImage.cs </Name>
//         <Created> 07/06/2017 10:25:03 PM </Created>
//         <Key> 7b5f309d-f5ae-400f-b3e9-1bd963e4cbe4 </Key>
//     </File>
//     <Summary>
//         CardSummaryLargeImage.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace Puppy.Web.SEO.Twitter.Cards
{
    /// <summary>
    ///     The Summary Card with Large Image features a large, full-width prominent image alongside
    ///     a tweet. It is designed to give the reader a rich photo experience, and clicking on the
    ///     image brings the user to your website. On twitter.com and the mobile clients, the image
    ///     appears below the tweet text. See https://dev.twitter.com/cards/types/summary-large-image.
    /// </summary>
    [HtmlTargetElement("twitter-card-summary-large-image",
        Attributes = SiteUsernameAttributeName + "," + CreatorUsernameAttributeName,
        TagStructure = TagStructure.WithoutEndTag)]
    public class CardSummaryLargeImage : CardSummary
    {
        /// <summary>
        ///     Gets the type of the Twitter card. 
        /// </summary>
        public override CardType Type => CardType.SummaryLargeImage;

        /// <summary>
        ///     Appends a HTML-encoded string representing this instance to the
        ///     <paramref name="stringBuilder" /> containing the Twitter card meta tags.
        /// </summary>
        /// <param name="stringBuilder"> The string builder. </param>
        public override void ToString(StringBuilder stringBuilder)
        {
            base.ToString(stringBuilder);

            stringBuilder.AppendMetaNameContentIfNotNull("twitter:site:Id", SiteId);
            stringBuilder.AppendMetaNameContentIfNotNull("twitter:title", Title);
            stringBuilder.AppendMetaNameContentIfNotNull("twitter:description", Description);
            stringBuilder.AppendMetaNameContentIfNotNull("twitter:creator", CreatorUsername);
            stringBuilder.AppendMetaNameContentIfNotNull("twitter:creator:id", CreatorId);

            if (Image != null)
            {
                stringBuilder.AppendMetaNameContent("twitter:image", Image.ImageUrl);
                stringBuilder.AppendMetaNameContentIfNotNull("twitter:image:height", Image.Height);
                stringBuilder.AppendMetaNameContentIfNotNull("twitter:image:width", Image.Width);
            }
        }
    }
}