using System;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ElasticBoolean : ElasticCoreTypes
    {
        private double _boost;
        private bool _boostSet;
        private bool _docValues;
        private bool _docValuesSet;
        private NumberIndex _index;
        private string _indexName;

        private bool _indexNameSet;
        private bool _indexSet;
        private object _nullValue;
        private bool _nullValueSet;
        private bool _store;
        private bool _storeSet;

        /// <summary>
        ///     index_name The name of the field that will be stored in the index. Defaults to the
        ///     property/field name.
        /// </summary>
        public virtual string IndexName
        {
            get => _indexName;
            set
            {
                _indexName = value;
                _indexNameSet = true;
            }
        }

        /// <summary>
        ///     index Set to no if the value should not be indexed. Setting to no disables
        ///     include_in_all. If set to no the field should be either stored in _source, have
        ///     include_in_all enabled, or store be set to true for this to be useful.
        /// </summary>
        public virtual NumberIndex Index
        {
            get => _index;
            set
            {
                _index = value;
                _indexSet = true;
            }
        }

        /// <summary>
        ///     store Set to true to actually store the field in the index, false to not store it.
        ///     Defaults to false (note, the JSON document itself is stored, and it can be retrieved
        ///     from it).
        /// </summary>
        public virtual bool Store
        {
            get => _store;
            set
            {
                _store = value;
                _storeSet = true;
            }
        }

        /// <summary>
        ///     doc_values Set to true to store field values in a column-stride fashion.
        ///     Automatically set to true when the fielddata format is doc_values.
        /// </summary>
        public virtual bool DocValues
        {
            get => _docValues;
            set
            {
                _docValues = value;
                _docValuesSet = true;
            }
        }

        /// <summary>
        ///     boost The boost value. Defaults to 1.0. 
        /// </summary>
        public virtual double Boost
        {
            get => _boost;
            set
            {
                _boost = value;
                _boostSet = true;
            }
        }

        /// <summary>
        ///     null_value When there is a (JSON) null value for the field, use the null_value as the
        ///     field value. Defaults to not adding the field at all.
        /// </summary>
        public virtual object NullValue
        {
            get => _nullValue;
            set
            {
                _nullValue = value;
                _nullValueSet = true;
            }
        }

        public override string JsonString()
        {
            var elasticCrudJsonWriter = new ElasticJsonWriter();
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("type", "boolean", elasticCrudJsonWriter);
            JsonHelper.WriteValue("index", _index.ToString(), elasticCrudJsonWriter, _indexSet);
            JsonHelper.WriteValue("index_name", _indexName, elasticCrudJsonWriter, _indexNameSet);
            JsonHelper.WriteValue("store", _store, elasticCrudJsonWriter, _storeSet);
            JsonHelper.WriteValue("doc_values", _docValues, elasticCrudJsonWriter, _docValuesSet);
            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);
            JsonHelper.WriteValue("null_value", _nullValue, elasticCrudJsonWriter, _nullValueSet);

            WriteBaseValues(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            return elasticCrudJsonWriter.Stringbuilder.ToString();
        }
    }
}