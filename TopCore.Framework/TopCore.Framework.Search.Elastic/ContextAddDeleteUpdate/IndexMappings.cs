using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.MappingModel;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Tracing;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate
{
    public class IndexMappings
    {
        private const bool CreatePropertyMappings = true;
        private readonly ElasticSerializerConfiguration _elasticSerializerConfiguration;
        private readonly List<string> _processedItems = new List<string>();
        private readonly ITraceProvider _traceProvider;
        public List<MappingCommand> Commands = new List<MappingCommand>();

        public List<string> CommandTypes = new List<string>();

        public IndexMappings(ITraceProvider traceProvider,
            ElasticSerializerConfiguration elasticSerializerConfiguration)
        {
            _elasticSerializerConfiguration = elasticSerializerConfiguration;
            _traceProvider = traceProvider;
        }

        public async Task<ResultDetails<string>> Execute(HttpClient client, string baseUrl,
            ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource)
        {
            var resultDetails = new ResultDetails<string> { Status = HttpStatusCode.InternalServerError };
            foreach (var command in Commands)
            {
                var content = new StringContent(command.Content + "\n");
                traceProvider.Trace(TraceEventType.Verbose, "{1}: sending init mappings request: {0}", command,
                    "InitMappings");
                traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP PUT uri: {0}", command.Url,
                    "InitMappings");
                traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP PUT content: {0}", command.Content,
                    "InitMappings");

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response;
                if (command.RequestType == "POST")
                    response = await client.PostAsync(baseUrl + command.Url, content, cancellationTokenSource.Token)
                        .ConfigureAwait(true);
                else
                    response = await client.PutAsync(baseUrl + command.Url, content, cancellationTokenSource.Token)
                        .ConfigureAwait(true);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                    var responseObject = JObject.Parse(errorInfo);
                    var source = responseObject["error"];
                    throw new ElasticException("IndexMappings: Execute Request POST BadRequest: " + source);
                }

                //resultDetails.Status = response.StatusCode;
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                {
                    traceProvider.Trace(TraceEventType.Warning, "{2}: SaveChangesAsync response status code: {0}, {1}",
                        response.StatusCode, response.ReasonPhrase, "InitMappings");
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                        resultDetails.Description = errorInfo;
                        return resultDetails;
                    }

                    return resultDetails;
                }

                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                traceProvider.Trace(TraceEventType.Verbose, "{1}: response: {0}", responseString, "InitMappings");
            }

            // no errors
            resultDetails.Status = HttpStatusCode.OK;
            return resultDetails;
        }

        public void CreateIndexSettingsForDocument(string index, IndexSettings indexSettings, IndexAliases indexAliases,
            IndexWarmers indexWarmers)
        {
            if (_processedItems.Contains("_index" + index))
                return;
            _processedItems.Add("_index" + index);
            var elasticCrudJsonWriter = new ElasticJsonWriter();
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            CreateIndexSettings(elasticCrudJsonWriter, indexSettings);
            indexAliases.WriteJson(elasticCrudJsonWriter);
            indexWarmers.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            CreateIndexCommand(elasticCrudJsonWriter.GetJsonString(), index);
        }

        public void CreateIndexSettingsAndMappingsForDocument(string index,
            IndexSettings indexSettings,
            IndexAliases indexAliases,
            IndexWarmers indexWarmers,
            EntityContextInfo entityInfo,
            MappingDefinition mappingDefinition)
        {
            if (_processedItems.Contains("_index" + index))
                return;
            _processedItems.Add("_index" + index);
            var elasticCrudJsonWriter = new ElasticJsonWriter();
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            CreateIndexSettings(elasticCrudJsonWriter, indexSettings);
            indexAliases.WriteJson(elasticCrudJsonWriter);
            indexWarmers.WriteJson(elasticCrudJsonWriter);

            IndexSettingsCreatePropertyMappingForTopDocument(entityInfo, mappingDefinition, elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            CreateIndexCommand(elasticCrudJsonWriter.GetJsonString(), index);
        }

        public void UpdateSettings(string index, IndexUpdateSettings indexUpdateSettings)
        {
            var elasticCrudJsonWriter = new ElasticJsonWriter();
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("index");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            indexUpdateSettings.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            SettingsCommand(elasticCrudJsonWriter.GetJsonString(), index);
        }

        public void CreatePropertyMappingForTopDocument(EntityContextInfo entityInfo,
            MappingDefinition mappingDefinition)
        {
            var elasticSearchMapping =
                _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(entityInfo.EntityType);
            elasticSearchMapping.TraceProvider = _traceProvider;
            elasticSearchMapping.SaveChildObjectsAsWellAsParent =
                _elasticSerializerConfiguration.SaveChildObjectsAsWellAsParent;
            elasticSearchMapping.ProcessChildDocumentsAsSeparateChildIndex = _elasticSerializerConfiguration
                .ProcessChildDocumentsAsSeparateChildIndex;

            CreatePropertyMappingForEntityForParentDocument(entityInfo, elasticSearchMapping, mappingDefinition);

            if (_elasticSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
                if (elasticSearchMapping.ChildIndexEntities.Count > 0)
                {
                    // Only save the top level items now
                    elasticSearchMapping.SaveChildObjectsAsWellAsParent = false;
                    foreach (var item in elasticSearchMapping.ChildIndexEntities)
                        CreatePropertyMappingForChildDocument(entityInfo, elasticSearchMapping, item,
                            mappingDefinition);
                }

            elasticSearchMapping.ChildIndexEntities.Clear();
        }

        public void IndexSettingsCreatePropertyMappingForTopDocument(EntityContextInfo entityInfo,
            MappingDefinition mappingDefinition, ElasticJsonWriter elasticCrudJsonWriter)
        {
            var elasticSearchMapping =
                _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(entityInfo.EntityType);
            elasticSearchMapping.TraceProvider = _traceProvider;
            elasticSearchMapping.SaveChildObjectsAsWellAsParent =
                _elasticSerializerConfiguration.SaveChildObjectsAsWellAsParent;
            elasticSearchMapping.ProcessChildDocumentsAsSeparateChildIndex = _elasticSerializerConfiguration
                .ProcessChildDocumentsAsSeparateChildIndex;

            IndexCreateCreatePropertyMappingForEntityForParentDocument(entityInfo, elasticSearchMapping,
                mappingDefinition, elasticCrudJsonWriter);

            if (_elasticSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
                if (elasticSearchMapping.ChildIndexEntities.Count > 0)
                {
                    // Only save the top level items now
                    elasticSearchMapping.SaveChildObjectsAsWellAsParent = false;
                    foreach (var item in elasticSearchMapping.ChildIndexEntities)
                        IndexCreateCreatePropertyMappingForChildDocument(elasticCrudJsonWriter, entityInfo,
                            elasticSearchMapping, item, mappingDefinition);
                }

            elasticSearchMapping.ChildIndexEntities.Clear();
        }

        /// <summary>
        ///   Create a new index for the parent document 
        /// </summary>
        /// <param name="entityInfo">       </param>
        /// <param name="elasticMapping">   </param>
        /// <param name="mappingDefinition"> mapping definitions for the index type </param>
        private void IndexCreateCreatePropertyMappingForEntityForParentDocument(EntityContextInfo entityInfo,
            ElasticMapping elasticMapping, MappingDefinition mappingDefinition, ElasticJsonWriter elasticCrudJsonWriter)
        {
            var itemType = elasticMapping.GetDocumentType(entityInfo.EntityType);
            if (_processedItems.Contains("_mapping" + itemType))
                return;
            _processedItems.Add("_mapping" + itemType);

            //elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("mappings");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(itemType);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            mappingDefinition.Source.WriteJson(elasticCrudJsonWriter);
            mappingDefinition.All.WriteJson(elasticCrudJsonWriter);

            if (entityInfo.RoutingDefinition.RoutingId != null && _elasticSerializerConfiguration.UserDefinedRouting)
                CreateForceRoutingMappingForDocument(elasticCrudJsonWriter);

            if (entityInfo.RoutingDefinition.ParentId != null)
                CreateParentMappingForDocument(
                    elasticCrudJsonWriter,
                    elasticMapping.GetDocumentType(entityInfo.ParentEntityType));

            ProccessPropertyMappingsWithoutTypeName(elasticCrudJsonWriter, entityInfo, elasticMapping);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            if (_elasticSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
                if (elasticMapping.ChildIndexEntities.Count > 0)
                {
                    elasticMapping.SaveChildObjectsAsWellAsParent = false;
                    foreach (var item in elasticMapping.ChildIndexEntities)
                        IndexCreateCreatePropertyMappingForChildDocument(elasticCrudJsonWriter, entityInfo,
                            elasticMapping, item, mappingDefinition);
                }

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            //elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            //CreateMappingCommandForTypeWithExistingIndex(elasticCrudJsonWriter.GetJsonString(), mappingDefinition.Index, itemType);
        }

        /// <summary>
        ///   Create a new mapping for the child type in the parent document index 
        /// </summary>
        /// <param name="entityInfo">       </param>
        /// <param name="elasticMapping">   </param>
        /// <param name="item">             </param>
        /// <param name="mappingDefinition"> definition for the type mappings </param>
        private void IndexCreateCreatePropertyMappingForChildDocument(ElasticJsonWriter elasticCrudJsonWriter,
            EntityContextInfo entityInfo, ElasticMapping elasticMapping, EntityContextInfo item,
            MappingDefinition mappingDefinition)
        {
            var childMapping =
                _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(item.EntityType);

            var parentMapping =
                _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(item.ParentEntityType);

            var childType = childMapping.GetDocumentType(item.EntityType);
            var parentType = parentMapping.GetDocumentType(item.ParentEntityType);

            var processedId = childType + "_" + parentType;
            if (_processedItems.Contains(childType))
            {
                var test = CommandTypes.Find(t => t.StartsWith(childType));
                if (test != processedId)
                    throw new ElasticException("InitMappings: Not supported, child documents can only have one parent");
                return;
            }
            _processedItems.Add(childType);
            CommandTypes.Add(processedId);

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(childType);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            mappingDefinition.Source.WriteJson(elasticCrudJsonWriter);
            mappingDefinition.All.WriteJson(elasticCrudJsonWriter);

            CreateParentMappingForDocument(
                elasticCrudJsonWriter,
                elasticMapping.GetDocumentType(item.ParentEntityType));

            if (item.RoutingDefinition.RoutingId != null && _elasticSerializerConfiguration.UserDefinedRouting)
                CreateForceRoutingMappingForDocument(elasticCrudJsonWriter);

            ProccessPropertyMappingsWithoutTypeName(elasticCrudJsonWriter, item, childMapping);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            // CreateMappingCommandForTypeWithExistingIndex(elasticCrudJsonWriter.GetJsonString(),
            // elasticMapping.GetIndexForType(entityInfo.EntityType), childMapping.GetDocumentType(item.EntityType));
        }

        /// <summary>
        ///   Create a new index for the parent document 
        /// </summary>
        /// <param name="entityInfo">       </param>
        /// <param name="elasticMapping">   </param>
        /// <param name="mappingDefinition"> mapping definitions for the index type </param>
        private void CreatePropertyMappingForEntityForParentDocument(EntityContextInfo entityInfo,
            ElasticMapping elasticMapping, MappingDefinition mappingDefinition)
        {
            var itemType = elasticMapping.GetDocumentType(entityInfo.EntityType);
            if (_processedItems.Contains("_mapping" + itemType))
                return;
            _processedItems.Add("_mapping" + itemType);

            var elasticCrudJsonWriter = new ElasticJsonWriter();
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(itemType);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            mappingDefinition.Source.WriteJson(elasticCrudJsonWriter);
            mappingDefinition.All.WriteJson(elasticCrudJsonWriter);

            if (entityInfo.RoutingDefinition.RoutingId != null && _elasticSerializerConfiguration.UserDefinedRouting)
                CreateForceRoutingMappingForDocument(elasticCrudJsonWriter);

            if (entityInfo.RoutingDefinition.ParentId != null)
                CreateParentMappingForDocument(
                    elasticCrudJsonWriter,
                    elasticMapping.GetDocumentType(entityInfo.ParentEntityType));

            ProccessPropertyMappingsWithoutTypeName(elasticCrudJsonWriter, entityInfo, elasticMapping);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            CreateMappingCommandForTypeWithExistingIndex(elasticCrudJsonWriter.GetJsonString(), mappingDefinition.Index,
                itemType);
        }

        /// <summary>
        ///   Create a new mapping for the child type in the parent document index 
        /// </summary>
        /// <param name="entityInfo">       </param>
        /// <param name="elasticMapping">   </param>
        /// <param name="item">             </param>
        /// <param name="mappingDefinition"> definition for the type mappings </param>
        private void CreatePropertyMappingForChildDocument(EntityContextInfo entityInfo, ElasticMapping elasticMapping,
            EntityContextInfo item, MappingDefinition mappingDefinition)
        {
            var childMapping =
                _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(item.EntityType);

            var parentMapping =
                _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(item.ParentEntityType);

            var childType = childMapping.GetDocumentType(item.EntityType);
            var parentType = parentMapping.GetDocumentType(item.ParentEntityType);

            var processedId = childType + "_" + parentType;
            if (_processedItems.Contains(childType))
            {
                var test = CommandTypes.Find(t => t.StartsWith(childType));
                if (test != processedId)
                    throw new ElasticException("InitMappings: Not supported, child documents can only have one parent");
                return;
            }
            _processedItems.Add(childType);
            CommandTypes.Add(processedId);

            var elasticCrudJsonWriter = new ElasticJsonWriter();
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(childType);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            mappingDefinition.Source.WriteJson(elasticCrudJsonWriter);
            mappingDefinition.All.WriteJson(elasticCrudJsonWriter);

            CreateParentMappingForDocument(
                elasticCrudJsonWriter,
                elasticMapping.GetDocumentType(item.ParentEntityType));

            if (item.RoutingDefinition.RoutingId != null && _elasticSerializerConfiguration.UserDefinedRouting)
                CreateForceRoutingMappingForDocument(elasticCrudJsonWriter);

            ProccessPropertyMappingsWithoutTypeName(elasticCrudJsonWriter, item, childMapping);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            CreateMappingCommandForTypeWithExistingIndex(elasticCrudJsonWriter.GetJsonString(),
                elasticMapping.GetIndexForType(entityInfo.EntityType), childMapping.GetDocumentType(item.EntityType));
        }

        private void ProccessPropertyMappingsWithoutTypeName(ElasticJsonWriter elasticCrudJsonWriter,
            EntityContextInfo entityInfo, ElasticMapping elasticMapping)
        {
            //"properties": {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("properties");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticMapping.MapEntityValues(entityInfo, elasticCrudJsonWriter, true, CreatePropertyMappings);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }

        private void CreateMappingCommandForTypeWithExistingIndex(string propertyMapping, string index,
            string documentType)
        {
            var command = new MappingCommand
            {
                Url = string.Format("/{0}/{1}/_mappings", index, documentType),
                RequestType = "PUT",
                Content = propertyMapping
            };
            //Console.WriteLine("XXXCreateMappingCommandForTypeWithExistingIndex: " + index + ": " + documentType);
            Commands.Add(command);
        }

        private void SettingsCommand(string indexJsonConfiguration, string index)
        {
            var command = new MappingCommand
            {
                Url = string.Format("/{0}/_settings", index),
                RequestType = "PUT",
                Content = indexJsonConfiguration
            };
            //Console.WriteLine("XXXSettingsCommand: " + index);
            Commands.Add(command);
        }

        private void CreateIndexCommand(string indexJsonConfiguration, string index)
        {
            var command = new MappingCommand
            {
                Url = string.Format("/{0}", index),
                RequestType = "PUT",
                Content = indexJsonConfiguration
            };
            //Console.WriteLine("XXXCreateIndexCommand: " + index);
            Commands.Add(command);
        }

        private void CreateParentMappingForDocument(ElasticJsonWriter elasticCrudJsonWriter, string parentType)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("_parent");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("type");
            elasticCrudJsonWriter.JsonWriter.WriteValue(parentType);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }

        /// <summary>
        ///   "_routing": { "required": true }, 
        /// </summary>
        /// <param name="elasticCrudJsonWriter"></param>
        private void CreateForceRoutingMappingForDocument(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("_routing");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("required");
            elasticCrudJsonWriter.JsonWriter.WriteValue("true");
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }

        /// "settings" : { "number_of_shards" : 1 },
        private void CreateIndexSettings(ElasticJsonWriter elasticCrudJsonWriter, IndexSettings indexSettings)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("settings");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            indexSettings.WriteJson(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}