using System;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    /// <summary>
    ///     type The type of the number. Can be float, double, integer, long, short, byte. Required. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ElasticNumber : ElasticCoreTypes
    {
        private double _boost;
        private bool _boostSet;
        private bool _coerce;
        private bool _coerceSet;
        private bool _docValues;
        private bool _docValuesSet;
        private bool _ignoreMalformed;
        private bool _ignoreMalformedSet;
        private bool _includeInAll;
        private bool _includeInAllSet;
        private NumberIndex _index;
        private string _indexName;

        private bool _indexNameSet;
        private bool _indexSet;
        private object _nullValue;
        private bool _nullValueSet;
        private int _precisionStep;
        private bool _precisionStepSet;
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

        /// <summary>
        ///     include_in_all Should the field be included in the _all field (if enabled). If index
        ///     is set to no this defaults to false, otherwise, defaults to true or to the parent
        ///     object type setting.
        /// </summary>
        public virtual bool IncludeInAll
        {
            get => _includeInAll;
            set
            {
                _includeInAll = value;
                _includeInAllSet = true;
            }
        }

        /// <summary>
        ///     precision_step The precision step (influences the number of terms generated for each
        ///     number value). Defaults to 16 for long, double, 8 for short, integer, float, and
        ///     2147483647 for byte.
        /// </summary>
        public virtual int PrecisionStep
        {
            get => _precisionStep;
            set
            {
                _precisionStep = value;
                _precisionStepSet = true;
            }
        }

        /// <summary>
        ///     ignore_malformed Ignored a malformed number. Defaults to false. 
        /// </summary>
        public virtual bool IgnoreMalformed
        {
            get => _ignoreMalformed;
            set
            {
                _ignoreMalformed = value;
                _ignoreMalformedSet = true;
            }
        }

        /// <summary>
        ///     coerce Try convert strings to numbers and truncate fractions for integers. Defaults
        ///     to true.
        /// </summary>
        public virtual bool Coerce
        {
            get => _coerce;
            set
            {
                _coerce = value;
                _coerceSet = true;
            }
        }

        protected string JsonStringInternal(string typeProperty)
        {
            var elasticCrudJsonWriter = new ElasticJsonWriter();
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("type", typeProperty, elasticCrudJsonWriter);
            JsonHelper.WriteValue("index_name", _indexName, elasticCrudJsonWriter, _indexNameSet);
            JsonHelper.WriteValue("store", _store, elasticCrudJsonWriter, _storeSet);
            JsonHelper.WriteValue("index", _index.ToString(), elasticCrudJsonWriter, _indexSet);
            JsonHelper.WriteValue("doc_values", _docValues, elasticCrudJsonWriter, _docValuesSet);
            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);
            JsonHelper.WriteValue("null_value", _nullValue, elasticCrudJsonWriter, _nullValueSet);
            JsonHelper.WriteValue("include_in_all", _includeInAll, elasticCrudJsonWriter, _includeInAllSet);
            JsonHelper.WriteValue("precision_step", _precisionStep, elasticCrudJsonWriter, _precisionStepSet);
            JsonHelper.WriteValue("ignore_malformed", _ignoreMalformed, elasticCrudJsonWriter, _ignoreMalformedSet);
            JsonHelper.WriteValue("coerce", _coerce, elasticCrudJsonWriter, _coerceSet);

            WriteBaseValues(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            return elasticCrudJsonWriter.Stringbuilder.ToString();
        }
    }

    public enum NumberIndex
    {
        no
    }
}