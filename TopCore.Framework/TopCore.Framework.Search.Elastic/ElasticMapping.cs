using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Model.GeoModel;
using TopCore.Framework.Search.Elastic.Tracing;

namespace TopCore.Framework.Search.Elastic
{
    /// <summary>
    ///     Default mapping for your Entity. You can implement this clas to implement your specific mapping if required
    ///     Everything is lowercase and the index is pluralized
    /// </summary>
    public class ElasticMapping
    {
        public List<EntityContextInfo> ChildIndexEntities = new List<EntityContextInfo>();
        protected HashSet<string> SerializedTypes = new HashSet<string>();
        public ITraceProvider TraceProvider = new NullTraceProvider();
        public bool SaveChildObjectsAsWellAsParent { get; set; }
        public bool ProcessChildDocumentsAsSeparateChildIndex { get; set; }

        /// <summary>
        ///     Ovveride this if your default mapping needs to be changed. default type is lowercase for properties, indes
        ///     pluralized and type to lower
        /// </summary>
        /// <param name="entityInfo">             Information about the entity </param>
        /// <param name="elasticCrudJsonWriter">  Serializer with added tracing </param>
        /// <param name="beginMappingTree">       begin new mapping tree </param>
        /// <param name="createPropertyMappings">
        ///     This tells the serializer to create a Json property mapping from the entity and
        ///     not the document itself
        /// </param>
        public virtual void MapEntityValues(EntityContextInfo entityInfo, ElasticJsonWriter elasticCrudJsonWriter,
            bool beginMappingTree = false, bool createPropertyMappings = false)
        {
            try
            {
                BeginNewEntityToDocumentMapping(entityInfo, beginMappingTree);

                TraceProvider.Trace(TraceEventType.Verbose, "ElasticMapping: SerializedTypes new Type added: {0}",
                    GetDocumentType(entityInfo.Document.GetType()));
                var propertyInfo = entityInfo.Document.GetType().GetProperties();
                foreach (var prop in propertyInfo)
                    if (prop.GetCustomAttribute(typeof(JsonIgnoreAttribute)) == null)
                        if (prop.GetCustomAttribute(typeof(ElasticGeoTypeAttribute)) != null)
                        {
                            var obj = prop.Name.ToLower();
                            // process GeoTypes
                            if (createPropertyMappings)
                            {
                                //#if NET46 || NET451 || NET452
                                //                                object[] attrs = prop.GetCustomAttributes(typeof(ElasticCoreTypes), true);

                                //                                if ((attrs.FirstOrDefault() as ElasticCoreTypes) != null)
                                //                                {
                                //                                    elasticCrudJsonWriter.JsonWriter.WritePropertyName(obj);
                                //                                    elasticCrudJsonWriter.JsonWriter.WriteRawValue((attrs.FirstOrDefault() as ElasticCoreTypes).JsonString());
                                //                                }
                                //#else
                                var attrs = prop.GetCustomAttributes(typeof(ElasticCoreTypes), true);

                                if (attrs.FirstOrDefault() as ElasticCoreTypes != null)
                                {
                                    elasticCrudJsonWriter.JsonWriter.WritePropertyName(obj);
                                    elasticCrudJsonWriter.JsonWriter.WriteRawValue(
                                        (attrs.FirstOrDefault() as ElasticCoreTypes).JsonString());
                                }
                                //#endif
                            }
                            else
                            {
                                var data = prop.GetValue(entityInfo.Document) as IGeoType;
                                elasticCrudJsonWriter.JsonWriter.WritePropertyName(obj);
                                data.WriteJson(elasticCrudJsonWriter);
                                // Write data
                            }
                        }
                        else if (IsPropertyACollection(prop))
                        {
                            ProcessArrayOrCollection(entityInfo, elasticCrudJsonWriter, prop, createPropertyMappings);
                        }
                        else
                        {
                            if (prop.PropertyType.GetTypeInfo().IsClass &&
                                prop.PropertyType.FullName != "System.String" &&
                                prop.PropertyType.FullName != "System.Decimal")
                            {
                                ProcessSingleObject(entityInfo, elasticCrudJsonWriter, prop, createPropertyMappings);
                            }
                            else
                            {
                                if (!ProcessChildDocumentsAsSeparateChildIndex ||
                                    ProcessChildDocumentsAsSeparateChildIndex && beginMappingTree)
                                {
                                    TraceProvider.Trace(TraceEventType.Verbose,
                                        "ElasticMapping: Property is a simple Type: {0}, {1}", prop.Name.ToLower(),
                                        prop.PropertyType.FullName);

                                    if (createPropertyMappings)
                                    {
                                        var obj = prop.Name.ToLower();
                                        if (prop.GetCustomAttribute(typeof(ElasticCoreTypes)) != null)
                                        {
                                            //#if NET46 || NET451 || NET452
                                            //                                            object[] attrs = prop.GetCustomAttributes(typeof(ElasticCoreTypes), true);

                                            //                                            if ((attrs.FirstOrDefault() as ElasticCoreTypes) != null)
                                            //                                            {
                                            //                                                elasticCrudJsonWriter.JsonWriter.WritePropertyName(obj);
                                            //                                                elasticCrudJsonWriter.JsonWriter.WriteRawValue((attrs.FirstOrDefault() as ElasticCoreTypes).JsonString());
                                            //                                            }
                                            //#else
                                            var attrs = prop.GetCustomAttributes(typeof(ElasticCoreTypes), true);

                                            if (attrs.FirstOrDefault() as ElasticCoreTypes != null)
                                            {
                                                elasticCrudJsonWriter.JsonWriter.WritePropertyName(obj);
                                                elasticCrudJsonWriter.JsonWriter.WriteRawValue(
                                                    (attrs.FirstOrDefault() as ElasticCoreTypes).JsonString());
                                            }
                                            //#endif
                                        }
                                        else
                                        {
                                            // no elastic property defined
                                            elasticCrudJsonWriter.JsonWriter.WritePropertyName(obj);
                                            if (prop.PropertyType.FullName == "System.DateTime" ||
                                                prop.PropertyType.FullName == "System.DateTimeOffset")
                                                elasticCrudJsonWriter.JsonWriter.WriteRawValue(
                                                    "{ \"type\" : \"date\", \"format\": \"dateOptionalTime\"}");
                                            else
                                                elasticCrudJsonWriter.JsonWriter.WriteRawValue(
                                                    "{ \"type\" : \"" + GetElasticType(prop.PropertyType) + "\" }");
                                        }
                                    }
                                    else
                                    {
                                        MapValue(prop.Name.ToLower(), prop.GetValue(entityInfo.Document),
                                            elasticCrudJsonWriter.JsonWriter);
                                    }
                                }
                            }
                        }
            }
            catch (Exception ex)
            {
                TraceProvider.Trace(TraceEventType.Critical, ex, "ElasticMapping: Property is a simple Type: {0}",
                    elasticCrudJsonWriter.GetJsonString());
                throw;
            }
        }

