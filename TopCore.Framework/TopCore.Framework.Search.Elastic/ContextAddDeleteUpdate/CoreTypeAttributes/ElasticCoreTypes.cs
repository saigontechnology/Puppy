using System;
using Newtonsoft.Json;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ElasticCoreTypes : Attribute
    {
        private string _copyTo;
        private string[] _copyToList;
        private bool _copyToListSet;
        private bool _copyToSet;
        private Type _fields;
        private bool _fieldsSet;
        private string _similarity;
        private bool _similaritySet;

        /// <summary>
        ///     "similarity":"BM25" 
        /// </summary>
        public virtual string Similarity
        {
            get => _similarity;
            set
            {
                _similarity = value;
                _similaritySet = true;
            }
        }

        public virtual string CopyTo
        {
            get => _copyTo;
            set
            {
                _copyTo = value;
                _copyToSet = true;
            }
        }

        public virtual string[] CopyToList
        {
            get => _copyToList;
            set
            {
                _copyToList = value;
                _copyToListSet = true;
            }
        }

        public virtual Type Fields
        {
            get => _fields;
            set
            {
                _fields = value;
                _fieldsSet = true;
            }
        }

        public virtual string JsonString()
        {
            return "";
        }

        protected void WriteBaseValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("similarity", _similarity, elasticCrudJsonWriter, _similaritySet);
            if (_copyToSet)
            {
                JsonHelper.WriteValue("copy_to", _copyTo, elasticCrudJsonWriter, _copyToSet);
            }
            else if (_copyToListSet)
            {
                var json = JsonConvert.SerializeObject(_copyToList);
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("copy_to");
                elasticCrudJsonWriter.JsonWriter.WriteRawValue(json);
            }
            if (_fieldsSet)
            {
                var fields = new Fields { FieldClass = _fields };
                fields.AddFieldData(elasticCrudJsonWriter);
            }
        }
    }
}