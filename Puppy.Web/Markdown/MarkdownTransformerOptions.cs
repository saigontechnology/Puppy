#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> MarkdownTransformerOptions.cs </Name>
//         <Created> 23/07/17 2:03:01 PM </Created>
//         <Key> f2a27277-128b-423d-a633-ac85763a8efe </Key>
//     </File>
//     <Summary>
//         MarkdownTransformerOptions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.Web.Markdown
{
    public class MarkdownTransformerOptions
    {
        /// <summary>
        ///     when true, (most) bare plain URLs are auto-hyper-linked
        ///     WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool AutoHyperlink { get; set; } = true;

        /// <summary>
        ///     when true, RETURN becomes a literal newline
        ///     WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool AutoNewlines { get; set; } = true;

        /// <summary>
        ///     use "&gt;" for HTML output, or " /&gt;" for XHTML output 
        /// </summary>
        public string EmptyElementSuffix { get; set; } = " />";

        /// <summary>
        ///     when true, problematic URL characters like [, ], (, and so forth will be encoded
        ///     WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool EncodeProblemUrlCharacters { get; set; } = false;

        /// <summary>
        ///     when false, email addresses will never be auto-linked
        ///     WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool LinkEmails { get; set; } = true;

        /// <summary>
        ///     when true, bold and italic require non-word characters on either side
        ///     WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool StrictBoldItalic { get; set; } = false;
    }
}