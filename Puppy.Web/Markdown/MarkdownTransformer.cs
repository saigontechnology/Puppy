#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> MarkdownTransformer.cs </Name>
//         <Created> 23/07/17 1:59:14 PM </Created>
//         <Key> 7e5d1974-399d-41b1-9093-b40b4e7ed48f </Key>
//     </File>
//     <Summary>
//         MarkdownTransformer.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Puppy.Web.Markdown
{
    /// <summary>
    ///     Markdown is a text-to-HTML conversion tool for web writers. Markdown allows you to write
    ///     using an easy-to-read, easy-to-write plain text format, then convert it to structurally
    ///     valid XHTML (or HTML).
    /// </summary>
    public class MarkdownTransformer
    {
        /// <summary>
        ///     maximum nested depth of [] and () supported by the transform; implementation detail 
        /// </summary>
        private const int NestDepth = 6;

        /// <summary>
        ///     Tabs are automatically converted to spaces as part of the transform this constant
        ///     determines how "wide" those tabs become in spaces
        /// </summary>
        private const int TabWidth = 4;

        private const string MarkerUl = @"[*+-]";
        private const string MarkerOl = @"\d+[.]";

        private const string CharInsideUrl = @"[-A-Z0-9+&@#/%?=~_|\[\]\(\)!:,\.;" + "\x1a]";
        private const string CharEndingUrl = "[-A-Z0-9+&@#/%=~_|\\[\\])]";

        private static readonly Dictionary<string, string> EscapeTable;
        private static readonly Dictionary<string, string> InvertedEscapeTable;
        private static readonly Dictionary<string, string> BackslashEscapeTable;

        private static readonly string AutoLinkPreventionMarker = "\x1AP"; // temporarily replaces "://" where auto-linking shouldn't happen;

        private static readonly Regex NewlinesLeadingTrailing = new Regex(@"^\n+|\n+\z", RegexOptions.Compiled);
        private static readonly Regex NewlinesMultiple = new Regex(@"\n{2,}", RegexOptions.Compiled);
        private static readonly Regex LeadingWhitespace = new Regex(@"^[ ]*", RegexOptions.Compiled);

        private static readonly Regex HtmlBlockHash = new Regex("\x1AH\\d+H", RegexOptions.Compiled);

        private static string _nestedBracketsPattern;

        private static string _nestedParensPattern;

        private static readonly Regex LinkDef = new Regex(string.Format(@"
                        ^[ ]{{0,{0}}}\[([^\[\]]+)\]:  # id = $1
                          [ ]*
                          \n?                   # maybe *one* newline
                          [ ]*
                        <?(\S+?)>?              # url = $2
                          [ ]*
                          \n?                   # maybe one newline
                          [ ]*
                        (?:
                            (?<=\s)             # lookbehind for whitespace
                            [""(]
                            (.+?)               # title = $3
                            ["")]
                            [ ]*
                        )?                      # title is optional
                        (?:\n+|\Z)", TabWidth - 1),
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        // compiling this monster regex results in worse performance. trust me.
        private static readonly Regex BlocksHtml =
            new Regex(GetBlockPattern(), RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex HtmlTokens = new Regex(@"
            (<!--(?:|(?:[^>-]|-[^>])(?:[^-]|-[^-])*)-->)|        # match <!-- foo -->
            (<\?.*?\?>)|                 # match <?foo?> " +
                                                              RepeatString(@"
            (<[A-Za-z\/!$](?:[^<>]|", NestDepth) + RepeatString(@")*>)", NestDepth) +
                                                              " # match <tag> and </tag>",
            RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.ExplicitCapture |
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex AnchorRef = new Regex(string.Format(@"
            (                               # wrap whole match in $1
                \[
                    ({0})                   # link text = $2
                \]

                [ ]?                        # one optional space
                (?:\n[ ]*)?                 # one optional newline followed by spaces

                \[
                    (.*?)                   # id = $3
                \]
            )", GetNestedBracketsPattern()),
            RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex AnchorInline = new Regex(string.Format(@"
                (                           # wrap whole match in $1
                    \[
                        ({0})               # link text = $2
                    \]
                    \(                      # literal paren
                        [ ]*
                        ({1})               # href = $3
                        [ ]*
                        (                   # $4
                        (['""])           # quote char = $5
                        (.*?)               # title = $6
                        \5                  # matching quote
                        [ ]*                # ignore any spaces between closing quote and )
                        )?                  # title is optional
                    \)
                )", GetNestedBracketsPattern(), GetNestedParensPattern()),
            RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex AnchorRefShortcut = new Regex(@"
            (                               # wrap whole match in $1
              \[
                 ([^\[\]]+)                 # link text = $2; can't contain [ or ]
              \]
            )", RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex ImagesRef = new Regex(@"
                    (               # wrap whole match in $1
                    !\[
                        (.*?)       # alt text = $2
                    \]

                    [ ]?            # one optional space
                    (?:\n[ ]*)?     # one optional newline followed by spaces

                    \[
                        (.*?)       # id = $3
                    \]

                    )", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex ImagesInline = new Regex(string.Format(@"
              (                     # wrap whole match in $1
                !\[
                    (.*?)           # alt text = $2
                \]
                \s?                 # one optional whitespace character
                \(                  # literal paren
                    [ ]*
                    ({0})           # href = $3
                    [ ]*
                    (               # $4
                    (['""])       # quote char = $5
                    (.*?)           # title = $6
                    \5              # matching quote
                    [ ]*
                    )?              # title is optional
                \)
              )", GetNestedParensPattern()),
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex HeaderText = new Regex(@"
                ^(.+?)
                [ ]*
                \n
                (=+|-+)     # $1 = string of ='s or -'s
                [ ]*
                \n+",
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex HeaderAtx = new Regex(@"
                ^(\#{1,6})  # $1 = string of #'s
                [ ]*
                (.+?)       # $2 = ProcessingTimeResponseHeader text
                [ ]*
                \#*         # optional closing #'s (not counted)
                \n+",
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex HorizontalRules = new Regex(@"
            ^[ ]{0,3}         # Leading space
                ([-*_])       # $1: First marker
                (?>           # Repeated marker group
                    [ ]{0,2}  # Zero, one, or two spaces.
                    \1        # Marker character
                ){2,}         # Group repeated at least twice
                [ ]*          # Trailing spaces
                $             # End of line.
            ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly string WholeList = string.Format(@"
            (                               # $1 = whole list
              (                             # $2
                [ ]{{0,{1}}}
                ({0})                       # $3 = first list item marker
                [ ]+
              )
              (?s:.+?)
              (                             # $4
                  \z
                |
                  \n{{2,}}
                  (?=\S)
                  (?!                       # Negative lookahead for another list item marker
                    [ ]*
                    {0}[ ]+
                  )
              )
            )", string.Format("(?:{0}|{1})", MarkerUl, MarkerOl), TabWidth - 1);

        private static readonly Regex ListNested = new Regex(@"^" + WholeList,
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex ListTopLevel = new Regex(@"(?:(?<=\n\n)|\A\n?)" + WholeList,
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex CodeBlock = new Regex(string.Format(@"
                    (?:\n\n|\A\n?)
                    (                        # $1 = the code block -- one or more lines, starting with a space
                    (?:
                        (?:[ ]{{{0}}})       # Lines must start with a tab-width of spaces
                        .*\n+
                    )+
                    )
                    ((?=^[ ]{{0,{0}}}[^ \t\n])|\Z) # Lookahead for non-space at line-start, or end of doc",
            TabWidth), RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex GithubCodeBlock =
            new Regex(@"(?<!\\)(`{3,}) *(\S+)? *\n([\s\S]+?)\s*\1 *(?:\n+|$)", RegexOptions.Compiled);

        private static readonly Regex CodeSpan = new Regex(@"
                    (?<!\\)   # Character before opening ` can't be a backslash
                    (\`)      # $1 = Opening run of `
                    (.+?)     # $2 = The code block
                    (?<!`)
                    \1
                    (?!`)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex Bold = new Regex(@"(\*\*|__) (?=\S) (.+?[*_]*) (?<=\S) \1",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex StrictBold =
            new Regex(@"(^|[\W_])(?:(?!\1)|(?=^))(\*|_)\2(?=\S)(.*?\S)\2\2(?!\2)(?=[\W_]|$)",
                RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex Italic = new Regex(@"(\*|_) (?=\S) (.+?) (?<=\S) \1",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex StrictItalic =
            new Regex(@"(^|[\W_])(?:(?!\1)|(?=^))(\*|_)(?=\S)((?:(?!\2).)*?\S)\2(?!\2)(?=[\W_]|$)",
                RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex Blockquote = new Regex(@"
            (                           # Wrap whole match in $1
                (
                ^[ ]*>[ ]?              # '>' at the start of a line
                    .+\n                # rest of the first line
                (.+\n)*                 # subsequent consecutive lines
                \n*                     # blanks
                )+
            )", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex AutolinkBare =
            new Regex(@"(<|="")?\b(https?|ftp)(://" + CharInsideUrl + "*" + CharEndingUrl + ")(?=$|\\W)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex EndCharRegex =
            new Regex(CharEndingUrl, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex OutDent = new Regex(@"^[ ]{1," + TabWidth + @"}",
            RegexOptions.Multiline | RegexOptions.Compiled);

        private readonly Dictionary<string, string> _htmlBlocks = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _titles = new Dictionary<string, string>();

        private readonly Dictionary<string, string> _urls = new Dictionary<string, string>();

        private int _listLevel;

        /// <summary>
        ///     In the static constuctor we'll initialize what stays the same across all transforms. 
        /// </summary>
        static MarkdownTransformer()
        {
            // Table of hash values for escaped characters:
            EscapeTable = new Dictionary<string, string>();

            InvertedEscapeTable = new Dictionary<string, string>();

            // Table of hash value for backslash escaped characters:
            BackslashEscapeTable = new Dictionary<string, string>();

            var backslashPattern = string.Empty;

            foreach (var c in @"\`*_{}[]()>#+-.!/")
            {
                var key = c.ToString();
                var hash = GetHashKey(key, false);
                EscapeTable.Add(key, hash);
                InvertedEscapeTable.Add(hash, key);
                BackslashEscapeTable.Add(@"\" + key, hash);
                backslashPattern += Regex.Escape(@"\" + key) + "|";
            }

            BackslashEscapes = new Regex(backslashPattern.Substring(0, backslashPattern.Length - 1),
                RegexOptions.Compiled);
        }

        /// <summary>
        ///     Create a new Markdown instance using default options 
        /// </summary>
        public MarkdownTransformer()
        {
        }

        /// <summary>
        ///     Create a new Markdown instance and set the options from the
        ///     MarkdownTransformerOptions object.
        /// </summary>
        public MarkdownTransformer(MarkdownTransformerOptions options)
        {
            Options = options;
        }

        public MarkdownTransformerOptions Options { get; set; } = new MarkdownTransformerOptions();

        /// <summary>
        ///     Transforms the provided Markdown-formatted text to HTML; see http://en.wikipedia.org/wiki/Markdown 
        /// </summary>
        /// <remarks>
        ///     The order in which other subs are called here is essential. Link and image
        ///     substitutions need to happen before EscapeSpecialChars(), so that any *'s or _'s in
        ///     the a and img tags get encoded.
        /// </remarks>
        public string Transform(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            Setup();

            text = Normalize(text);

            text = HashHtmlBlocks(text);
            text = StripLinkDefinitions(text);
            text = RunBlockGamut(text);
            text = UnEscape(text);

            Cleanup();

            return text + "\n";
        }

        /// <summary>
        ///     Perform transformations that form block-level tags like paragraphs, headers, and list items.
        /// </summary>
        private string RunBlockGamut(string text, bool unHash = true)
        {
            text = DoHeaders(text);
            text = DoHorizontalRules(text);
            text = DoLists(text);
            text = DoGithubCodeBlocks(text);
            text = DoCodeBlocks(text);
            text = DoBlockQuotes(text);

            // We already ran HashHTMLBlocks() before, in Markdown(), but that was to escape raw HTML
            // in the original Markdown source. This time, we're escaping the markup we've just
            // created, so that we don't wrap <p> tags around block-level tags.
            text = HashHtmlBlocks(text);

            text = FormParagraphs(text, unHash);

            return text;
        }

        /// <summary>
        ///     Perform transformations that occur *within* block-level tags like paragraphs,
        ///     headers, and list items.
        /// </summary>
        private string RunSpanGamut(string text)
        {
            text = DoCodeSpans(text);
            text = EscapeSpecialCharsWithinTagAttributes(text);
            text = EscapeBackslashes(text);

            // Images must come first, because ![foo][f] looks like an anchor.
            text = DoImages(text);
            text = DoAnchors(text);

            // Must come after DoAnchors(), because you can use < and > delimiters in inline links
            // like [this](<url>).
            text = DoAutoLinks(text);

            text = text.Replace(AutoLinkPreventionMarker, "://");

            text = EncodeAmpsAndAngles(text);
            text = DoItalicsAndBold(text);
            text = DoHardBreaks(text);

            return text;
        }

        /// <summary>
        ///     splits on two or more newlines, to form "paragraphs"; each paragraph is then unHashed
        ///     (if it is a hash and unHashing isn't turned off) or wrapped in HTML p tag
        /// </summary>
        private string FormParagraphs(string text, bool unHash = true)
        {
            // split on two or more newlines
            var grafs = NewlinesMultiple.Split(NewlinesLeadingTrailing.Replace(text, string.Empty));

            var skipGraphs = false;
            for (var i = 0; i < grafs.Length; i++)
            {
                if (grafs[i].Contains("<code ")
                    && !grafs[i].Contains("</code>"))
                    skipGraphs = true;
                if (grafs[i].Contains("</code>")
                    && !grafs[i].Contains("<code "))
                {
                    skipGraphs = false;
                    continue;
                }

                if (skipGraphs) continue;

                if (grafs[i].StartsWith("\x1AH"))
                {
                    // unHashify HTML blocks
                    if (unHash)
                    {
                        var sanityCheck = 50; // just for safety, guard against an infinite loop
                        var keepGoing = true; // as long as replacements where made, keep going
                        while (keepGoing && sanityCheck > 0)
                        {
                            keepGoing = false;
                            grafs[i] = HtmlBlockHash.Replace(grafs[i], match =>
                            {
                                keepGoing = true;
                                return _htmlBlocks[match.Value];
                            });
                            sanityCheck--;
                        }
                        /* if (keepGoing)
                        {
                            // Logging of an infinite loop goes here. If such a thing should happen,
                            // please open a new issue on http://code.google.com/p/markdownsharp/
                            // with the input that caused it.
                        }*/
                    }
                }
                else
                {
                    // do span level processing inside the block, then wrap result in <p> tags
                    grafs[i] = LeadingWhitespace.Replace(RunSpanGamut(grafs[i]), "<p>") + "</p>";
                }
            }

            return string.Join("\n\n", grafs);
        }

        private void Setup()
        {
            // Clear the global hashes. If we don't clear these, you get conflicts from other
            // articles when generating a page which contains more than one article (e.g. an index
            // page that shows the N most recent articles):
            _urls.Clear();
            _titles.Clear();
            _htmlBlocks.Clear();
            _listLevel = 0;
        }

        private void Cleanup()
        {
            Setup();
        }

        /// <summary>
        ///     Reusable pattern to match balanced [brackets]. See Friedl's "Mastering Regular
        ///     Expressions", 2nd Ed., pp. 328-331.
        /// </summary>
        private static string GetNestedBracketsPattern()
        {
            // in other words [this] and [this[also]] and [this[also[too]]] up to _nestDepth
            if (_nestedBracketsPattern == null)
                _nestedBracketsPattern =
                    RepeatString(@"
                    (?>              # Atomic matching
                       [^\[\]]+      # Anything other than brackets
                     |
                       \[
                           ", NestDepth) + RepeatString(
                        @" \]
                    )*"
                        , NestDepth);
            return _nestedBracketsPattern;
        }

        /// <summary>
        ///     Reusable pattern to match balanced (parens). See Friedl's "Mastering Regular
        ///     Expressions", 2nd Ed., pp. 328-331.
        /// </summary>
        private static string GetNestedParensPattern()
        {
            // in other words (this) and (this(also)) and (this(also(too))) up to _nestDepth
            if (_nestedParensPattern == null)
                _nestedParensPattern =
                    RepeatString(@"
                    (?>              # Atomic matching
                       [^()\s]+      # Anything other than parens or whitespace
                     |
                       \(
                           ", NestDepth) + RepeatString(
                        @" \)
                    )*"
                        , NestDepth);
            return _nestedParensPattern;
        }

        /// <summary>
        ///     Strips link definitions from text, stores the URLs and titles in hash references. 
        /// </summary>
        /// <remarks> ^[id]: url "optional title" </remarks>
        private string StripLinkDefinitions(string text)
        {
            return LinkDef.Replace(text, LinkEvaluator);
        }

        private string LinkEvaluator(Match match)
        {
            var linkId = match.Groups[1].Value.ToLowerInvariant();
            _urls[linkId] = EncodeAmpsAndAngles(match.Groups[2].Value);

            if (match.Groups[3] != null && match.Groups[3].Length > 0)
                _titles[linkId] = match.Groups[3].Value.Replace("\"", "&quot;");

            return string.Empty;
        }

        /// <summary>
        ///     derived pretty much verbatim from PHP Markdown 
        /// </summary>
        private static string GetBlockPattern()
        {
            // Hashify HTML blocks: We only want to do this for block-level HTML tags, such as
            // headers, lists, and tables. That's because we still want to wrap <p>s around
            // "paragraphs" that are wrapped in non-block-level tags, such as anchors, phrase
            // emphasis, and spans. The list of tags we're looking for is hard-coded:
            //
            // *  List "a" is made of tags which can be both inline or block-level. These will be
            //    treated block-level when the start tag is alone on its line, otherwise they're not
            //    matched here and will be taken as inline later.
            // *  List "b" is made of tags which are always block-level;
            var blockTagsA = "ins|del";
            var blockTagsB =
                "p|div|h[1-6]|blockquote|pre|table|dl|ol|ul|address|script|noscript|form|fieldset|iframe|math";

            // Regular expression for the content of a block tag.
            var attr = @"
            (?>﻿  ﻿  ﻿  ﻿              # optional tag attributes
              \s﻿  ﻿  ﻿              # starts with whitespace
              (?>
                [^>""/]+﻿              # text outside quotes
              |
                /+(?!>)﻿  ﻿              # slash not followed by >
              |
                string.Empty[^""]*""﻿  ﻿          # text inside double quotes (tolerate >)
              |
                '[^']*'﻿                  # text inside single quotes (tolerate >)
              )*
            )?﻿
            ";

            var content = RepeatString(@"
                (?>
                  [^<]+﻿  ﻿  ﻿          # content without tag
                |
                  <\2﻿  ﻿  ﻿          # nested opening tag
                    " + attr + @"       # attributes
                  (?>
                      />
                  |
                      >", NestDepth) + // end of opening tag
                          ".*?" + // last level nested tag content
                          RepeatString(@"
                      </\2\s*>﻿          # closing nested tag
                  )
                  |﻿  ﻿  ﻿  ﻿
                  <(?!/\2\s*>           # other tags with a different name
                  )
                )*", NestDepth);

            var content2 = content.Replace(@"\2", @"\3");

            // First, look for nested blocks, e.g.: ﻿ 
            // <div> ﻿ ﻿ <div> ﻿ ﻿ tags for inner block must be indented. ﻿ ﻿ </div> ﻿ </div>
            // The outermost tags must start at the left margin for this to match, and the inner
            // nested divs must be indented. We need to do this before the next, more liberal match,
            // because the next match will start at the first `
            // <div> ` and stop at the first ` </div>
            // `.
            var pattern = @"
            (?>
                  (?>
                    (?<=\n)     # Starting at the beginning of a line
                    |           # or
                    \A\n?       # the beginning of the doc
                  )
                  (             # save in $1

                    # Match from `\n<tag>` to `</tag>\n`, handling nested tags
                    # in between.

                        <($block_tags_b_re)   # start tag = $2
                        $attr>                # attributes followed by > and \n
                        $content              # content, support nesting
                        </\2>                 # the matching end tag
                        [ ]*                  # trailing spaces
                        (?=\n+|\Z)            # followed by a newline or end of document

                  | # Special version for tags of group a.

                        <($block_tags_a_re)   # start tag = $3
                        $attr>[ ]*\n          # attributes followed by >
                        $content2             # content, support nesting
                        </\3>                 # the matching end tag
                        [ ]*                  # trailing spaces
                        (?=\n+|\Z)            # followed by a newline or end of document

                  | # Special case just for <hr />. It was easier to make a special
                    # case than to make the other regex more complicated.

                        [ ]{0,$less_than_tab}
                        <hr
                        $attr                 # attributes
                        /?>                   # the matching end tag
                        [ ]*
                        (?=\n{2,}|\Z)         # followed by a blank line or end of document

                  | # Special case for standalone HTML comments:

                      (?<=\n\n|\A)            # preceded by a blank line or start of document
                      [ ]{0,$less_than_tab}
                      (?s:
                        <!--(?:|(?:[^>-]|-[^>])(?:[^-]|-[^-])*)-->
                      )
                      [ ]*
                      (?=\n{2,}|\Z)            # followed by a blank line or end of document

                  | # PHP and ASP-style processor instructions (<? and <%)

                      [ ]{0,$less_than_tab}
                      (?s:
                        <([?%])                # $4
                        .*?
                        \4>
                      )
                      [ ]*
                      (?=\n{2,}|\Z)            # followed by a blank line or end of document

                  )
            )";

            pattern = pattern.Replace("$less_than_tab", (TabWidth - 1).ToString());
            pattern = pattern.Replace("$block_tags_b_re", blockTagsB);
            pattern = pattern.Replace("$block_tags_a_re", blockTagsA);
            pattern = pattern.Replace("$attr", attr);
            pattern = pattern.Replace("$content2", content2);
            pattern = pattern.Replace("$content", content);

            return pattern;
        }

        /// <summary>
        ///     replaces any block-level HTML blocks with hash entries 
        /// </summary>
        private string HashHtmlBlocks(string text)
        {
            return BlocksHtml.Replace(text, HtmlEvaluator);
        }

        private string HtmlEvaluator(Match match)
        {
            var text = match.Groups[1].Value;
            var key = GetHashKey(text, true);
            _htmlBlocks[key] = text;

            return string.Concat("\n\n", key, "\n\n");
        }

        private static string GetHashKey(string s, bool isHtmlBlock)
        {
            var delim = isHtmlBlock ? 'H' : 'E';
            return "\x1A" + delim + Math.Abs(s.GetHashCode()) + delim;
        }

        /// <summary>
        ///     returns an array of HTML tokens comprising the input string. Each token is either a
        ///     tag (possibly with nested, tags contained therein, such as &lt;a
        ///     href="&lt;MTFoo&gt;"&gt;, or a run of text between tags. Each element of the array is
        ///     a two-element array; the first is either 'tag' or 'text'; the second is the actual value.
        /// </summary>
        private List<Token> TokenizeHTML(string text)
        {
            var pos = 0;
            var tokens = new List<Token>();

            // this regex is derived from the _tokenize() subroutine in Brad Choate's MTRegex plugin. http://www.bradchoate.com/past/mtregex.php
            foreach (Match m in HtmlTokens.Matches(text))
            {
                var tagStart = m.Index;

                if (pos < tagStart)
                    tokens.Add(new Token(TokenType.Text, text.Substring(pos, tagStart - pos)));

                tokens.Add(new Token(TokenType.Tag, m.Value));
                pos = tagStart + m.Length;
            }

            if (pos < text.Length)
                tokens.Add(new Token(TokenType.Text, text.Substring(pos, text.Length - pos)));

            return tokens;
        }

        /// <summary>
        ///     Turn Markdown link shortcuts into HTML anchor tags 
        /// </summary>
        /// <remarks> [link text](url "title") [link text][id] [id] </remarks>
        private string DoAnchors(string text)
        {
            // First, handle reference-style links: [link text] [id]
            text = AnchorRef.Replace(text, AnchorRefEvaluator);

            // Next, inline-style links: [link text](url "optional title") or [link text](url
            // "optional title")
            text = AnchorInline.Replace(text, AnchorInlineEvaluator);

            // Last, handle reference-style shortcuts: [link text] These must come last in case
            // you've also got [link test][1] or [link test](/foo)
            text = AnchorRefShortcut.Replace(text, AnchorRefShortcutEvaluator);
            return text;
        }

        private string SaveFromAutoLinking(string s)
        {
            return s.Replace("://", AutoLinkPreventionMarker);
        }

        private string AnchorRefEvaluator(Match match)
        {
            var wholeMatch = match.Groups[1].Value;
            var linkText = SaveFromAutoLinking(match.Groups[2].Value);
            var linkId = match.Groups[3].Value.ToLowerInvariant();

            string result;

            // for shortcut links like [this][].
            if (linkId == string.Empty)
                linkId = linkText.ToLowerInvariant();

            if (_urls.ContainsKey(linkId))
            {
                var url = _urls[linkId];

                url = EncodeProblemUrlChars(url);
                url = EscapeBoldItalic(url);
                result = "<a href=\"" + url + "\"";

                if (_titles.ContainsKey(linkId))
                {
                    var title = AttributeEncode(_titles[linkId]);
                    title = AttributeEncode(EscapeBoldItalic(title));
                    result += " title=\"" + title + "\"";
                }

                result += ">" + linkText + "</a>";
            }
            else
            {
                result = wholeMatch;
            }

            return result;
        }

        private string AnchorRefShortcutEvaluator(Match match)
        {
            var wholeMatch = match.Groups[1].Value;
            var linkText = SaveFromAutoLinking(match.Groups[2].Value);
            var linkId =
                Regex.Replace(linkText.ToLowerInvariant(), @"[ ]*\n[ ]*",
                    " "); // lower case and remove newlines / extra spaces

            string result;

            if (_urls.ContainsKey(linkId))
            {
                var url = _urls[linkId];

                url = EncodeProblemUrlChars(url);
                url = EscapeBoldItalic(url);
                result = "<a href=\"" + url + "\"";

                if (_titles.ContainsKey(linkId))
                {
                    var title = AttributeEncode(_titles[linkId]);
                    title = EscapeBoldItalic(title);
                    result += " title=\"" + title + "\"";
                }

                result += ">" + linkText + "</a>";
            }
            else
            {
                result = wholeMatch;
            }

            return result;
        }

        private string AnchorInlineEvaluator(Match match)
        {
            var linkText = SaveFromAutoLinking(match.Groups[2].Value);
            var url = match.Groups[3].Value;
            var title = match.Groups[6].Value;

            url = EncodeProblemUrlChars(url);
            url = EscapeBoldItalic(url);
            if (url.StartsWith("<") && url.EndsWith(">"))
                url = url.Substring(1, url.Length - 2); // remove <>'s surrounding URL, if present

            var result = string.Format("<a href=\"{0}\"", url);

            if (!string.IsNullOrWhiteSpace(title))
            {
                title = AttributeEncode(title);
                title = EscapeBoldItalic(title);
                result += string.Format(" title=\"{0}\"", title);
            }

            result += string.Format(">{0}</a>", linkText);
            return result;
        }

        /// <summary>
        ///     Turn Markdown image shortcuts into HTML img tags. 
        /// </summary>
        /// <remarks> ![alt text][id] ![alt text](url "optional title") </remarks>
        private string DoImages(string text)
        {
            // First, handle reference-style labeled images: ![alt text][id]
            text = ImagesRef.Replace(text, ImageReferenceEvaluator);

            // Next, handle inline images: ![alt text](url "optional title") Don't forget: encode *
            // and _
            text = ImagesInline.Replace(text, ImageInlineEvaluator);

            return text;
        }

        // This prevents the creation of horribly broken HTML when some syntax ambiguities collide.
        // It likely still doesn't do what the user meant, but at least we're not outputting garbage.
        private string EscapeImageAltText(string s)
        {
            s = EscapeBoldItalic(s);
            s = Regex.Replace(s, @"[\[\]()]", m => EscapeTable[m.ToString()]);
            return s;
        }

        private string ImageReferenceEvaluator(Match match)
        {
            var wholeMatch = match.Groups[1].Value;
            var altText = match.Groups[2].Value;
            var linkId = match.Groups[3].Value.ToLowerInvariant();

            // for shortcut links like ![this][].
            if (linkId == string.Empty)
                linkId = altText.ToLowerInvariant();

            if (_urls.ContainsKey(linkId))
            {
                var url = _urls[linkId];
                string title = null;

                if (_titles.ContainsKey(linkId))
                    title = _titles[linkId];

                return ImageTag(url, altText, title);
            }
            // If there's no such link ID, leave intact:
            return wholeMatch;
        }

        private string ImageInlineEvaluator(Match match)
        {
            var alt = match.Groups[2].Value;
            var url = match.Groups[3].Value;
            var title = match.Groups[6].Value;

            if (url.StartsWith("<") && url.EndsWith(">"))
                url = url.Substring(1, url.Length - 2); // Remove <>'s surrounding URL, if present

            return ImageTag(url, alt, title);
        }

        private string ImageTag(string url, string altText, string title)
        {
            altText = EscapeImageAltText(AttributeEncode(altText));
            url = EncodeProblemUrlChars(url);
            url = EscapeBoldItalic(url);
            var result = string.Format("<img src=\"{0}\" alt=\"{1}\"", url, altText);
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = AttributeEncode(EscapeBoldItalic(title));
                result += string.Format(" title=\"{0}\"", title);
            }
            result += Options.EmptyElementSuffix;
            return result;
        }

        /// <summary>
        ///     Turn Markdown headers into HTML header tags 
        /// </summary>
        /// <remarks>
        ///     ProcessingTimeResponseHeader 1 ======== ProcessingTimeResponseHeader 2 -------- #
        ///     ProcessingTimeResponseHeader 1 ## ProcessingTimeResponseHeader 2 ##
        ///     ProcessingTimeResponseHeader 2 with closing hashes ## ... ######
        ///     ProcessingTimeResponseHeader 6
        /// </remarks>
        private string DoHeaders(string text)
        {
            text = HeaderText.Replace(text, TextHeaderEvaluator);
            text = HeaderAtx.Replace(text, AtxHeaderEvaluator);
            return text;
        }

        private string TextHeaderEvaluator(Match match)
        {
            var header = match.Groups[1].Value;
            var level = match.Groups[2].Value.StartsWith("=") ? 1 : 2;
            return string.Format("<h{1}>{0}</h{1}>\n\n", RunSpanGamut(header), level);
        }

        private string AtxHeaderEvaluator(Match match)
        {
            var header = match.Groups[2].Value;
            var level = match.Groups[1].Value.Length;
            return string.Format("<h{1}>{0}</h{1}>\n\n", RunSpanGamut(header), level);
        }

        /// <summary>
        ///     Turn Markdown horizontal rules into HTML hr tags 
        /// </summary>
        /// <remarks>
        ///     ***
        ///     * * * ---
        ///     - - -
        /// </remarks>
        private string DoHorizontalRules(string text)
        {
            return HorizontalRules.Replace(text, "<hr" + Options.EmptyElementSuffix + "\n");
        }

        /// <summary>
        ///     Turn Markdown lists into HTML ul and ol and li tags 
        /// </summary>
        private string DoLists(string text, bool isInsideParagraphLessListItem = false)
        {
            // We use a different prefix before nested lists than top-level lists. See extended
            // comment in _ProcessListItems().
            if (_listLevel > 0)
                text = ListNested.Replace(text, GetListEvaluator(isInsideParagraphLessListItem));
            else
                text = ListTopLevel.Replace(text, GetListEvaluator());

            return text;
        }

        private MatchEvaluator GetListEvaluator(bool isInsideParagraphLessListItem = false)
        {
            return match =>
            {
                var list = match.Groups[1].Value;
                var listType = Regex.IsMatch(match.Groups[3].Value, MarkerUl) ? "ul" : "ol";
                string result;

                result = ProcessListItems(list, listType == "ul" ? MarkerUl : MarkerOl,
                    isInsideParagraphLessListItem);

                result = string.Format("<{0}>\n{1}</{0}>\n", listType, result);
                return result;
            };
        }

        /// <summary>
        ///     Process the contents of a single ordered or unordered list, splitting it into
        ///     individual list items.
        /// </summary>
        private string ProcessListItems(string list, string marker, bool isInsideParagraphLessListItem = false)
        {
            // The listLevel global keeps track of when we're inside a list. Each time we enter a
            // list, we increment it; when we leave a list, we decrement. If it's zero, we're not in
            // a list anymore.

            // We do this because when we're not inside a list, we want to treat something like this:

            // I recommend upgrading to version
            // 8. Oops, now this line is treated as a sub-list.

            // As a single paragraph, despite the fact that the second line starts with a
            // digit-period-space sequence.

            // Whereas when we're inside a list (or sub-list), that line will be treated as the start
            // of a sub-list. What a kludge, huh? This is an aspect of Markdown's syntax that's hard
            // to parse perfectly without resorting to mind-reading. Perhaps the solution is to
            // change the syntax rules such that sub-lists must start with a starting cardinal
            // number; e.g. "1." or "a.".

            _listLevel++;

            // Trim trailing blank lines:
            list = Regex.Replace(list, @"\n{2,}\z", "\n");

            var pattern = string.Format(
                @"(^[ ]*)                    # leading whitespace = $1
                ({0}) [ ]+                 # list marker = $2
                ((?s:.+?)                  # list item text = $3
                (\n+))
                (?= (\z | \1 ({0}) [ ]+))", marker);

            var lastItemHadADoubleNewline = false;

            // has to be a closure, so subsequent invocations can share the bool
            string ListItemEvaluator(Match match)
            {
                var item = match.Groups[3].Value;

                var endsWithDoubleNewline = item.EndsWith("\n\n");
                var containsDoubleNewline = endsWithDoubleNewline || item.Contains("\n\n");

                if (containsDoubleNewline || lastItemHadADoubleNewline)
                // we could correct any bad indentation here..
                {
                    item = RunBlockGamut(Outdent(item) + "\n", false);
                }
                else
                {
                    // recursion for sub-lists
                    item = DoLists(Outdent(item), true);
                    item = item.TrimEnd('\n');
                    if (!isInsideParagraphLessListItem) // only the outer-most item should run this, otherwise it's run multiple times for the inner ones
                        item = RunSpanGamut(item);
                }
                lastItemHadADoubleNewline = endsWithDoubleNewline;
                return string.Format("<li>{0}</li>\n", item);
            }

            list = Regex.Replace(list, pattern, ListItemEvaluator,
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            _listLevel--;
            return list;
        }

        /// <summary>
        ///     /// Turn Markdown 4-space indented code into HTML pre code blocks 
        /// </summary>
        private string DoCodeBlocks(string text)
        {
            text = CodeBlock.Replace(text, CodeBlockEvaluator);
            return text;
        }

        private string CodeBlockEvaluator(Match match)
        {
            var codeBlock = match.Groups[1].Value;

            codeBlock = EncodeCode(Outdent(codeBlock));
            codeBlock = NewlinesLeadingTrailing.Replace(codeBlock, string.Empty);

            return string.Concat("\n\n<pre><code>\n", codeBlock, "\n</code></pre>\n\n");
        }

        private string DoGithubCodeBlocks(string text)
        {
            return GithubCodeBlock.Replace(text, GithubCodeEvaluator);
        }

        private string GithubCodeEvaluator(Match match)
        {
            var codeBlock = match.Groups[3].Value;
            var typeBlock = match.Groups[2].Value;

            //removed Out-dent on the code-block
            codeBlock = EncodeCode(codeBlock);
            codeBlock = NewlinesLeadingTrailing.Replace(codeBlock, string.Empty);

            return string.Concat("\n\n<pre><code class=\"language-", typeBlock, "\">", codeBlock,
                "\n</code></pre>\n\n");
        }

        /// <summary>
        ///     Turn Markdown `code spans` into HTML code tags 
        /// </summary>
        private string DoCodeSpans(string text)
        {
            // * You can use multiple back-ticks as the delimiters if you want to include literal
            //   back-ticks in the code span. So, this input:
            //
            // Just type ``foo `bar` baz`` at the prompt.
            //
            // Will translate to:
            // <p>
            //     Just type 
            //     <code>
            // foo `bar` baz
            //     </code>
            //     at the prompt. 
            // </p>
            // There's no arbitrary limit to the number of back-ticks you can use as delimiters. If
            // you need three consecutive back-ticks in your code, use four for delimiters, etc.
            //
            // * You can use spaces to get literal back-ticks at the edges:
            //
            // ... type `` `bar` `` ...
            //
            // Turns to:
            //
            // ... type
            // <code>
            // `bar`
            // </code>
            // ...

            return CodeSpan.Replace(text, CodeSpanEvaluator);
        }

        private string CodeSpanEvaluator(Match match)
        {
            var span = match.Groups[2].Value;
            span = Regex.Replace(span, @"^[ ]*", string.Empty); // leading whitespace
            span = Regex.Replace(span, @"[ ]*$", string.Empty); // trailing whitespace
            span = EncodeCode(span);
            span = SaveFromAutoLinking(
                span); // to prevent auto-linking. Not necessary in code *blocks*, but in code spans.

            return string.Concat("<code>", span, "</code>");
        }

        /// <summary>
        ///     Turn Markdown *italics* and **bold** into HTML strong and em tags 
        /// </summary>
        private string DoItalicsAndBold(string text)
        {
            // <strong> must go first, then <em>
            if (Options.StrictBoldItalic)
            {
                text = StrictBold.Replace(text, "$1<strong>$3</strong>");
                text = StrictItalic.Replace(text, "$1<em>$3</em>");
            }
            else
            {
                text = Bold.Replace(text, "<strong>$2</strong>");
                text = Italic.Replace(text, "<em>$2</em>");
            }
            return text;
        }

        /// <summary>
        ///     Turn markdown line breaks (two space at end of line) into HTML break tags 
        /// </summary>
        private string DoHardBreaks(string text)
        {
            text = Regex.Replace(text, Options.AutoNewlines ? @"\n" : @" {2,}\n", string.Format("<br{0}\n", Options.EmptyElementSuffix));
            return text;
        }

        /// <summary>
        ///     Turn Markdown &gt; quoted blocks into HTML block-quote blocks 
        /// </summary>
        private string DoBlockQuotes(string text)
        {
            return Blockquote.Replace(text, BlockQuoteEvaluator);
        }

        private string BlockQuoteEvaluator(Match match)
        {
            var bq = match.Groups[1].Value;

            bq = Regex.Replace(bq, @"^[ ]*>[ ]?", string.Empty, RegexOptions.Multiline); // trim one level of quoting
            bq = Regex.Replace(bq, @"^[ ]+$", string.Empty, RegexOptions.Multiline); // trim whitespace-only lines
            bq = RunBlockGamut(bq); // re-curse

            bq = Regex.Replace(bq, @"^", "  ", RegexOptions.Multiline);

            // These leading spaces screw with <pre> content, so we need to fix that:
            bq = Regex.Replace(bq, @"(\s*<pre>.+?</pre>)", BlockQuoteEvaluator2,
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            bq = string.Format("<blockquote>\n{0}\n</blockquote>", bq);
            var key = GetHashKey(bq, true);
            _htmlBlocks[key] = bq;

            return "\n\n" + key + "\n\n";
        }

        private string BlockQuoteEvaluator2(Match match)
        {
            return Regex.Replace(match.Groups[1].Value, @"^  ", string.Empty, RegexOptions.Multiline);
        }

        private static string HandleTrailingParens(Match match)
        {
            // The first group is essentially a negative lookbehind -- if there's a < or a =", we
            // don't touch this. We're not using a *real* lookbehind, because of links with in links,
            // like <a href="http://web.archive.org/web/20121130000728/http://www.google.com/"> With
            // a real lookbehind, the full link would never be matched, and thus the
            // http://www.google.com *would* be matched. With the simulated lookbehind, the full link
            // *is* matched (just not handled, because of this early return), causing the google link
            // to not be matched again.
            if (match.Groups[1].Success)
                return match.Value;

            var protocol = match.Groups[2].Value;
            var link = match.Groups[3].Value;
            if (!link.EndsWith(")"))
                return "<" + protocol + link + ">";
            var level = 0;
            foreach (Match c in Regex.Matches(link, "[()]"))
                if (c.Value == "(")
                    if (level <= 0)
                        level = 1;
                    else
                        level++;
                else
                    level--;
            var tail = string.Empty;
            if (level < 0)
                link = Regex.Replace(link, @"\){1," + -level + "}$", m =>
                {
                    tail = m.Value;
                    return string.Empty;
                });
            if (tail.Length > 0)
            {
                var lastChar = link[link.Length - 1];
                if (!EndCharRegex.IsMatch(lastChar.ToString()))
                {
                    tail = lastChar + tail;
                    link = link.Substring(0, link.Length - 1);
                }
            }
            return "<" + protocol + link + ">" + tail;
        }

        /// <summary>
        ///     Turn angle-delimited URLs into HTML anchor tags 
        /// </summary>
        /// <remarks> &lt;http://www.example.com&gt; </remarks>
        private string DoAutoLinks(string text)
        {
            if (Options.AutoHyperlink)
                text = AutolinkBare.Replace(text, HandleTrailingParens);

            // Hyperlinks: <http://topnguyen.net>
            text = Regex.Replace(text, "<((https?|ftp):[^'\">\\s]+)>", HyperlinkEvaluator);

            if (Options.LinkEmails)
            {
                // Email addresses: <address@domain.foo>
                var pattern =
                    @"<
                      (?:mailto:)?
                      (
                        [-.\w]+
                        \@
                        [-a-z0-9]+(\.[-a-z0-9]+)*\.[a-z]+
                      )
                      >";
                text = Regex.Replace(text, pattern, EmailEvaluator,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            }

            return text;
        }

        private string HyperlinkEvaluator(Match match)
        {
            var link = match.Groups[1].Value;
            return string.Format("<a href=\"{0}\">{1}</a>", EncodeProblemUrlChars(link), link);
        }

        private string EmailEvaluator(Match match)
        {
            var email = UnEscape(match.Groups[1].Value);

            // Input: an email address, e.g. "foo@example.com"
            //
            // Output: the email address as a mailto link, with each character of the address encoded
            //         as either a decimal or hex entity, in the hopes of foiling most address
            //         harvesting spam bots. E.g.:
            email = "mailto:" + email;

            // leave ':' alone (to spot mailto: later)
            email = EncodeEmailAddress(email);

            email = string.Format("<a href=\"{0}\">{0}</a>", email);

            // strip the mailto: from the visible part
            email = Regex.Replace(email, "\">.+?:", "\">");
            return email;
        }

        /// <summary>
        ///     Remove one level of line-leading spaces 
        /// </summary>
        private string Outdent(string block)
        {
            return OutDent.Replace(block, string.Empty);
        }

        /// <summary>
        ///     this is to emulate what's available in PHP 
        /// </summary>
        private static string RepeatString(string text, int count)
        {
            var sb = new StringBuilder(text.Length * count);
            for (var i = 0; i < count; i++)
                sb.Append(text);
            return sb.ToString();
        }

        private enum TokenType
        {
            Text,
            Tag
        }

        private struct Token
        {
            public Token(TokenType type, string value)
            {
                Type = type;
                Value = value;
            }

            public readonly TokenType Type;
            public readonly string Value;
        }

        #region Encoding and Normalization

        /// <summary>
        ///     encodes email address randomly roughly 10% raw, 45% hex, 45% dec note that @ is
        ///     always encoded and : never is
        /// </summary>
        private string EncodeEmailAddress(string addr)
        {
            var sb = new StringBuilder(addr.Length * 5);
            var rand = new Random();
            foreach (var c in addr)
            {
                var r = rand.Next(1, 100);
                if ((r > 90 || c == ':') && c != '@')
                    sb.Append(c); // m
                else if (r < 45)
                    sb.AppendFormat("&#x{0:x};", (int)c); // &#x6D
                else
                    sb.AppendFormat("&#{0};", (int)c); // &#109
            }
            return sb.ToString();
        }

        private static readonly Regex CodeEncoder = new Regex(@"&|<|>|\\|\*|_|\{|\}|\[|\]", RegexOptions.Compiled);

        /// <summary>
        ///     Encode/escape certain Markdown characters inside code blocks and spans where they are literals
        /// </summary>
        private string EncodeCode(string code)
        {
            return CodeEncoder.Replace(code, EncodeCodeEvaluator);
        }

        private string EncodeCodeEvaluator(Match match)
        {
            switch (match.Value)
            {
                // Encode all ampersands; HTML entities are not entities within a Markdown code span.
                case "&":
                    return "&amp;";
                // Do the angle bracket song and dance
                case "<":
                    return "&lt;";
                case ">":
                    return "&gt;";
                // escape characters that are magic in Markdown
                default:
                    return EscapeTable[match.Value];
            }
        }

        private static readonly Regex Amps = new Regex(@"&(?!((#[0-9]+)|(#[xX][a-fA-F0-9]+)|([a-zA-Z][a-zA-Z0-9]*));)",
            RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        private static readonly Regex Angles = new Regex(@"<(?![A-Za-z/?\$!])",
            RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        /// <summary>
        ///     Encode any ampersands (that aren't part of an HTML entity) and left or right angle brackets
        /// </summary>
        private string EncodeAmpsAndAngles(string s)
        {
            s = Amps.Replace(s, "&amp;");
            s = Angles.Replace(s, "&lt;");
            return s;
        }

        private static readonly Regex BackslashEscapes;

        /// <summary>
        ///     Encodes any escaped characters such as \`, \*, \[ etc 
        /// </summary>
        private string EscapeBackslashes(string s)
        {
            return BackslashEscapes.Replace(s, EscapeBackslashesEvaluator);
        }

        private string EscapeBackslashesEvaluator(Match match)
        {
            return BackslashEscapeTable[match.Value];
        }

        private static readonly Regex UnEscapes = new Regex("\x1A" + "E\\d+E", RegexOptions.Compiled);

        /// <summary>
        ///     swap back in all the special characters we've hidden 
        /// </summary>
        private string UnEscape(string s)
        {
            return UnEscapes.Replace(s, UnEscapeEvaluator);
        }

        private string UnEscapeEvaluator(Match match)
        {
            return InvertedEscapeTable[match.Value];
        }

        /// <summary>
        ///     escapes Bold [ * ] and Italic [ _ ] characters 
        /// </summary>
        private string EscapeBoldItalic(string s)
        {
            s = s.Replace("*", EscapeTable["*"]);
            s = s.Replace("_", EscapeTable["_"]);
            return s;
        }

        private static string AttributeEncode(string s)
        {
            return s.Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;");
        }

        private static readonly char[] ProblemUrlChars = @"""'*()[]$:_".ToCharArray();

        /// <summary>
        ///     hex-encodes some unusual "problem" chars in URLs to avoid URL detection problems 
        /// </summary>
        private string EncodeProblemUrlChars(string url)
        {
            if (!Options.EncodeProblemUrlCharacters) return url;

            var sb = new StringBuilder(url.Length);

            for (var i = 0; i < url.Length; i++)
            {
                var c = url[i];
                var encode = Array.IndexOf(ProblemUrlChars, c) != -1;
                if (encode && c == ':' && i < url.Length - 1)
                    encode = url[i + 1] != '/' && !(url[i + 1] >= '0' && url[i + 1] <= '9');

                if (encode)
                    sb.Append("%" + string.Format("{0:x}", (byte)c));
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Within tags -- meaning between &lt; and &gt; -- encode [\ ` * _] so they don't
        ///     conflict with their use in Markdown for code, italics and strong. We're replacing
        ///     each such character with its corresponding hash value; this is likely overkill, but
        ///     it should prevent us from colliding with the escape values by accident.
        /// </summary>
        private string EscapeSpecialCharsWithinTagAttributes(string text)
        {
            var tokens = TokenizeHTML(text);

            // now, rebuild text from the tokens
            var sb = new StringBuilder(text.Length);

            foreach (var token in tokens)
            {
                var value = token.Value;

                if (token.Type == TokenType.Tag)
                {
                    value = value.Replace(@"\", EscapeTable[@"\"]);

                    if (Options.AutoHyperlink && value.StartsWith("<!")
                    ) // escape slashes in comments to prevent auto-linking there -- http://meta.stackoverflow.com/questions/95987/html-comment-containing-url-breaks-if-followed-by-another-html-comment
                        value = value.Replace("/", EscapeTable["/"]);

                    value = Regex.Replace(value, "(?<=.)</?code>(?=.)", EscapeTable[@"`"]);
                    value = EscapeBoldItalic(value);
                }

                sb.Append(value);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     convert all tabs to _tabWidth spaces; standardizes line endings from DOS (CR LF) or
        ///     Mac (CR) to UNIX (LF); makes sure text ends with a couple of newlines; removes any
        ///     blank lines (only spaces) in the text
        /// </summary>
        private string Normalize(string text)
        {
            var output = new StringBuilder(text.Length);
            var line = new StringBuilder();
            var valid = false;

            for (var i = 0; i < text.Length; i++)
                switch (text[i])
                {
                    case '\n':
                        if (valid) output.Append(line);
                        output.Append('\n');
                        line.Length = 0;
                        valid = false;
                        break;

                    case '\r':
                        if (i < text.Length - 1 && text[i + 1] != '\n')
                        {
                            if (valid) output.Append(line);
                            output.Append('\n');
                            line.Length = 0;
                            valid = false;
                        }
                        break;

                    case '\t':
                        var width = TabWidth - line.Length % TabWidth;
                        for (var k = 0; k < width; k++)
                            line.Append(' ');
                        break;

                    case '\x1A':
                        break;

                    default:
                        if (!valid && text[i] != ' ') valid = true;
                        line.Append(text[i]);
                        break;
                }

            if (valid) output.Append(line);
            output.Append('\n');

            // add two newlines to the end before return
            return output.Append("\n\n").ToString();
        }

        #endregion
    }
}