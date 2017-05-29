using System.Collections.Generic;

namespace Puppy.Search.Elastic.Utils
{
    public static class JsonHelper
    {
        public static void WriteValue(string key, object valueObj, ElasticJsonWriter elasticCrudJsonWriter,
            bool writeValue = true)
        {
            if (writeValue)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName(key);
                elasticCrudJsonWriter.JsonWriter.WriteValue(valueObj);
            }
        }

        public static void WriteListValue(string key, List<string> valueObj, ElasticJsonWriter elasticCrudJsonWriter,
            bool writeValue = true)
        {
            if (writeValue)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName(key);
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var obj in valueObj)
                    elasticCrudJsonWriter.JsonWriter.WriteValue(obj);

                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
        }

        public static void WriteListValue(string key, List<double> valueObj, ElasticJsonWriter elasticCrudJsonWriter,
            bool writeValue = true)
        {
            if (writeValue)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName(key);
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var obj in valueObj)
                    elasticCrudJsonWriter.JsonWriter.WriteValue(obj);

                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
        }

        public static void WriteListValue(string key, List<object> valueObj, ElasticJsonWriter elasticCrudJsonWriter,
            bool writeValue = true)
        {
            if (writeValue)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName(key);
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var obj in valueObj)
                    elasticCrudJsonWriter.JsonWriter.WriteValue(obj);

                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
        }
    }
}