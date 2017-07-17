#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> TwitterCard.cs </Name>
//         <Created> 07/06/2017 10:14:21 PM </Created>
//         <Key> da6402a6-8147-496b-b492-16652c50c8ce </Key>
//     </File>
//     <Summary>
//         TwitterCard.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Razor.TagHelpers;
using Puppy.Web.SEO.Twitter.Enums;
using System.Text;

namespace Puppy.Web.SEO.Twitter
{
    /// <summary>
    ///     With Twitter Cards, you can attach rich photos, videos and media experience to Tweets
    ///     that drive traffic to your website. Users who Tweet links to your content will have a
    ///     "Card" added to the Tweet that's visible to all of their followers. See
    ///     https://dev.twitter.com/cards/overview. Sign up for Twitter Card analytics to see who is
    ///     sharing your site pages on Twitter. See https://analytics.twitter.com/about Validate your
    ///     Twitter cards. See https://twitter.com/login?redirect_after_login=https%3A%2F%2Fcards-dev.twitter.com%2Fvalidator
    /// </summary>
    public abstract class TwitterCard : TagHelper
    {
        /// <summary>
        ///     The Twitter user ID of content creator. 
        /// </summary>
        protected const string CreatorIdAttributeName = "creator-id";

        /// <summary>
        ///     The Twitter @username of content creator e.g. @Top. 
        /// </summary>
        protected const string CreatorUsernameAttributeName = "creator";

        /// <summary>
        ///     The Twitter Id associated with this site. 
        /// </summary>
        protected const string SiteIdAttributeName = "site-id";

        /// <summary>
        ///     The Twitter @username associated with the page e.g. @Microsoft. This is a required
        ///     property. Required for Twitter Card analytic.
        /// </summary>
        protected const string SiteUsernameAttributeName = "site";

        /// <summary>
        ///     Gets or sets the Twitter user ID of content creator. 
        /// </summary>
        /// <value> The Twitter user ID of content creator. </value>
        [HtmlAttributeName(CreatorIdAttributeName)]
        public string CreatorId { get; set; }

        /// <summary>
        ///     Gets or sets the Twitter @username of content creator e.g. @Top. 
        /// </summary>
        [HtmlAttributeName(CreatorUsernameAttributeName)]
        public string CreatorUsername { get; set; }

        /// <summary>
        ///     Gets or sets the Site's Twitter site Id. Either twitter:site or twitter:site:id is required.
        /// </summary>
        /// <value> The twitter Id for the site. </value>
        [HtmlAttributeName(SiteIdAttributeName)]
        public string SiteId { get; set; }

        /// <summary>
        ///     Gets or sets the twitter site username. e.g. @Microsoft. 
        /// </summary>
        /// <value>
        ///     The Site's Twitter @username the card should be attributed to. Required for Twitter
        ///     Card analytic.
        /// </value>
        [HtmlAttributeName(SiteUsernameAttributeName)]
        public string SiteUsername { get; set; }

        /// <summary>
        ///     Gets the type of the Twitter card. 
        /// </summary>
        public abstract CardType Type { get; }

        /// <summary>
        ///     Synchronously executes the <see cref="TagHelper" /> with the given
        ///     <paramref name="context" /> and <paramref name="output" />.
        /// </summary>
        /// <param name="context">
        ///     Contains information associated with the current HTML tag.
        /// </param>
        /// <param name="output">  A stateful HTML element used to generate an HTML tag. </param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.SetHtmlContent(ToString());
            output.TagName = null;
        }

        /// <summary>
        ///     Returns a HTML-encoded <see cref="string" /> that represents this instance containing
        ///     the Twitter card meta tags.
        /// </summary>
        /// <returns>
        ///     A HTML-encoded <see cref="string" /> that represents this instance containing the
        ///     Twitter card meta tags.
        /// </returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            ToString(stringBuilder);
            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Appends a HTML-encoded string representing this instance to the
        ///     <paramref name="stringBuilder" /> containing the Twitter card meta tags.
        /// </summary>
        /// <param name="stringBuilder"> The string builder. </param>
        public virtual void ToString(StringBuilder stringBuilder)
        {
            Validate();

            stringBuilder.AppendMetaNameContent("twitter:card", Type.ToTwitterString());

            if (!string.IsNullOrEmpty(SiteUsername))
                stringBuilder.AppendMetaNameContent("twitter:site", SiteUsername);
        }

        /// <summary>
        ///     Checks that this instance is valid and throws exceptions if not valid. 
        /// </summary>
        protected virtual void Validate()
        {
        }
    }
}