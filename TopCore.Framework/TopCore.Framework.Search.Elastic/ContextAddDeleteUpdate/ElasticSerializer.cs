using System;
using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.MappingModel;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Tracing;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate
{
	public class ElasticSerializer : IDisposable
	{
		private readonly ITraceProvider _traceProvider;
		private ElasticJsonWriter _elasticCrudJsonWriter;
		private readonly ElasticSerializerConfiguration _elasticSerializerConfiguration;
		private readonly bool _saveChangesAndInitMappingsForChildDocuments;
		private ElasticSerializationResult _elasticSerializationResult = new ElasticSerializationResult();
		private readonly IndexMappings _indexMappings;

		public ElasticSerializer(ITraceProvider traceProvider, ElasticSerializerConfiguration elasticSerializerConfiguration, bool saveChangesAndInitMappingsForChildDocuments)
		{
			_elasticSerializerConfiguration = elasticSerializerConfiguration;
			_saveChangesAndInitMappingsForChildDocuments = saveChangesAndInitMappingsForChildDocuments;
			_traceProvider = traceProvider;
			_indexMappings = new IndexMappings(_traceProvider, _elasticSerializerConfiguration);
			_elasticSerializationResult.IndexMappings = _indexMappings;
		}

		public ElasticSerializationResult Serialize(IEnumerable<EntityContextInfo> entities)
		{
			if (entities == null)
			{
				return null;
			}

			_elasticSerializationResult = new ElasticSerializationResult();
			_elasticCrudJsonWriter = new ElasticJsonWriter();

			foreach (var entity in entities)
			{
				string index = _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(entity.EntityType).GetIndexForType(entity.EntityType);
				MappingUtils.GuardAgainstBadIndexName(index);

				if (_saveChangesAndInitMappingsForChildDocuments)
				{
					_indexMappings.CreateIndexSettingsAndMappingsForDocument(
						index,
						new IndexSettings { NumberOfShards = 5, NumberOfReplicas = 1 },
						new IndexAliases(),
						new IndexWarmers(),
						entity, new MappingDefinition { Index = index });
				}

				if (entity.DeleteDocument)
				{
					DeleteEntity(entity);
				}
				else
				{
					AddUpdateEntity(entity);
				}
			}

			_elasticCrudJsonWriter.Dispose();
			_elasticSerializationResult.Content = _elasticCrudJsonWriter.Stringbuilder.ToString();
			_elasticSerializationResult.IndexMappings = _indexMappings;
			return _elasticSerializationResult;
		}

		private void DeleteEntity(EntityContextInfo entityInfo)
		{
			var elasticSearchMapping = _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(entityInfo.EntityType);
			elasticSearchMapping.TraceProvider = _traceProvider;
			elasticSearchMapping.SaveChildObjectsAsWellAsParent = _elasticSerializerConfiguration.SaveChildObjectsAsWellAsParent;
			elasticSearchMapping.ProcessChildDocumentsAsSeparateChildIndex = _elasticSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex;
			_elasticCrudJsonWriter.JsonWriter.WriteStartObject();

			_elasticCrudJsonWriter.JsonWriter.WritePropertyName("delete");

			// Write the batch "index" operation header
			_elasticCrudJsonWriter.JsonWriter.WriteStartObject();
			WriteValue("_index", elasticSearchMapping.GetIndexForType(entityInfo.EntityType));
			WriteValue("_type", elasticSearchMapping.GetDocumentType(entityInfo.EntityType));
			WriteValue("_id", entityInfo.Id);
			if (entityInfo.RoutingDefinition.ParentId != null && _elasticSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
			{
				// It's a document which belongs to a parent
				WriteValue("_parent", entityInfo.RoutingDefinition.ParentId);
			}
			if (entityInfo.RoutingDefinition.RoutingId != null && _elasticSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex &&
				_elasticSerializerConfiguration.UserDefinedRouting)
			{
				// It's a document which has a specific route
				WriteValue("_routing", entityInfo.RoutingDefinition.RoutingId);
			}
			_elasticCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticCrudJsonWriter.JsonWriter.WriteRaw("\n");
		}

		private void AddUpdateEntity(EntityContextInfo entityInfo)
		{
			var elasticSearchMapping = _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(entityInfo.EntityType);
			elasticSearchMapping.TraceProvider = _traceProvider;
			elasticSearchMapping.SaveChildObjectsAsWellAsParent = _elasticSerializerConfiguration.SaveChildObjectsAsWellAsParent;
			elasticSearchMapping.ProcessChildDocumentsAsSeparateChildIndex = _elasticSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex;

			CreateBulkContentForParentDocument(entityInfo, elasticSearchMapping);

			if (_elasticSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
			{
				if (elasticSearchMapping.ChildIndexEntities.Count > 0)
				{
					// Only save the top level items now
					elasticSearchMapping.SaveChildObjectsAsWellAsParent = false;
					foreach (var item in elasticSearchMapping.ChildIndexEntities)
					{
						CreateBulkContentForChildDocument(entityInfo, elasticSearchMapping, item);
					}
				}
			}
			elasticSearchMapping.ChildIndexEntities.Clear();
		}

		private void CreateBulkContentForParentDocument(EntityContextInfo entityInfo, ElasticMapping elasticMapping)
		{
			_elasticCrudJsonWriter.JsonWriter.WriteStartObject();
			_elasticCrudJsonWriter.JsonWriter.WritePropertyName("index");
			// Write the batch "index" operation header
			_elasticCrudJsonWriter.JsonWriter.WriteStartObject();
			WriteValue("_index", elasticMapping.GetIndexForType(entityInfo.EntityType));
			WriteValue("_type", elasticMapping.GetDocumentType(entityInfo.EntityType));
			WriteValue("_id", entityInfo.Id);
			if (entityInfo.RoutingDefinition.ParentId != null && _elasticSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
			{
				// It's a document which belongs to a parent
				WriteValue("_parent", entityInfo.RoutingDefinition.ParentId);
			}
			if (entityInfo.RoutingDefinition.RoutingId != null &&
				_elasticSerializerConfiguration.UserDefinedRouting)
			{
				// It's a document which has a specific route
				WriteValue("_routing", entityInfo.RoutingDefinition.RoutingId);
			}
			_elasticCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticCrudJsonWriter.JsonWriter.WriteRaw("\n"); //ES requires this \n separator
			_elasticCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticMapping.MapEntityValues(entityInfo, _elasticCrudJsonWriter, true);

			_elasticCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticCrudJsonWriter.JsonWriter.WriteRaw("\n");
		}

		private void CreateBulkContentForChildDocument(EntityContextInfo entityInfo, ElasticMapping elasticMapping,
			EntityContextInfo item)
		{
			var childMapping =
				_elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(item.EntityType);

			_elasticCrudJsonWriter.JsonWriter.WriteStartObject();
			_elasticCrudJsonWriter.JsonWriter.WritePropertyName("index");
			// Write the batch "index" operation header
			_elasticCrudJsonWriter.JsonWriter.WriteStartObject();

			// Always write to the same index
			WriteValue("_index", childMapping.GetIndexForType(entityInfo.EntityType));
			WriteValue("_type", childMapping.GetDocumentType(item.EntityType));
			WriteValue("_id", item.Id);
			WriteValue("_parent", item.RoutingDefinition.ParentId);
			if (item.RoutingDefinition.RoutingId != null && _elasticSerializerConfiguration.UserDefinedRouting)
			{
				// It's a document which has a specific route
				WriteValue("_routing", item.RoutingDefinition.RoutingId);
			}
			_elasticCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticCrudJsonWriter.JsonWriter.WriteRaw("\n"); //ES requires this \n separator
			_elasticCrudJsonWriter.JsonWriter.WriteStartObject();

			childMapping.MapEntityValues(item, _elasticCrudJsonWriter, true);

			_elasticCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticCrudJsonWriter.JsonWriter.WriteRaw("\n");
		}

		private void WriteValue(string key, object valueObj)
		{
			_elasticCrudJsonWriter.JsonWriter.WritePropertyName(key);
			_elasticCrudJsonWriter.JsonWriter.WriteValue(valueObj);
		}

		public void Dispose()
		{
			_elasticCrudJsonWriter.Dispose();
		}
	}
}