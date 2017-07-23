#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> MarkdownTagHelper.cs </Name>
//         <Created> 23/07/17 1:33:32 PM </Created>
//         <Key> 4b4fbb82-75f3-404f-a235-108599716853 </Key>
//     </File>
//     <Summary>
//         MarkdownTagHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Puppy.Web.Markdown
{
    [HtmlTargetElement(Markdown, TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement(Attributes = Markdown)]
    public class MarkdownTagHelper : TagHelper
    {
        private const string Markdown = "markdown";

        [HtmlAttributeName("asp-for")]
        public ModelExpression AspFor { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (output.TagName == Markdown)
            {
                output.TagName = null;
            }
            output.Attributes.RemoveAll(Markdown);
            var markdownTransformer = new MarkdownTransformer();
            var content = await GetContentAsync(output).ConfigureAwait(true);
            var html = markdownTransformer.Transform(content);
            output.Content.SetHtmlContent(html ?? string.Empty);
        }

        private async Task<string> GetContentAsync(TagHelperOutput output)
        {
            return AspFor == null ? (await output.GetChildContentAsync().ConfigureAwait(true)).GetContent() : AspFor.Model?.ToString();
        }
    }
}