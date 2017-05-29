using System;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
    public abstract class AnalyzerBase
    {
        private string _tokenizer;
        private bool _tokenizerSet;
        protected bool AnalyzerSet;
        protected string Name;
        protected string Type;

        public string Tokenizer
        {
            get => _tokenizer;
            set
            {
                _tokenizer = value;
                _tokenizerSet = true;
            }
        }

        public abstract void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);

        protected virtual void WriteJsonBase(ElasticJsonWriter elasticCrudJsonWriter,
            Action<ElasticJsonWriter> writeFilterSpecific)
        {
            if (AnalyzerSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName(Name);
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
                JsonHelper.WriteValue("tokenizer", _tokenizer, elasticCrudJsonWriter, _tokenizerSet);
                writeFilterSpecific.Invoke(elasticCrudJsonWriter);

                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
        }
    }
}