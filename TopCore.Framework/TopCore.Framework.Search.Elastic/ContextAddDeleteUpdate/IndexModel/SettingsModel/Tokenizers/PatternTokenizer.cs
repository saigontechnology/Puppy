using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
    public class PatternTokenizer : AnalysisTokenizerBase
    {
        private string _flags;
        private bool _flagsSet;
        private string _group;
        private bool _groupSet;
        private string _pattern;
        private bool _patternSet;

        /// <summary>
        ///   A tokenizer of type pattern that can flexibly separate text into terms via a regular expression. 
        /// </summary>
        /// <param name="name"> name of custom tokenizer ToLower() </param>
        public PatternTokenizer(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenizers.Pattern;
        }

        /// <summary>
        ///   pattern The regular expression pattern, defaults to \W+.
        ///   IMPORTANT: The regular expression should match the token separators, not the tokens themselves. Note that you may need to escape pattern
        ///              string literal according to your client language rules. For example, in many programming languages a string literal for \W+
        ///              pattern is written as "\\W+". There is nothing special about pattern (you may have to escape other string literals as well);
        ///              escaping pattern is common just because it often contains characters that should be escaped. group set to -1 (the default) is
        ///              equivalent to "split". Using group &gt;= 0 selects the matching group as the token. For example, if you have: pattern =
        ///              '([^']+)' group = 0 input = aaa 'bbb' 'ccc' the output will be two tokens: 'bbb' and 'ccc' (including the ' marks). With the
        ///              same input but using group=1, the output would be: bbb and ccc (no ' marks).
        /// </summary>
        public string Pattern
        {
            get => _pattern;
            set
            {
                _pattern = value;
                _patternSet = true;
            }
        }

        /// <summary>
        ///   flags The regular expression flags. 
        /// </summary>
        public string Flags
        {
            get => _flags;
            set
            {
                _flags = value;
                _flagsSet = true;
            }
        }

        /// <summary>
        ///   group Which group to extract into tokens. Defaults to -1 (split). 
        /// </summary>
        public string Group
        {
            get => _group;
            set
            {
                _group = value;
                _groupSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("pattern", _pattern, elasticCrudJsonWriter, _patternSet);

            JsonHelper.WriteValue("flags", _flags, elasticCrudJsonWriter, _flagsSet);
            JsonHelper.WriteValue("group", _group, elasticCrudJsonWriter, _groupSet);
        }
    }
}