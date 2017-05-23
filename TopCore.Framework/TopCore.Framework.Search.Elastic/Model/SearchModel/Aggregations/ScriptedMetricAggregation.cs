using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Aggregations
{
    /// <summary>
    ///     map_script Executed once per document collected. This is the only required script. If no
    ///     combine_script is specified, the resulting state needs to be stored in an object named _agg.
    /// </summary>
    public class ScriptedMetricAggregation : IAggs
    {
        private readonly string _mapScript;
        private readonly string _name;
        private string _combineScript;
        private string _combineScriptFile;
        private bool _combineScriptFileSet;
        private string _combineScriptId;
        private bool _combineScriptIdSet;
        private bool _combineScriptSet;
        private string _initScript;
        private string _initScriptFile;
        private bool _initScriptFileSet;
        private string _initScriptId;
        private bool _initScriptIdSet;
        private bool _initScriptSet;
        private string _lang;
        private bool _langSet;
        private string _mapScriptFile;
        private bool _mapScriptFileSet;
        private string _mapScriptId;
        private bool _mapScriptIdSet;
        private ParamsForScript _params;
        private bool _paramsSet;
        private ParamsForScript _reduceParams;
        private bool _reduceParamsSet;
        private string _reduceScript;
        private string _reduceScriptFile;
        private bool _reduceScriptFileSet;
        private string _reduceScriptId;
        private bool _reduceScriptIdSet;
        private bool _reduceScriptSet;

        public ScriptedMetricAggregation(string name, string mapScript)
        {
            _name = name;
            _mapScript = mapScript;
        }

        public ParamsForScript Params
        {
            get => _params;
            set
            {
                _params = value;
                _paramsSet = true;
            }
        }

        /// <summary>
        ///     init_script Executed prior to any collection of documents. Allows the aggregation to
        ///     set up any initial state.
        /// </summary>
        public string InitScript
        {
            get => _initScript;
            set
            {
                _initScript = value;
                _initScriptSet = true;
            }
        }

        /// <summary>
        ///     combine_script Executed once on each shard after document collection is complete.
        ///     Allows the aggregation to consolidate the state returned from each shard. If a
        ///     combine_script is not provided the combine phase will return the aggregation variable.
        /// </summary>
        public string CombineScript
        {
            get => _combineScript;
            set
            {
                _combineScript = value;
                _combineScriptSet = true;
            }
        }

        /// <summary>
        ///     reduce_script Executed once on the coordinating node after all shards have returned
        ///     their results. The script is provided with access to a variable _aggs which is an
        ///     array of the result of the combine_script on each shard. If a reduce_script is not
        ///     provided the reduce phase will return the _aggs variable.
        /// </summary>
        public string ReduceScript
        {
            get => _reduceScript;
            set
            {
                _reduceScript = value;
                _reduceScriptSet = true;
            }
        }

        /// <summary>
        ///     reduce_params Optional. An object whose contents will be passed as variables to the
        ///     reduce_script. This can be useful to allow the user to control the behavior of the
        ///     reduce phase. If this is not specified the variable will be undefined in the
        ///     reduce_script execution.
        /// </summary>
        public ParamsForScript ReduceParams
        {
            get => _reduceParams;
            set
            {
                _reduceParams = value;
                _reduceParamsSet = true;
            }
        }

        /// <summary>
        ///     lang Optional. The script language used for the scripts. If this is not specified the
        ///     default scripting language is used.
        /// </summary>
        public string Lang
        {
            get => _lang;
            set
            {
                _lang = value;
                _langSet = true;
            }
        }

        /// <summary>
        ///     init_script_file Optional. Can be used in place of the init_script parameter to
        ///     provide the script using in a file.
        /// </summary>
        public string InitScriptFile
        {
            get => _initScriptFile;
            set
            {
                _initScriptFile = value;
                _initScriptFileSet = true;
            }
        }

        /// <summary>
        ///     init_script_id Optional. Can be used in place of the init_script parameter to provide
        ///     the script using an indexed script.
        /// </summary>
        public string InitScriptId
        {
            get => _initScriptId;
            set
            {
                _initScriptId = value;
                _initScriptIdSet = true;
            }
        }

        /// <summary>
        ///     map_script_file Optional. Can be used in place of the map_script parameter to provide
        ///     the script using in a file.
        /// </summary>
        public string MapScriptFile
        {
            get => _mapScriptFile;
            set
            {
                _mapScriptFile = value;
                _mapScriptFileSet = true;
            }
        }

        /// <summary>
        ///     map_script_id Optional. Can be used in place of the map_script parameter to provide
        ///     the script using an indexed script.
        /// </summary>
        public string MapScriptId
        {
            get => _mapScriptId;
            set
            {
                _mapScriptId = value;
                _mapScriptIdSet = true;
            }
        }

        /// <summary>
        ///     combine_script_file Optional. Can be used in place of the combine_script parameter to
        ///     provide the script using in a file.
        /// </summary>
        public string CombineScriptFile
        {
            get => _combineScriptFile;
            set
            {
                _combineScriptFile = value;
                _combineScriptFileSet = true;
            }
        }

        /// <summary>
        ///     combine_script_id Optional. Can be used in place of the combine_script parameter to
        ///     provide the script using an indexed script.
        /// </summary>
        public string CombineScriptId
        {
            get => _combineScriptId;
            set
            {
                _combineScriptId = value;
                _combineScriptIdSet = true;
            }
        }

        /// <summary>
        ///     reduce_script_file Optional. Can be used in place of the reduce_script parameter to
        ///     provide the script using in a file.
        /// </summary>
        public string ReduceScriptFile
        {
            get => _reduceScriptFile;
            set
            {
                _reduceScriptFile = value;
                _reduceScriptFileSet = true;
            }
        }

        /// <summary>
        ///     reduce_script_id Optional. Can be used in place of the reduce_script parameter to
        ///     provide the script using an indexed script.
        /// </summary>
        public string ReduceScriptId
        {
            get => _reduceScriptId;
            set
            {
                _reduceScriptId = value;
                _reduceScriptIdSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_name);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("scripted_metric");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("map_script");
            elasticCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _mapScript + "\"");

            if (_initScriptSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("init_script");
                elasticCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _initScript + "\"");
            }

            if (_combineScriptSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("combine_script");
                elasticCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _combineScript + "\"");
            }

            if (_reduceScriptSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("reduce_script");
                elasticCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _reduceScript + "\"");
            }

            if (_paramsSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("params");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                elasticCrudJsonWriter.JsonWriter.WritePropertyName(_params.TransactionName);
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                foreach (var item in _params.Params)
                {
                    elasticCrudJsonWriter.JsonWriter.WritePropertyName(item.ParameterName);
                    elasticCrudJsonWriter.JsonWriter.WriteValue(item.ParameterValue);
                }
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            if (_reduceParamsSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("reduce_params");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                elasticCrudJsonWriter.JsonWriter.WritePropertyName(_reduceParams.TransactionName);
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                foreach (var item in _reduceParams.Params)
                {
                    elasticCrudJsonWriter.JsonWriter.WritePropertyName(item.ParameterName);
                    elasticCrudJsonWriter.JsonWriter.WriteValue(item.ParameterValue);
                }
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            JsonHelper.WriteValue("lang", _lang, elasticCrudJsonWriter, _langSet);
            JsonHelper.WriteValue("init_script_file", _initScriptFile, elasticCrudJsonWriter, _initScriptFileSet);
            JsonHelper.WriteValue("init_script_id", _initScriptId, elasticCrudJsonWriter, _initScriptIdSet);
            JsonHelper.WriteValue("map_script_file", _mapScriptFile, elasticCrudJsonWriter, _mapScriptFileSet);
            JsonHelper.WriteValue("map_script_id", _mapScriptId, elasticCrudJsonWriter, _mapScriptIdSet);
            JsonHelper.WriteValue("combine_script_file", _combineScriptFile, elasticCrudJsonWriter,
                _combineScriptFileSet);
            JsonHelper.WriteValue("combine_script_id", _combineScriptId, elasticCrudJsonWriter, _combineScriptIdSet);
            JsonHelper.WriteValue("reduce_script_file", _reduceScriptFile, elasticCrudJsonWriter, _reduceScriptFileSet);
            JsonHelper.WriteValue("reduce_script_id", _reduceScriptId, elasticCrudJsonWriter, _reduceScriptIdSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }

    public class ParamsForScript
    {
        public string TransactionName;

        public ParamsForScript(string transactionName)
        {
            TransactionName = transactionName;
        }

        public List<ScriptParameter> Params { get; set; }
    }
}