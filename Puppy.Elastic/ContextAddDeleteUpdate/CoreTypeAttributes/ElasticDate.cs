using Puppy.Elastic.Utils;
using System;

namespace Puppy.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ElasticDate : ElasticCoreTypes
    {
        private double _boost;
        private bool _boostSet;
        private bool _docValues;
        private bool _docValuesSet;

        private string _format;
        private bool _formatSet;
        private bool _ignoreMalformed;
        private bool _ignoreMalformedSet;
        private bool _includeInAll;
        private bool _includeInAllSet;
        private string _indexName;

        private bool _indexNameSet;
        private object _nullValue;
        private bool _nullValueSet;
        private int _precisionStep;
        private bool _precisionStepSet;
        private bool _store;
        private bool _storeSet;

        public ElasticDate()
        {
            // The format is always set.
            Format = "dateOptionalTime";
        }

        /// <summary>
        ///     The date format. Defaults to dateOptionalTime. The DateTimeFormats class can be used
        ///     for this.
        /// </summary>
        public virtual string Format
        {
            get => _format;
            set
            {
                _format = value;
                _formatSet = true;
            }
        }

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
        ///     number value). Defaults to 16.
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

        public override string JsonString()
        {
            var elasticCrudJsonWriter = new ElasticJsonWriter();
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("type", "date", elasticCrudJsonWriter);
            JsonHelper.WriteValue("format", _format, elasticCrudJsonWriter, _formatSet);
            JsonHelper.WriteValue("index_name", _indexName, elasticCrudJsonWriter, _indexNameSet);
            JsonHelper.WriteValue("store", _store, elasticCrudJsonWriter, _storeSet);
            JsonHelper.WriteValue("doc_values", _docValues, elasticCrudJsonWriter, _docValuesSet);
            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);
            JsonHelper.WriteValue("null_value", _nullValue, elasticCrudJsonWriter, _nullValueSet);
            JsonHelper.WriteValue("include_in_all", _includeInAll, elasticCrudJsonWriter, _includeInAllSet);
            JsonHelper.WriteValue("precision_step", _precisionStep, elasticCrudJsonWriter, _precisionStepSet);
            JsonHelper.WriteValue("ignore_malformed", _ignoreMalformed, elasticCrudJsonWriter, _ignoreMalformedSet);

            WriteBaseValues(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            return elasticCrudJsonWriter.Stringbuilder.ToString();
        }
    }
}