        private void BeginNewEntityToDocumentMapping(EntityContextInfo entityInfo, bool beginMappingTree)
        {
            if (beginMappingTree)
            {
                SerializedTypes = new HashSet<string>();
                TraceProvider.Trace(TraceEventType.Verbose, "ElasticMapping: Serialize BEGIN for Type: {0}",
                    entityInfo.Document.GetType());
            }
        }

        private void ProcessSingleObject(EntityContextInfo entityInfo, ElasticJsonWriter elasticCrudJsonWriter,
            PropertyInfo prop, bool createPropertyMappings)
        {
            TraceProvider.Trace(TraceEventType.Verbose, "ElasticMapping: Property is an Object: {0}", prop.ToString());
            // This is a single object and not a reference to it's parent

            if (createPropertyMappings && prop.GetValue(entityInfo.Document) == null)
                prop.SetValue(entityInfo.Document, Activator.CreateInstance(prop.PropertyType));
            if (prop.GetValue(entityInfo.Document) != null && SaveChildObjectsAsWellAsParent)
            {
                var child = GetDocumentType(prop.GetValue(entityInfo.Document).GetType());
                var parent = GetDocumentType(entityInfo.EntityType);
                if (!SerializedTypes.Contains(child + parent))
                {
                    SerializedTypes.Add(parent + child);
                    if (ProcessChildDocumentsAsSeparateChildIndex)
                        ProcessSingleObjectAsChildDocument(entityInfo, elasticCrudJsonWriter, prop,
                            createPropertyMappings);
                    else
                        ProcessSingleObjectAsNestedObject(entityInfo, elasticCrudJsonWriter, prop,
                            createPropertyMappings);
                }
            }
        }

