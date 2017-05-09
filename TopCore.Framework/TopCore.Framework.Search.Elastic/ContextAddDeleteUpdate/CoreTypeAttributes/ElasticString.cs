using System;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ElasticString : ElasticCoreTypes
    {
        private string _analyzer;
        private bool _analyzerSet;
        private double _boost;
        private bool _boostSet;
        private bool _docValues;
        private bool _docValuesSet;
        private string _ignoreAbove;
        private bool _ignoreAboveSet;
        private bool _includeInAll;
        private bool _includeInAllSet;
        private StringIndex _index;
        private string _indexAnalyzer;
        private bool _indexAnalyzerSet;
        private string _indexName;

        private bool _indexNameSet;
        private IndexOptions _indexOptions;
        private bool _indexOptionsSet;
        private bool _indexSet;
        private bool _normsEnabled;
        private bool _normsEnabledSet;
        private NormsLoading _normsLoading;
        private bool _normsLoadingSet;
        private object _nullValue;
        private bool _nullValueSet;
        private long _positionOffsetGap;
        private bool _positionOffsetGapSet;
        private string _searchAnalyzer;
        private bool _searchAnalyzerSet;
        private bool _store;
        private bool _storeSet;
        private TermVector _termVector;
        private bool _termVectorSet;

        /// <summary>
        ///     index_name The name of the field that will be stored in the index. Defaults to the property/field name. 
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
        ///     store Set to true to actually store the field in the index, false to not store it. Defaults to false (note, the JSON document itself is stored, and it can be retrieved from it). 
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
        ///     index Set to analyzed for the field to be indexed and searchable after being broken down into token using an analyzer. not_analyzed means that its still searchable, but does not go through any analysis process or broken down into tokens. no means that it won’t be searchable at all (as
        ///     an individual field; it may still be included in _all). Setting to no disables include_in_all. Defaults to analyzed.
        /// </summary>
        public virtual StringIndex Index
        {
            get => _index;
            set
            {
                _index = value;
                _indexSet = true;
            }
        }

        /// <summary>
        ///     doc_values Set to true to store field values in a column-stride fashion. Automatically set to true when the fielddata format is doc_values. 
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
        ///     term_vector Possible values are no, yes, with_offsets, with_positions, with_positions_offsets. Defaults to no. 
        /// </summary>
        public virtual TermVector TermVector
        {
            get => _termVector;
            set
            {
                _termVector = value;
                _termVectorSet = true;
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
        ///     null_value When there is a (JSON) null value for the field, use the null_value as the field value. Defaults to not adding the field at all. 
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
        ///     norms: {enabled: value} Boolean value if norms should be enabled or not. Defaults to true for analyzed fields, and to false for not_analyzed fields. See the section about norms. 
        /// </summary>
        public virtual bool NormsEnabled
        {
            get => _normsEnabled;
            set
            {
                _normsEnabled = value;
                _normsEnabledSet = true;
            }
        }

        /// <summary>
        ///     norms: {loading: value} Describes how norms should be loaded, possible values are eager and lazy (default). It is possible to change the default value to eager for all fields by configuring the index setting index.norms.loading to eager. 
        /// </summary>
        public virtual NormsLoading NormsLoading
        {
            get => _normsLoading;
            set
            {
                _normsLoading = value;
                _normsLoadingSet = true;
            }
        }

        /// <summary>
        ///     index_options Allows to set the indexing options, possible values are docs (only doc numbers are indexed), freqs (doc numbers and term frequencies), and positions (doc numbers, term frequencies and positions). Defaults to positions for analyzed fields, and to docs for not_analyzed
        ///     fields. It is also possible to set it to offsets (doc numbers, term frequencies, positions and offsets).
        /// </summary>
        public virtual IndexOptions IndexOptions
        {
            get => _indexOptions;
            set
            {
                _indexOptions = value;
                _indexOptionsSet = true;
            }
        }

        /// <summary>
        ///     analyzer The analyzer used to analyze the text contents when analyzed during indexing and when searching using a query string. Defaults to the globally configured analyzer. 
        /// </summary>
        public virtual string Analyzer
        {
            get => _analyzer;
            set
            {
                _analyzer = value;
                _analyzerSet = true;
            }
        }

        /// <summary>
        ///     index_analyzer The analyzer used to analyze the text contents when analyzed during indexing. 
        /// </summary>
        public virtual string IndexAnalyzer
        {
            get => _indexAnalyzer;
            set
            {
                _indexAnalyzer = value;
                _indexAnalyzerSet = true;
            }
        }

        /// <summary>
        ///     search_analyzer The analyzer used to analyze the field when part of a query string. Can be updated on an existing field. 
        /// </summary>
        public virtual string SearchAnalyzer
        {
            get => _searchAnalyzer;
            set
            {
                _searchAnalyzer = value;
                _searchAnalyzerSet = true;
            }
        }

        /// <summary>
        ///     include_in_all Should the field be included in the _all field (if enabled). If index is set to no this defaults to false, otherwise, defaults to true or to the parent object type setting. 
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
        ///     ignore_above The analyzer will ignore strings larger than this size. Useful for generic not_analyzed fields that should ignore long text. 
        /// </summary>
        public virtual string IgnoreAbove
        {
            get => _ignoreAbove;
            set
            {
                _ignoreAbove = value;
                _ignoreAboveSet = true;
            }
        }

        /// <summary>
        ///     position_offset_gap Position increment gap between field instances with the same field name. Defaults to 0. 
        /// </summary>
        public virtual long PositionOffsetGap
        {
            get => _positionOffsetGap;
            set
            {
                _positionOffsetGap = value;
                _positionOffsetGapSet = true;
            }
        }

        public override string JsonString()
        {
            var elasticCrudJsonWriter = new ElasticJsonWriter();
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("type", "string", elasticCrudJsonWriter);
            JsonHelper.WriteValue("index_name", _indexName, elasticCrudJsonWriter, _indexNameSet);
            JsonHelper.WriteValue("store", _store, elasticCrudJsonWriter, _storeSet);
            JsonHelper.WriteValue("index", _index.ToString(), elasticCrudJsonWriter, _indexSet);
            JsonHelper.WriteValue("doc_values", _docValues, elasticCrudJsonWriter, _docValuesSet);
            JsonHelper.WriteValue("term_vector", _termVector.ToString(), elasticCrudJsonWriter, _termVectorSet);
            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);
            JsonHelper.WriteValue("null_value", _nullValue, elasticCrudJsonWriter, _nullValueSet);

            //"norms" : {
            //		"enabled" : false
            //	}
            if (_normsEnabledSet || _normsLoadingSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("norms");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                JsonHelper.WriteValue("enabled", _normsEnabled, elasticCrudJsonWriter, _normsEnabledSet);
                JsonHelper.WriteValue("loading", _normsLoading.ToString(), elasticCrudJsonWriter, _normsLoadingSet);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            JsonHelper.WriteValue("index_options", _indexOptions.ToString(), elasticCrudJsonWriter, _indexOptionsSet);
            JsonHelper.WriteValue("analyzer", _analyzer, elasticCrudJsonWriter, _analyzerSet);
            JsonHelper.WriteValue("index_analyzer", _indexAnalyzer, elasticCrudJsonWriter, _indexAnalyzerSet);
            JsonHelper.WriteValue("search_analyzer", _searchAnalyzer, elasticCrudJsonWriter, _searchAnalyzerSet);
            JsonHelper.WriteValue("include_in_all", _includeInAll, elasticCrudJsonWriter, _includeInAllSet);
            JsonHelper.WriteValue("ignore_above", _ignoreAbove, elasticCrudJsonWriter, _ignoreAboveSet);
            JsonHelper.WriteValue("position_offset_gap", _positionOffsetGap, elasticCrudJsonWriter,
                _positionOffsetGapSet);

            WriteBaseValues(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            return elasticCrudJsonWriter.Stringbuilder.ToString();
        }
    }

    public enum StringIndex
    {
        not_analyzed,
        analyzed,
        no
    }

    // no, yes, with_offsets, with_positions, with_positions_offsets. Defaults to no.
    public enum TermVector
    {
        no,
        yes,
        with_offsets,
        with_positions,
        with_positions_offsets
    }

    public enum NormsLoading
    {
        eager,
        lazy
    }

    public enum IndexOptions
    {
        docs,
        freqs,
        positions,
        offsets
    }
}