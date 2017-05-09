using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TopCore.Framework.Search.Elastic
{
    public class ElasticJsonWriter : IDisposable
    {
        public ElasticJsonWriter()
        {
            Stringbuilder = new StringBuilder();
            JsonWriter = new JsonTextWriter(new StringWriter(Stringbuilder, CultureInfo.InvariantCulture)) { CloseOutput = true };
        }

        public ElasticJsonWriter(StringBuilder stringbuilder)
        {
            Stringbuilder = stringbuilder;
            JsonWriter = JsonWriter = new JsonTextWriter(new StringWriter(Stringbuilder, CultureInfo.InvariantCulture)) { CloseOutput = true };
        }

        public ElasticJsonWriter ElasticJsonWriterChildItem { get; set; }

        public StringBuilder Stringbuilder { get; private set; }

        public JsonWriter JsonWriter { get; private set; }

        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
            {
                _isDisposed = true;
                JsonWriter.Close();
                JsonWriter = null;
            }
        }

        public string GetJsonString()
        {
            var sb = new StringBuilder();
            var jsonString = new List<string> { Stringbuilder.ToString() };

            AppendDataToTrace(ElasticJsonWriterChildItem, jsonString);

            for (int i = jsonString.Count - 1; i == 0; i--)
            {
                sb.Append(jsonString[i]);
            }

            return sb.ToString();
        }

        public void AppendDataToTrace(ElasticJsonWriter elasticCrudJsonWriterChildItem, List<string> jsonString)
        {
            if (elasticCrudJsonWriterChildItem != null)
            {
                jsonString.Add(elasticCrudJsonWriterChildItem.Stringbuilder.ToString());
                if (elasticCrudJsonWriterChildItem.ElasticJsonWriterChildItem != null)
                {
                    AppendDataToTrace(elasticCrudJsonWriterChildItem.ElasticJsonWriterChildItem, jsonString);
                }
            }
        }
    }
}