        private void ProcessArrayOrCollection(EntityContextInfo entityInfo, ElasticJsonWriter elasticCrudJsonWriter,
            PropertyInfo prop, bool createPropertyMappings)
        {
            TraceProvider.Trace(TraceEventType.Verbose, "ElasticMapping: IsPropertyACollection: {0}",
                prop.Name.ToLower());

            if (createPropertyMappings && prop.GetValue(entityInfo.Document) == null)
                if (prop.PropertyType.IsArray)
                    prop.SetValue(entityInfo.Document, Array.CreateInstance(prop.PropertyType.GetElementType(), 0));
                else
                    prop.SetValue(entityInfo.Document, Activator.CreateInstance(prop.PropertyType));

            if (prop.GetValue(entityInfo.Document) != null && SaveChildObjectsAsWellAsParent)
                if (ProcessChildDocumentsAsSeparateChildIndex)
                    ProcessArrayOrCollectionAsChildDocument(entityInfo, elasticCrudJsonWriter, prop,
                        createPropertyMappings);
                else
                    ProcessArrayOrCollectionAsNestedObject(entityInfo, elasticCrudJsonWriter, prop,
                        createPropertyMappings);
        }

        private void ProcessSingleObjectAsNestedObject(EntityContextInfo entityInfo,
            ElasticJsonWriter elasticCrudJsonWriter, PropertyInfo prop, bool createPropertyMappings)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(prop.Name.ToLower());
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            if (createPropertyMappings)
            {
                // "properties": {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("properties");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            }
            // Do class mapping for nested type
            var entity = prop.GetValue(entityInfo.Document);
            var routingDefinition = new RoutingDefinition {ParentId = entityInfo.Id};
            var child = new EntityContextInfo
            {
                Document = entity,
                RoutingDefinition = routingDefinition,
                EntityType = entity.GetType(),
                DeleteDocument = entityInfo.DeleteDocument
            };

            MapEntityValues(child, elasticCrudJsonWriter, false, createPropertyMappings);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            if (createPropertyMappings)
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }

        private void ProcessSingleObjectAsChildDocument(EntityContextInfo entityInfo,
            ElasticJsonWriter elasticCrudJsonWriter, PropertyInfo prop, bool createPropertyMappings)
        {
            var entity = prop.GetValue(entityInfo.Document);
            CreateChildEntityForDocumentIndex(entityInfo, elasticCrudJsonWriter, entity, createPropertyMappings);
        }

