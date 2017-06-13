#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ReferrerModeTagHelper.cs </Name>
//         <Created> 07/06/2017 10:55:03 PM </Created>
//         <Key> fd86bc3a-c675-4786-85f5-73c43c1e0460 </Key>
//     </File>
//     <Summary>
//         ReferrerModeTagHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Puppy.Web.Referrer
{
    /// <summary>
    ///     Meta tag <see cref="TagHelper" /> which controls what is sent in the HTTP referrer header
    ///     when a client navigates from your page to an external site.
    /// </summary>
    [HtmlTargetElement("meta", Attributes = ReferrerAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class ReferrerModeTagHelper : TagHelper
    {
        private const string ContentAttributeName = "content";
        private const string NameAttributeName = "name";
        private const string ReferrerAttributeName = "asp-referrer";

        /// <summary>
        ///     Gets or sets the referrer mode, which controls what is sent in the HTTP referrer
        ///     header when a client navigates from your page to an external site. This is a required property.
        /// </summary>
        [HtmlAttributeName(ReferrerAttributeName)]
        public ReferrerMode Referrer { get; set; }

        /// <summary>
        ///     Synchronously executes the <see cref="TagHelper" /> with the given context and output. 
        /// </summary>
        /// <param name="context">
        ///     Contains information associated with the current HTML tag.
        /// </param>
        /// <param name="output">  A stateful HTML element used to generate an HTML tag. </param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute(NameAttributeName, "referrer");
            output.Attributes.SetAttribute(ContentAttributeName, Referrer.ToLowercaseString());
        }
    }
}