using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
    public abstract class BaseNGramTokenizer : AnalysisTokenizerBase
    {
        private int _maxGram;
        private bool _maxGramSet;
        private int _minGram;
        private bool _minGramSet;
        private List<TokenChar> _tokenChars;
        private bool _tokenCharsSet;

        /// <summary>
        ///     min_gram Minimum size in codepoints of a single n-gram 
        /// </summary>
        public int MinGram
        {
            get => _minGram;
            set
            {
                _minGram = value;
                _minGramSet = true;
            }
        }

        /// <summary>
        ///     max_gram Maximum size in codepoints of a single n-gram 
        /// </summary>
        public int MaxGram
        {
            get => _maxGram;
            set
            {
                _maxGram = value;
                _maxGramSet = true;
            }
        }

        /// <summary>
        ///     token_chars Characters classes to keep in the tokens, Elastic will split on characters that don’t belong to any of these classes. [] (Keep all characters) token_chars accepts the following character classes: letter for example a, b, ï or 京 digit for example 3 or 7 whitespace for
        ///     example " " or "\n" punctuation for example ! or " symbol for example $ or √
        /// </summary>
        public List<TokenChar> TokenChars
        {
            get => _tokenChars;
            set
            {
                _tokenChars = value;
                _tokenCharsSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("min_gram", _minGram, elasticCrudJsonWriter, _minGramSet);
            JsonHelper.WriteValue("max_gram", _maxGram, elasticCrudJsonWriter, _maxGramSet);
            WriteTokenCharValue("token_chars", _tokenChars, elasticCrudJsonWriter, _tokenCharsSet);
        }

        public static void WriteTokenCharValue(string key, List<TokenChar> valueObj,
            ElasticJsonWriter elasticCrudJsonWriter, bool writeValue = true)
        {
            if (writeValue)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName(key);
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var obj in valueObj)
                    elasticCrudJsonWriter.JsonWriter.WriteValue(obj.ToString());

                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
        }
    }
}