        private void CreateChildEntityForDocumentIndex(EntityContextInfo parentEntityInfo,
            ElasticJsonWriter elasticCrudJsonWriter, object entity, bool createPropertyMappings)
        {
            var propertyInfo = entity.GetType().GetProperties();
            foreach (var property in propertyInfo)
                //#if NET46 || NET451 || NET452
                //                // TODO support this property.GetCustomAttribute(typeof(KeyAttribute)) != null ||
                //                if ( property.GetCustomAttribute(typeof(ElasticIdAttribute)) != null)
                //                {
                //#else
                if (property.GetCustomAttribute(typeof(KeyAttribute)) != null ||
                    property.GetCustomAttribute(typeof(ElasticIdAttribute)) != null)
                {
                    //#endif
                    var obj = property.GetValue(entity);

                    if (obj == null && createPropertyMappings)
                        obj = "0";

                    RoutingDefinition routingDefinition;
                    if (parentEntityInfo.RoutingDefinition.RoutingId != null)
                        routingDefinition =
                            new RoutingDefinition
                            {
                                ParentId = parentEntityInfo.Id,
                                RoutingId = parentEntityInfo.RoutingDefinition.RoutingId
                            };
                    else
                        routingDefinition =
                            new RoutingDefinition {ParentId = parentEntityInfo.Id, RoutingId = parentEntityInfo.Id};

                    var child = new EntityContextInfo
                    {
                        Document = entity,
                        RoutingDefinition = routingDefinition,
                        EntityType = GetEntityDocumentType(entity.GetType()),
                        ParentEntityType = GetEntityDocumentType(parentEntityInfo.EntityType),
                        DeleteDocument = parentEntityInfo.DeleteDocument,
                        Id = obj.ToString()
                    };
                    ChildIndexEntities.Add(child);
                    MapEntityValues(child, elasticCrudJsonWriter, false, createPropertyMappings);

                    return;
                }

            throw new ElasticException("No Key found for child object: " + parentEntityInfo.Document.GetType());
        }

        private void ProcessArrayOrCollectionAsNestedObject(EntityContextInfo entityInfo,
            ElasticJsonWriter elasticCrudJsonWriter, PropertyInfo prop, bool createPropertyMappings)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(prop.Name.ToLower());
            TraceProvider.Trace(TraceEventType.Verbose, "ElasticMapping: BEGIN ARRAY or COLLECTION: {0} {1}",
                prop.Name.ToLower(), elasticCrudJsonWriter.JsonWriter.Path);
            var typeOfEntity = prop.GetValue(entityInfo.Document).GetType().GetGenericArguments();
            if (typeOfEntity.Length > 0)
            {
                var child = GetDocumentType(typeOfEntity[0]);
                var parent = GetDocumentType(entityInfo.EntityType);

                if (!SerializedTypes.Contains(child + parent))
                {
                    SerializedTypes.Add(parent + child);
                    TraceProvider.Trace(TraceEventType.Verbose,
                        "ElasticMapping: SerializedTypes type ok, BEGIN ARRAY or COLLECTION: {0}", typeOfEntity[0]);
                    TraceProvider.Trace(TraceEventType.Verbose, "ElasticMapping: SerializedTypes new Type added: {0}",
                        GetDocumentType(typeOfEntity[0]));
                    MapCollectionOrArray(prop, entityInfo, elasticCrudJsonWriter, createPropertyMappings);
                }
                else
                {
                    elasticCrudJsonWriter.JsonWriter.WriteRawValue("null");
                }
            }
            else
            {
                TraceProvider.Trace(TraceEventType.Verbose,
                    "ElasticMapping: BEGIN ARRAY or COLLECTION NOT A GENERIC: {0}",
                    prop.Name.ToLower());
                // Not a generic
                MapCollectionOrArray(prop, entityInfo, elasticCrudJsonWriter, createPropertyMappings);
            }
        }

        private void ProcessArrayOrCollectionAsChildDocument(EntityContextInfo entityInfo,
            ElasticJsonWriter elasticCrudJsonWriter, PropertyInfo prop, bool createPropertyMappings)
        {
            TraceProvider.Trace(TraceEventType.Verbose, "ElasticMapping: BEGIN ARRAY or COLLECTION: {0} {1}",
                prop.Name.ToLower(), elasticCrudJsonWriter.JsonWriter.Path);
            var typeOfEntity = prop.GetValue(entityInfo.Document).GetType().GetGenericArguments();
            if (typeOfEntity.Length > 0)
            {
                var child = GetDocumentType(typeOfEntity[0]);
                var parent = GetDocumentType(entityInfo.EntityType);

                if (!SerializedTypes.Contains(child + parent))
                {
                    SerializedTypes.Add(parent + child);
                    TraceProvider.Trace(TraceEventType.Verbose,
                        "ElasticMapping: SerializedTypes type ok, BEGIN ARRAY or COLLECTION: {0}", typeOfEntity[0]);
                    TraceProvider.Trace(TraceEventType.Verbose, "ElasticMapping: SerializedTypes new Type added: {0}",
                        GetDocumentType(typeOfEntity[0]));

                    MapCollectionOrArray(prop, entityInfo, elasticCrudJsonWriter, createPropertyMappings);
                }
            }
            else
            {
                TraceProvider.Trace(TraceEventType.Verbose,
                    "ElasticMapping: BEGIN ARRAY or COLLECTION NOT A GENERIC: {0}",
                    prop.Name.ToLower());
                // Not a generic
                MapCollectionOrArray(prop, entityInfo, elasticCrudJsonWriter, createPropertyMappings);
            }
        }

