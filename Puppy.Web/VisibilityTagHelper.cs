#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> VisibilityTagHelper.cs </Name>
//         <Created> 07/06/2017 10:56:06 PM </Created>
//         <Key> a35a5b76-2929-4fe7-b07f-5ae25beb8518 </Key>
//     </File>
//     <Summary>
//         VisibilityTagHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Puppy.Web
{
    /// <summary>
    ///     Determine whether a target element should be visible or not based on a conditional. 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Razor.TagHelpers.TagHelper" />
    [HtmlTargetElement(Attributes = IsVisibleAttributeName)]
    public class VisibilityTagHelper : TagHelper
    {
        private const string IsVisibleAttributeName = "asp-is-visible";

        /// <summary>
        ///     Gets or sets a value indicating whether the target instance is visible. 
        /// </summary>
        /// <value>
        ///     <c> true </c> if the target instance is visible; otherwise, <c> false </c>.
        /// </value>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        ///     Synchronously executes the
        ///     <see cref="T:Microsoft.AspNetCore.Razor.TagHelpers.TagHelper" /> with the given
        ///     <paramref name="context" /> and <paramref name="output" />.
        /// </summary>
        /// <param name="context"> Contains information associated with the current HTML tag. </param>
        /// <param name="output">  A stateful HTML element used to generate an HTML tag. </param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!this.IsVisible)
            {
                output.SuppressOutput();
            }

            base.Process(context, output);
        }
    }
}