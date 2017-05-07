using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	public class IndexUpdateSettings
    {
        public enum RoutingAllocationEnableEnum
        {
            all,
            primaries,
            new_primaries,
            none
        }

        public enum TranslogFsTypeEnum
        {
            simple,
            buffered
        }

        private string _autoExpandReplicas;
        private bool _autoExpandReplicasSet;
        private bool _blocksMetadata;
        private bool _blocksMetadataSet;
        private bool _blocksRead;
        private bool _blocksReadOnly;
        private bool _blocksReadOnlySet;
        private bool _blocksReadSet;
        private bool _blocksWrite;
        private bool _blocksWriteSet;
        private string _cacheFilterExpire;
        private bool _cacheFilterExpireSet;
        private string _cacheFilterMaxSize;
        private bool _cacheFilterMaxSizeSet;
        private bool _codecBloomLoad;
        private bool _codecBloomLoadSet;
        private bool _compoundFormat;
        private bool _compoundFormatSet;
        private bool _compoundOnFlush;
        private bool _compoundOnFlushSet;
        private bool _failOnMergeFailure;
        private bool _failOnMergeFailureSet;
        private string _gatewaySnapshotInterval;
        private bool _gatewaySnapshotIntervalSet;
        private bool _gcDeletes;
        private bool _gcDeletesSet;
        private ushort _indexConcurrency;
        private bool _indexConcurrencySet;
        private int _numberOfReplicas;
        private bool _numberOfReplicasSet;
        private string _recoveryInitialShards;
        private bool _recoveryInitialShardsSet;

        private string _refreshInterval;
        private bool _refreshIntervalSet;
        private RoutingAllocationEnableEnum _routingAllocationEnable;
        private bool _routingAllocationEnableSet;
        private List<RoutingAllocation> _routingAllocationExclude;
        private bool _routingAllocationExcludeSet;
        private List<RoutingAllocation> _routingAllocationInclude;
        private bool _routingAllocationIncludeSet;
        private List<RoutingAllocation> _routingAllocationRequire;
        private bool _routingAllocationRequireSet;
        private string _routingAllocationTotalShardsPerNode;
        private bool _routingAllocationTotalShardsPerNodeSet;
        private string _translogDisableFlush;
        private bool _translogDisableFlushSet;
        private string _translogFlushThresholdOps;
        private bool _translogFlushThresholdOpsSet;
        private string _translogflushThresholdPeriod;
        private bool _translogflushThresholdPeriodSet;
        private string _translogFlushThresholdSize;
        private bool _translogFlushThresholdSizeSet;
        private TranslogFsTypeEnum _translogFsType;
        private bool _translogFsTypeSet;
        private bool _ttlDisablePurge;
        private bool _ttlDisablePurgeSet;
        private bool _warmerEnabled;
        private bool _warmerEnabledSet;

        public IndexUpdateSettings()
        {
            Analysis = new Analysis();
            Similarities = new Similarities();
        }

        public Analysis Analysis { get; set; }

        public Similarities Similarities { get; set; }

	    /// <summary>
	    ///     index.number_of_replicas The number of replicas each shard has. 
	    /// </summary>
	    public int NumberOfReplicas
        {
            get => _numberOfReplicas;
            set
            {
                _numberOfReplicas = value;
                _numberOfReplicasSet = true;
            }
        }

	    /// <summary>
	    ///     index.refresh_interval The async refresh interval of a shard. ----- bulk Indexing Usage For example, the update settings API can be used to dynamically change the index from being more performant for bulk indexing, and then move it to more real time indexing state. Before the bulk
	    ///     indexing is started, use: "refresh_interval" : "-1" (Another optimization option is to start the index without any replicas, and only later adding them, but that really depends on the use case). Then, once bulk indexing is done, the settings can be updated (back to the defaults for
	    ///     example): "refresh_interval" : "1s" And, an optimize should be called: curl -XPOST 'http://localhost:9200/test/_optimize?max_num_segments=5'
	    /// </summary>
	    public string RefreshInterval
        {
            get => _refreshInterval;
            set
            {
                _refreshInterval = value;
                _refreshIntervalSet = true;
            }
        }

	    /// <summary>
	    ///     index.auto_expand_replicas (string) Set to a dash delimited lower and upper bound (e.g. 0-5) or one may use all as the upper bound (e.g. 0-all), or false to disable it. 
	    /// </summary>
	    public string AutoExpandReplicas
        {
            get => _autoExpandReplicas;
            set
            {
                _autoExpandReplicas = value;
                _autoExpandReplicasSet = true;
            }
        }

	    /// <summary>
	    ///     index.blocks.read_only Set to true to have the index read only, false to allow writes and metadata changes. 
	    /// </summary>
	    public bool BlocksReadOnly
        {
            get => _blocksReadOnly;
            set
            {
                _blocksReadOnly = value;
                _blocksReadOnlySet = true;
            }
        }

	    /// <summary>
	    ///     index.blocks.read Set to true to disable read operations against the index. 
	    /// </summary>
	    public bool BlocksRead
        {
            get => _blocksRead;
            set
            {
                _blocksRead = value;
                _blocksReadSet = true;
            }
        }

	    /// <summary>
	    ///     index.blocks.write Set to true to disable write operations against the index. 
	    /// </summary>
	    public bool BlocksWrite
        {
            get => _blocksRead;
            set
            {
                _blocksWrite = value;
                _blocksWriteSet = true;
            }
        }

	    /// <summary>
	    ///     index.blocks.metadata Set to true to disable metadata operations against the index. 
	    /// </summary>
	    public bool BlocksMetadata
        {
            get => _blocksMetadata;
            set
            {
                _blocksMetadata = value;
                _blocksMetadataSet = true;
            }
        }

	    /// <summary>
	    ///     index.index_concurrency Defaults to 8. 
	    /// </summary>
	    public ushort IndexConcurrency
        {
            get => _indexConcurrency;
            set
            {
                _indexConcurrency = value;
                _indexConcurrencySet = true;
            }
        }

	    /// <summary>
	    ///     index.codec.bloom.load Whether to load the bloom filter. Defaults to false. 
	    /// </summary>
	    public bool CodecBloomLoad
        {
            get => _codecBloomLoad;
            set
            {
                _codecBloomLoad = value;
                _codecBloomLoadSet = true;
            }
        }

	    /// <summary>
	    ///     index.fail_on_merge_failure Default to true. 
	    /// </summary>
	    public bool FailOnMergeFailure
        {
            get => _failOnMergeFailure;
            set
            {
                _failOnMergeFailure = value;
                _failOnMergeFailureSet = true;
            }
        }

	    /// <summary>
	    ///     index.translog.flush_threshold_ops When to flush based on operations. http://www.elastic.org/guide/en/elastic/reference/current/index-modules-translog.html 
	    /// </summary>
	    public string TranslogFlushThresholdOps
        {
            get => _translogFlushThresholdOps;
            set
            {
                _translogFlushThresholdOps = value;
                _translogFlushThresholdOpsSet = true;
            }
        }

	    /// <summary>
	    ///     index.translog.flush_threshold_size When to flush based on translog (bytes) size. 
	    /// </summary>
	    public string TranslogFlushThresholdSize
        {
            get => _translogFlushThresholdSize;
            set
            {
                _translogFlushThresholdSize = value;
                _translogFlushThresholdSizeSet = true;
            }
        }

	    /// <summary>
	    ///     index.translog.flush_threshold_period When to flush based on a period of not flushing. 
	    /// </summary>
	    public string TranslogflushThresholdPeriod
        {
            get => _translogflushThresholdPeriod;
            set
            {
                _translogflushThresholdPeriod = value;
                _translogflushThresholdPeriodSet = true;
            }
        }

	    /// <summary>
	    ///     index.translog.disable_flush Disables flushing. Note, should be set for a short interval and then enabled. 
	    /// </summary>
	    public string TranslogDisableFlush
        {
            get => _translogDisableFlush;
            set
            {
                _translogDisableFlush = value;
                _translogDisableFlushSet = true;
            }
        }

	    /// <summary>
	    ///     index.cache.filter.max_size The maximum size of filter cache (per segment in shard). Set to -1 to disable. 
	    /// </summary>
	    public string CacheFilterMaxSize
        {
            get => _cacheFilterMaxSize;
            set
            {
                _cacheFilterMaxSize = value;
                _cacheFilterMaxSizeSet = true;
            }
        }

	    /// <summary>
	    ///     index.cache.filter.expire The expire after access time for filter cache. Set to -1 to disable. 
	    /// </summary>
	    public string CacheFilterExpire
        {
            get => _cacheFilterExpire;
            set
            {
                _cacheFilterExpire = value;
                _cacheFilterExpireSet = true;
            }
        }

	    /// <summary>
	    ///     index.gateway.snapshot_interval The gateway snapshot interval (only applies to shared gateways). Defaults to 10s. 
	    /// </summary>
	    public string GatewaySnapshotInterval
        {
            get => _gatewaySnapshotInterval;
            set
            {
                _gatewaySnapshotInterval = value;
                _gatewaySnapshotIntervalSet = true;
            }
        }

	    /// <summary>
	    ///     index.routing.allocation.include.* A node matching any rule will be allowed to host shards from the index. 
	    /// </summary>
	    public List<RoutingAllocation> RoutingAllocationInclude
        {
            get => _routingAllocationInclude;
            set
            {
                _routingAllocationInclude = value;
                _routingAllocationIncludeSet = true;
            }
        }

	    /// <summary>
	    ///     index.routing.allocation.exclude.* A node matching any rule will NOT be allowed to host shards from the index. 
	    /// </summary>
	    public List<RoutingAllocation> RoutingAllocationExclude
        {
            get => _routingAllocationExclude;
            set
            {
                _routingAllocationExclude = value;
                _routingAllocationExcludeSet = true;
            }
        }

	    /// <summary>
	    ///     index.routing.allocation.require.* Only nodes matching all rules will be allowed to host shards from the index. 
	    /// </summary>
	    public List<RoutingAllocation> RoutingAllocationRequire
        {
            get => _routingAllocationRequire;
            set
            {
                _routingAllocationRequire = value;
                _routingAllocationRequireSet = true;
            }
        }

	    /// <summary>
	    ///     index.routing.allocation.enable Enables shard allocation for a specific index. It can be set to: all (default) - Allows shard allocation for all shards. primaries - Allows shard allocation only for primary shards. new_primaries - Allows shard allocation only for primary shards for new
	    ///     indices. none - No shard allocation is allowed.
	    /// </summary>
	    public RoutingAllocationEnableEnum RoutingAllocationEnable
        {
            get => _routingAllocationEnable;
            set
            {
                _routingAllocationEnable = value;
                _routingAllocationEnableSet = true;
            }
        }

	    /// <summary>
	    ///     index.routing.allocation.total_shards_per_node Controls the total number of shards (replicas and primaries) allowed to be allocated on a single node. Defaults to unbounded (-1). 
	    /// </summary>
	    public string RoutingAllocationTotalShardsPerNode
        {
            get => _routingAllocationTotalShardsPerNode;
            set
            {
                _routingAllocationTotalShardsPerNode = value;
                _routingAllocationTotalShardsPerNodeSet = true;
            }
        }

	    /// <summary>
	    ///     index.recovery.initial_shards When using local gateway a particular shard is recovered only if there can be allocated quorum shards in the cluster. It can be set to: quorum (default) quorum-1 (or half) full full-1. Number values are also supported, e.g. 1. 
	    /// </summary>
	    public string RecoveryInitialShards
        {
            get => _recoveryInitialShards;
            set
            {
                _recoveryInitialShards = value;
                _recoveryInitialShardsSet = true;
            }
        }

	    /// <summary>
	    ///     index.ttl.disable_purge Disables temporarily the purge of expired docs. 
	    /// </summary>
	    public bool TtlDisablePurge
        {
            get => _ttlDisablePurge;
            set
            {
                _ttlDisablePurge = value;
                _ttlDisablePurgeSet = true;
            }
        }

	    /// <summary>
	    ///     index.gc_deletes Disables temporarily the purge of expired docs. 
	    /// </summary>
	    public bool GcDeletes
        {
            get => _gcDeletes;
            set
            {
                _gcDeletes = value;
                _gcDeletesSet = true;
            }
        }

	    /// <summary>
	    ///     index.translog.fs.type Either simple or buffered (default). 
	    /// </summary>
	    public TranslogFsTypeEnum TranslogFsType
        {
            get => _translogFsType;
            set
            {
                _translogFsType = value;
                _translogFsTypeSet = true;
            }
        }

	    /// <summary>
	    ///     index.compound_format See index.compound_format in the section called “Index Settingsedit”. 
	    /// </summary>
	    public bool CompoundFormat
        {
            get => _compoundFormat;
            set
            {
                _compoundFormat = value;
                _compoundFormatSet = true;
            }
        }

	    /// <summary>
	    ///     index.compound_on_flush See `index.compound_on_flush in the section called “Index Settingsedit”. 
	    /// </summary>
	    public bool CompoundOnFlush
        {
            get => _compoundOnFlush;
            set
            {
                _compoundOnFlush = value;
                _compoundOnFlushSet = true;
            }
        }

	    /// <summary>
	    ///     index.warmer.enabled See Warmers. Defaults to true. 
	    /// </summary>
	    public bool WarmerEnabled
        {
            get => _warmerEnabled;
            set
            {
                _warmerEnabled = value;
                _warmerEnabledSet = true;
            }
        }

        public virtual void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("number_of_replicas", _numberOfReplicas, elasticCrudJsonWriter, _numberOfReplicasSet);
            JsonHelper.WriteValue("refresh_interval", _refreshInterval, elasticCrudJsonWriter, _refreshIntervalSet);
            JsonHelper.WriteValue("auto_expand_replicas", _autoExpandReplicas, elasticCrudJsonWriter,
                _autoExpandReplicasSet);
            JsonHelper.WriteValue("blocks.read_only", _blocksReadOnly, elasticCrudJsonWriter, _blocksReadOnlySet);
            JsonHelper.WriteValue("blocks.read", _blocksRead, elasticCrudJsonWriter, _blocksReadSet);
            JsonHelper.WriteValue("blocks.write", _blocksWrite, elasticCrudJsonWriter, _blocksWriteSet);
            JsonHelper.WriteValue("blocks.metadata", _blocksMetadata, elasticCrudJsonWriter, _blocksMetadataSet);
            JsonHelper.WriteValue("index_concurrency", _indexConcurrency, elasticCrudJsonWriter, _indexConcurrencySet);
            JsonHelper.WriteValue("codec.bloom.load", _codecBloomLoad, elasticCrudJsonWriter, _codecBloomLoadSet);
            JsonHelper.WriteValue("fail_on_merge_failure", _failOnMergeFailure, elasticCrudJsonWriter,
                _failOnMergeFailureSet);
            JsonHelper.WriteValue("translog.flush_threshold_ops", _translogFlushThresholdOps, elasticCrudJsonWriter,
                _translogFlushThresholdOpsSet);
            JsonHelper.WriteValue("translog.flush_threshold_size", _translogFlushThresholdSize, elasticCrudJsonWriter,
                _translogFlushThresholdSizeSet);
            JsonHelper.WriteValue("translog.flush_threshold_period", _translogflushThresholdPeriod,
                elasticCrudJsonWriter, _translogflushThresholdPeriodSet);
            JsonHelper.WriteValue("translog.disable_flush", _translogDisableFlush, elasticCrudJsonWriter,
                _translogDisableFlushSet);
            JsonHelper.WriteValue("cache.filter.max_size", _cacheFilterMaxSize, elasticCrudJsonWriter,
                _cacheFilterMaxSizeSet);
            JsonHelper.WriteValue("cache.filter.expire", _cacheFilterExpire, elasticCrudJsonWriter,
                _cacheFilterExpireSet);
            JsonHelper.WriteValue("gateway.snapshot_interval", _gatewaySnapshotInterval, elasticCrudJsonWriter,
                _gatewaySnapshotIntervalSet);

            WriteListValue("routing.allocation.include.", _routingAllocationInclude, elasticCrudJsonWriter,
                _routingAllocationIncludeSet);
            WriteListValue("routing.allocation.exclude.", _routingAllocationExclude, elasticCrudJsonWriter,
                _routingAllocationExcludeSet);
            WriteListValue("routing.allocation.require.", _routingAllocationRequire, elasticCrudJsonWriter,
                _routingAllocationRequireSet);

            JsonHelper.WriteValue("routing.allocation.enable", _routingAllocationEnable.ToString(),
                elasticCrudJsonWriter, _routingAllocationEnableSet);
            JsonHelper.WriteValue("routing.allocation.total_shards_per_node", _routingAllocationTotalShardsPerNode,
                elasticCrudJsonWriter, _routingAllocationTotalShardsPerNodeSet);
            JsonHelper.WriteValue("recovery.initial_shards", _recoveryInitialShards, elasticCrudJsonWriter,
                _recoveryInitialShardsSet);

            JsonHelper.WriteValue("index.gc_deletes", _gcDeletes, elasticCrudJsonWriter, _gcDeletesSet);
            JsonHelper.WriteValue("ttl.disable_purge", _ttlDisablePurge, elasticCrudJsonWriter, _ttlDisablePurgeSet);
            JsonHelper.WriteValue("translog.fs.type", _translogFsType.ToString(), elasticCrudJsonWriter,
                _translogFsTypeSet);
            JsonHelper.WriteValue("compound_format", _compoundFormat, elasticCrudJsonWriter, _compoundFormatSet);
            JsonHelper.WriteValue("compound_on_flush", _compoundOnFlush, elasticCrudJsonWriter, _compoundOnFlushSet);
            JsonHelper.WriteValue("warmer.enabled", _warmerEnabled, elasticCrudJsonWriter, _warmerEnabledSet);
        }

        private void WriteListValue(string key, List<RoutingAllocation> valueObj,
            ElasticJsonWriter elasticCrudJsonWriter, bool writeValue = true)
        {
            if (writeValue)
                foreach (var obj in valueObj)
                {
                    elasticCrudJsonWriter.JsonWriter.WritePropertyName(key + obj.Key);
                    elasticCrudJsonWriter.JsonWriter.WriteValue(obj.Value);
                }
        }

        public class RoutingAllocation
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}