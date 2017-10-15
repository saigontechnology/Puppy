#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ObjectToArrayConverter.cs </Name>
//         <Created> 13/10/17 2:19:06 PM </Created>
//         <Key> 62424e66-a4c9-4c25-b17f-5c5b4e3a4039 </Key>
//     </File>
//     <Summary>
//         ObjectToArrayConverter.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace Puppy.Core.ObjectUtils
{
    public class ObjectToArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(T) == objectType;
        }

        private static bool ShouldSkip(JsonProperty property)
        {
            return property.Ignored || !property.Readable || !property.Writable;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = value.GetType();

            if (!(serializer.ContractResolver.ResolveContract(type) is JsonObjectContract contract))
            {
                throw new JsonSerializationException("invalid type " + type.FullName);
            }

            var list = contract.Properties.Where(p => !ShouldSkip(p)).Select(p => p.ValueProvider.GetValue(value));

            serializer.Serialize(writer, list);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var token = JToken.Load(reader);

            if (token.Type != JTokenType.Array)
            {
                throw new JsonSerializationException("token was not an array");
            }

            if (!(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract contract))
            {
                throw new JsonSerializationException("invalid type " + objectType.FullName);
            }

            var value = existingValue ?? contract.DefaultCreator();

            foreach (var pair in contract.Properties.Where(p => !ShouldSkip(p)).Zip(token, (p, v) => new { Value = v, Property = p }))
            {
                var propertyValue = pair.Value.ToObject(pair.Property.PropertyType, serializer);

                pair.Property.ValueProvider.SetValue(value, propertyValue);
            }

            return value;
        }
    }
}