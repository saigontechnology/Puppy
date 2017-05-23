using System;
using System.Linq;
using System.Reflection;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    public class Fields
    {
        /// <summary>
        ///     You can define all the Elastic properties here 
        /// </summary>
        public Type FieldClass { get; set; }

        public void AddFieldData(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("fields");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            var propertyInfo = FieldClass.GetProperties();
            foreach (var property in propertyInfo)
                //#if NET46 || NET451 || NET452
                //                if (Attribute.IsDefined(property, typeof(ElasticCoreTypes)))
                //                {
                //                    var propertyName = property.Name.ToLower();
                //                    object[] attrs = property.GetCustomAttributes(typeof(ElasticCoreTypes), true);

                //                    if ((attrs[0] as ElasticCoreTypes) != null)
                //                    {
                //                        elasticCrudJsonWriter.JsonWriter.WritePropertyName(propertyName);
                //                        elasticCrudJsonWriter.JsonWriter.WriteRawValue((attrs[0] as ElasticCoreTypes).JsonString());
                //                    }
                //                }
                //#else
                if (property.GetCustomAttribute(typeof(ElasticCoreTypes)) != null)
                {
                    var propertyName = property.Name.ToLower();

                    var attrs = property.GetCustomAttributes(typeof(ElasticCoreTypes), true);

                    if (attrs.FirstOrDefault() as ElasticCoreTypes != null)
                    {
                        elasticCrudJsonWriter.JsonWriter.WritePropertyName(propertyName);
                        elasticCrudJsonWriter.JsonWriter.WriteRawValue((attrs.FirstOrDefault() as ElasticCoreTypes)
                            .JsonString());
                    }
                }
            //#endif

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}