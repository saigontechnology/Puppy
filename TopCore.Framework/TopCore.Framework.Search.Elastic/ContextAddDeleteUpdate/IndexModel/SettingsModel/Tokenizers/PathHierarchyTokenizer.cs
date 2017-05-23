using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
    public class PathHierarchyTokenizer : AnalysisTokenizerBase
    {
        private int _bufferSize;
        private bool _bufferSizeSet;
        private string _delimiter;
        private bool _delimiterSet;
        private string _replacement;
        private bool _replacementSet;
        private bool _reverse;
        private bool _reverseSet;
        private int _skip;
        private bool _skipSet;

        /// <summary>
        ///     The path_hierarchy tokenizer takes something like this: something/something/else And
        ///     produces tokens: something something/something something/something/else
        /// </summary>
        /// <param name="name"> custom name </param>
        public PathHierarchyTokenizer(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenizers.Pattern;
        }

        /// <summary>
        ///     delimiter The character delimiter to use, defaults to /. 
        /// </summary>
        public string Delimiter
        {
            get => _delimiter;
            set
            {
                _delimiter = value;
                _delimiterSet = true;
            }
        }

        /// <summary>
        ///     replacement An optional replacement character to use. Defaults to the delimiter. 
        /// </summary>
        public string Replacement
        {
            get => _replacement;
            set
            {
                _replacement = value;
                _replacementSet = true;
            }
        }

        /// <summary>
        ///     buffer_size The buffer size to use, defaults to 1024. 
        /// </summary>
        public int BufferSize
        {
            get => _bufferSize;
            set
            {
                _bufferSize = value;
                _bufferSizeSet = true;
            }
        }

        /// <summary>
        ///     reverse Generates tokens in reverse order, defaults to false. 
        /// </summary>
        public bool Reverse
        {
            get => _reverse;
            set
            {
                _reverse = value;
                _reverseSet = true;
            }
        }

        /// <summary>
        ///     skip Controls initial tokens to skip, defaults to 0. 
        /// </summary>
        public int skip
        {
            get => _skip;
            set
            {
                _skip = value;
                _skipSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("delimiter", _delimiter, elasticCrudJsonWriter, _delimiterSet);

            JsonHelper.WriteValue("replacement", _replacement, elasticCrudJsonWriter, _replacementSet);
            JsonHelper.WriteValue("buffer_size", _bufferSize, elasticCrudJsonWriter, _bufferSizeSet);
            JsonHelper.WriteValue("reverse", _reverse, elasticCrudJsonWriter, _reverseSet);
            JsonHelper.WriteValue("skip", _skip, elasticCrudJsonWriter, _skipSet);
        }
    }
}