        // Nested
        // "tags" : ["elastic", "wow"], (string array or int array)
        // Nested
        //"lists" : [
        //	{
        //		"name" : "prog_list",
        //		"description" : "programming list"
        //	},
        protected virtual void MapCollectionOrArray(PropertyInfo prop, EntityContextInfo entityInfo,
            ElasticJsonWriter elasticCrudJsonWriter, bool createPropertyMappings)
        {
            var type = prop.PropertyType;

            if (type.HasElementType)
            {
                // It is a collection
                var ienumerable = (Array) prop.GetValue(entityInfo.Document);
                if (ProcessChildDocumentsAsSeparateChildIndex)
                {
                    MapIEnumerableEntitiesForChildIndexes(elasticCrudJsonWriter, ienumerable, entityInfo, prop,
                        createPropertyMappings);
                }
                else
                {
                    if (createPropertyMappings)
                        MapIEnumerableEntitiesForMapping(elasticCrudJsonWriter, entityInfo, prop, true);
                    else
                        MapIEnumerableEntities(elasticCrudJsonWriter, ienumerable, entityInfo, false);
                }
            }
            else if (prop.PropertyType.GetTypeInfo().IsGenericType)
            {
                // It is a collection
                var ienumerable = (IEnumerable) prop.GetValue(entityInfo.Document);

                if (ProcessChildDocumentsAsSeparateChildIndex)
                {
                    MapIEnumerableEntitiesForChildIndexes(elasticCrudJsonWriter, ienumerable, entityInfo, prop,
                        createPropertyMappings);
                }
                else
                {
                    if (createPropertyMappings)
                        MapIEnumerableEntitiesForMapping(elasticCrudJsonWriter, entityInfo, prop, true);
                    else
                        MapIEnumerableEntities(elasticCrudJsonWriter, ienumerable, entityInfo, false);
                }
            }
        }

        private void MapIEnumerableEntitiesForChildIndexes(ElasticJsonWriter elasticCrudJsonWriter,
            IEnumerable ienumerable, EntityContextInfo parentEntityInfo, PropertyInfo prop, bool createPropertyMappings)
        {
            if (createPropertyMappings)
            {
                object item;
                if (prop.PropertyType.GenericTypeArguments.Length == 0)
                    item = Activator.CreateInstance(prop.PropertyType.GetElementType());
                else
                    item = Activator.CreateInstance(prop.PropertyType.GenericTypeArguments[0]);

                CreateChildEntityForDocumentIndex(parentEntityInfo, elasticCrudJsonWriter, item, true);
            }
            else
            {
                if (ienumerable != null)
                    foreach (var item in ienumerable)
                        CreateChildEntityForDocumentIndex(parentEntityInfo, elasticCrudJsonWriter, item, false);
            }
        }

