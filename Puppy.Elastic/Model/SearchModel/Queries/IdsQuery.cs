using Puppy.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Elastic.Model.SearchModel.Queries
{
    public class IdsQuery : IQuery
    {
        private readonly List<object> _ids;
        private string _type;
        private bool _typeSet;

        /// <summary>
        ///     Filters documents that only have the provided ids. Note, this filter does not require
        ///     the _id field to be indexed since it works using the _uid field.
        /// </summary>
        /// <param name="ids"></param>
        public IdsQuery(List<object> ids)
        {
            _ids = ids;
        }

        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                _typeSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("ids");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("type", _type, elasticCrudJsonWriter, _typeSet);
            JsonHelper.WriteListValue("values", _ids, elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}