#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> CardApp.cs </Name>
//         <Created> 07/06/2017 10:21:26 PM </Created>
//         <Key> 347dd807-edc4-44c8-a8a0-db7a0c4e5ff4 </Key>
//     </File>
//     <Summary>
//         CardApp.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Text;

namespace Puppy.Web.SEO.Twitter.Cards
{
    /// <summary>
    ///     The App Card is a great way to represent mobile applications on Twitter and to drive
    ///     installs. The app card is designed to allow for a name, description and icon, and also to
    ///     highlight attributes such as the rating and the price. This Card type is currently
    ///     available on the twitter.com website, as well as iOS and Android mobile clients. It is
    ///     not yet available on mobile web. See https://dev.twitter.com/cards/types/app.
    /// </summary>
    [HtmlTargetElement("twitter-card-app", Attributes = SiteUsernameAttributeName,
        TagStructure = TagStructure.WithoutEndTag)]
    public class CardApp : TwitterCard
    {
        private const string CountryAttributeName = "country";
        private const string DescriptionAttributeName = "description";
        private const string GooglePlayAttributeName = "google-play";
        private const string GooglePlayCustomUrlSchemeAttributeName = "google-play-custom-url-scheme";
        private const string IPadAttributeName = "ipad";
        private const string IPadCustomUrlSchemeAttributeName = "iphone-custom-url-scheme";
        private const string IPhoneAttributeName = "ipad";
        private const string IPhoneCustomUrlSchemeAttributeName = "iphone-custom-url-scheme";

        /// <summary>
        ///     Gets or sets the country. If your application is not available in the US App Store,
        ///     you must set this value to the two-letter country code for the App Store that
        ///     contains your application.
        /// </summary>
        [HtmlAttributeName(CountryAttributeName)]
        public string Country { get; set; }

        /// <summary>
        ///     Gets or sets the description that concisely summarizes the content of the page, as
        ///     appropriate for presentation within a Tweet. Do not re-use the title text as the
        ///     description, or use this field to describe the general services provided by the
        ///     website. Description text will be truncated at the word to 200 characters. If you are
        ///     using Facebook's Open Graph og:description, do not use this unless you want a
        ///     different description.
        /// </summary>
        [HtmlAttributeName(DescriptionAttributeName)]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the numeric representation of your app ID in Google Play (.i.e. "com.android.app"). 
        /// </summary>
        [HtmlAttributeName(GooglePlayAttributeName)]
        public string GooglePlay { get; set; }

        /// <summary>
        ///     Gets or sets your google play app’s custom URL scheme (you must include "://" after
        ///     your scheme name).
        /// </summary>
        [HtmlAttributeName(GooglePlayCustomUrlSchemeAttributeName)]
        public string GooglePlayCustomUrlScheme { get; set; }

        /// <summary>
        ///     Gets or sets numeric representation of your iPad app ID in the App Store (.i.e. "307234931"). 
        /// </summary>
        [HtmlAttributeName(IPadAttributeName)]
        public string IPad { get; set; }

        /// <summary>
        ///     Gets or sets your iPad app’s custom URL scheme (you must include "://" after your
        ///     scheme name).
        /// </summary>
        [HtmlAttributeName(IPadCustomUrlSchemeAttributeName)]
        public string IPadCustomUrlScheme { get; set; }

        /// <summary>
        ///     Gets or sets numeric representation of your iPhone app ID in the App Store (.i.e. “307234931”).
        /// </summary>
        [HtmlAttributeName(IPhoneAttributeName)]
        public string IPhone { get; set; }

        /// <summary>
        ///     Gets or sets your iPhone app’s custom URL scheme (you must include "://" after your
        ///     scheme name).
        /// </summary>
        [HtmlAttributeName(IPhoneCustomUrlSchemeAttributeName)]
        public string IPhoneCustomUrlScheme { get; set; }

        /// <summary>
        ///     Gets the type of the Twitter card. 
        /// </summary>
        public override CardType Type => CardType.App;

        /// <summary>
        ///     Appends a HTML-encoded string representing this instance to the
        ///     <paramref name="stringBuilder" /> containing the Twitter card meta tags.
        /// </summary>
        /// <param name="stringBuilder"> The string builder. </param>
        public override void ToString(StringBuilder stringBuilder)
        {
            base.ToString(stringBuilder);

            stringBuilder.AppendMetaNameContentIfNotNull("twitter:description", Description);
            stringBuilder.AppendMetaNameContentIfNotNull("twitter:app:id:iphone", IPhone);
            stringBuilder.AppendMetaNameContentIfNotNull("twitter:app:url:iphone", IPhoneCustomUrlScheme);
            stringBuilder.AppendMetaNameContentIfNotNull("twitter:app:id:ipad", IPad);
            stringBuilder.AppendMetaNameContentIfNotNull("twitter:app:url:ipad", IPadCustomUrlScheme);
            stringBuilder.AppendMetaNameContentIfNotNull("twitter:app:id:googleplay", GooglePlay);
            stringBuilder.AppendMetaNameContentIfNotNull("twitter:app:url:googleplay", GooglePlayCustomUrlScheme);
            stringBuilder.AppendMetaNameContentIfNotNull("twitter:app:country", Country);
        }

        /// <summary>
        ///     Checks that this instance is valid and throws exceptions if not valid. 
        /// </summary>
        protected override void Validate()
        {
            base.Validate();

            if (string.IsNullOrEmpty(SiteUsername))
                throw new ArgumentNullException(nameof(SiteUsername));

            if (string.IsNullOrEmpty(IPhone))
                throw new ArgumentNullException(nameof(IPhone));

            if (string.IsNullOrEmpty(IPad))
                throw new ArgumentNullException(nameof(IPad));

            if (string.IsNullOrEmpty(GooglePlay))
                throw new ArgumentNullException(nameof(GooglePlay));
        }
    }
}