        private void MapIEnumerableEntitiesForMapping(ElasticJsonWriter elasticCrudJsonWriter,
            EntityContextInfo parentEntityInfo, PropertyInfo prop, bool createPropertyMappings)
        {
            object item;
            if (prop.PropertyType.FullName == "System.String[]")
                item = string.Empty;
            else if (prop.PropertyType.GenericTypeArguments.Length == 0)
                item = Activator.CreateInstance(prop.PropertyType.GetElementType());
            else if (prop.PropertyType.GenericTypeArguments[0].FullName == "System.String")
                item = string.Empty;
            else
                item = Activator.CreateInstance(prop.PropertyType.GenericTypeArguments[0]);

            var typeofArrayItem = item.GetType();
            if (typeofArrayItem.GetTypeInfo().IsClass && typeofArrayItem.FullName != "System.String" &&
                typeofArrayItem.FullName != "System.Decimal")
            {
                // collection of Objects
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                //#if NET46 || NET451 || NET452
                //                if (prop.GetCustomAttribute(typeof(ElasticCoreTypes)) != null)
                //                {
                //                    var propertyName = prop.Name.ToLower();

                // object[] attrs = prop.GetCustomAttributes(typeof(ElasticCoreTypes), true);

                //                    if ((attrs.FirstOrDefault() as ElasticCoreTypes) != null)
                //                    {
                //                        elasticCrudJsonWriter.JsonWriter.WritePropertyName(propertyName);
                //                        elasticCrudJsonWriter.JsonWriter.WriteRawValue((attrs.FirstOrDefault() as ElasticCoreTypes).JsonString());
                //                    }
                //                }
                //#else
                if (prop.GetCustomAttribute(typeof(ElasticNestedAttribute)) != null)
                {
                    elasticCrudJsonWriter.JsonWriter.WritePropertyName("type");
                    elasticCrudJsonWriter.JsonWriter.WriteValue("nested");

                    var attrs = prop.GetCustomAttributes(typeof(ElasticNestedAttribute), true);

                    if (attrs.FirstOrDefault() as ElasticNestedAttribute != null)
                        (attrs.FirstOrDefault() as ElasticNestedAttribute).WriteJson(elasticCrudJsonWriter);
                }
                //#endif
                // "properties": {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("properties");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                // Do class mapping for nested type
                var routingDefinition = new RoutingDefinition
                {
                    ParentId = parentEntityInfo.Id,
                    RoutingId = parentEntityInfo.RoutingDefinition.RoutingId
                };
                var child = new EntityContextInfo
                {
                    Document = item,
                    RoutingDefinition = routingDefinition,
                    EntityType = item.GetType(),
                    DeleteDocument = parentEntityInfo.DeleteDocument
                };
                MapEntityValues(child, elasticCrudJsonWriter, false, createPropertyMappings);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
            else
            {
                // {"type": "ienumerable"} collection of simple types
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("type");
                elasticCrudJsonWriter.JsonWriter.WriteValue(GetElasticType(item.GetType()));
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
        }

        private void MapIEnumerableEntities(ElasticJsonWriter elasticCrudJsonWriter, IEnumerable ienumerable,
            EntityContextInfo parentEntityInfo, bool createPropertyMappings)
        {
            string json = null;
            var isSimpleArrayOrCollection = true;
            var doProccessingIfTheIEnumerableHasAtLeastOneItem = false;
            if (ienumerable != null)
            {
                var sbCollection = new StringBuilder();
                sbCollection.Append("[");
                foreach (var item in ienumerable)
                {
                    doProccessingIfTheIEnumerableHasAtLeastOneItem = true;

                    var childElasticJsonWriter = new ElasticJsonWriter(sbCollection);
                    elasticCrudJsonWriter.ElasticJsonWriterChildItem = childElasticJsonWriter;

                    var typeofArrayItem = item.GetType();
                    if (typeofArrayItem.GetTypeInfo().IsClass && typeofArrayItem.FullName != "System.String" &&
                        typeofArrayItem.FullName != "System.Decimal")
                    {
                        isSimpleArrayOrCollection = false;
                        // collection of Objects
                        childElasticJsonWriter.JsonWriter.WriteStartObject();
                        // Do class mapping for nested type
                        var routingDefinition =
                            new RoutingDefinition
                            {
                                ParentId = parentEntityInfo.Id,
                                RoutingId = parentEntityInfo.RoutingDefinition.RoutingId
                            };
                        var child = new EntityContextInfo
                        {
                            Document = item,
                            RoutingDefinition = routingDefinition,
                            EntityType = item.GetType(),
                            DeleteDocument = parentEntityInfo.DeleteDocument
                        };
                        MapEntityValues(child, childElasticJsonWriter, false, createPropertyMappings);
                        childElasticJsonWriter.JsonWriter.WriteEndObject();

                        // Add as separate document later
                    }
                    else
                    {
                        // collection of simple types, serialize all items in one go and break from the loop
                        json = JsonConvert.SerializeObject(ienumerable);

                        break;
                    }
                    sbCollection.Append(",");
                }

                if (isSimpleArrayOrCollection && doProccessingIfTheIEnumerableHasAtLeastOneItem)
                {
                    elasticCrudJsonWriter.JsonWriter.WriteRawValue(json);
                }
                else
                {
                    if (doProccessingIfTheIEnumerableHasAtLeastOneItem)

                        sbCollection.Remove(sbCollection.Length - 1, 1);

                    sbCollection.Append("]");
                    elasticCrudJsonWriter.JsonWriter.WriteRawValue(sbCollection.ToString());
                }
            }
            else
            {
                elasticCrudJsonWriter.JsonWriter.WriteRawValue("");
            }
        }

        protected void MapValue(string key, object valueObj, JsonWriter writer)
        {
            writer.WritePropertyName(key);
            writer.WriteValue(valueObj);
        }

        protected bool IsPropertyACollection(PropertyInfo property)
        {
            if (property.PropertyType.FullName == "System.String" || property.PropertyType.FullName == "System.Decimal")
                return false;

            if (property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)) ||
                property.PropertyType.GetInterfaces().Contains(typeof(ICollection)) ||
                property.PropertyType.GetInterfaces().Contains(typeof(IList)))
                return true;

            return false;
        }

