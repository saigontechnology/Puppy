using System;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ElasticBinary : ElasticCoreTypes
    {
        private bool _compress;
        private bool _compressSet;
        private int _compressThreshold;
        private bool _compressThresholdSet;
        private bool _docValues;
        private bool _docValuesSet;
        private string _indexName;

        private bool _indexNameSet;
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
        ///     compress Set to true to compress the stored binary value. 
        /// </summary>
        public virtual bool Compress
        {
            get => _compress;
            set
            {
                _compress = value;
                _compressSet = true;
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
        ///     //compress_threshold Compression will only be applied to stored binary fields that
        ///     are greater than this size. Defaults to -1
        /// </summary>
        public virtual int CompressThreshold
        {
            get => _compressThreshold;
            set
            {
                _compressThreshold = value;
                _compressThresholdSet = true;
            }
        }

        public override string JsonString()
        {
            var elasticCrudJsonWriter = new ElasticJsonWriter();
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("type", "binary", elasticCrudJsonWriter);
            JsonHelper.WriteValue("compress_threshold", _compressThreshold, elasticCrudJsonWriter,
                _compressThresholdSet);
            JsonHelper.WriteValue("index_name", _indexName, elasticCrudJsonWriter, _indexNameSet);
            JsonHelper.WriteValue("store", _store, elasticCrudJsonWriter, _storeSet);
            JsonHelper.WriteValue("doc_values", _docValues, elasticCrudJsonWriter, _docValuesSet);
            JsonHelper.WriteValue("compress", _compress, elasticCrudJsonWriter, _compressSet);

            WriteBaseValues(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            return elasticCrudJsonWriter.Stringbuilder.ToString();
        }
    }
}