        public virtual object ParseEntity(JToken source, Type type)
        {
            return JsonConvert.DeserializeObject(
                source.ToString(),
                type
            );
        }

        /// <summary>
        ///     Override this if you require a special type definition for your document type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns> The type used in Elastic for this type </returns>
        public virtual string GetDocumentType(Type type)
        {
            // Adding support for EF types
            if (type.GetTypeInfo().BaseType != null && type.Namespace == "System.Data.Entity.DynamicProxies")
                type = type.GetTypeInfo().BaseType;
            return type.Name.ToLower();
        }

        public virtual Type GetEntityDocumentType(Type type)
        {
            // Adding support for EF types
            if (type.GetTypeInfo().BaseType != null && type.Namespace == "System.Data.Entity.DynamicProxies")
                type = type.GetTypeInfo().BaseType;
            return type;
        }

        /// <summary>
        ///     Overide this if you need to define the index for your document. Required if your using a child document type.
        ///     Default: pluralize the default type
        /// </summary>
        /// <param name="type"> Type of class used </param>
        /// <returns> The index used in Elastic for this type </returns>
        public virtual string GetIndexForType(Type type)
        {
            // Adding support for EF types
            if (type.GetTypeInfo().BaseType != null && type.Namespace == "System.Data.Entity.DynamicProxies")
                type = type.GetTypeInfo().BaseType;
            return type.Name.ToLower() + "s";
        }

        /// <summary>
        ///     bool System.Boolean byte System.Byte sbyte System.SByte char System.Char decimal System.Decimal =&gt; string double
        ///     System.Double float System.Single int System.Int32 uint System.UInt32 long System.Int64 ulong System.UInt64 short
        ///     System.Int16 ushort System.UInt16 string System.String
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns> string, boolean, and null. float, double, byte, short, integer, and long date binary </returns>
        public string GetElasticType(Type propertyType)
        {
            switch (propertyType.FullName)
            {
                case "System.Boolean":
                    return "boolean";

                case "System.Byte":
                    return "byte";

                case "System.SByte":
                    return "byte";

                case "System.Double":
                    return "double";

                case "System.Single":
                    return "float";

                case "System.Int32":
                    return "integer";

                case "System.UInt32":
                    return "integer";

                case "System.Int64":
                    return "long";

                case "System.UInt64":
                    return "long";

                case "System.Int16":
                    return "short";

                case "System.UInt16":
                    return "short";

                default:
                    return "string";
            }
        }
    }
}