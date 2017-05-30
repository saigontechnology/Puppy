#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> NewtonsoftJsonSerializer.cs </Name>
//         <Created> 30/05/2017 4:55:34 PM </Created>
//         <Key> d223986b-2925-4c58-afe4-e87c72caeeef </Key>
//     </File>
//     <Summary>
//         NewtonsoftJsonSerializer.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using RestSharp.Serializers;
using System.IO;

namespace Puppy.OneSignal
{
    /// <summary>
    ///     Custom implementation to Json serializer in order to comply with REST Sharp requirements. 
    /// </summary>
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        /// <summary>
        ///     Content type. 
        /// </summary>
        public string ContentType
        {
            get { return "application/json"; }
            set { }
        }

        /// <summary>
        ///     Date format. 
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        ///     Namespace. 
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///     Root element. 
        /// </summary>
        public string RootElement { get; set; }

        /// <summary>
        ///     Serializes object. 
        /// </summary>
        /// <param name="obj"> Object to serialize. </param>
        /// <returns></returns>
        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';

                    _serializer.Serialize(jsonTextWriter, obj);

                    var result = stringWriter.ToString();
                    return result;
                }
            }
        }

        /// <summary>
        ///     Default constructor that prevents null values to be serialized. 
        /// </summary>
        public NewtonsoftJsonSerializer()
        {
            _serializer = new Newtonsoft.Json.JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include
            };
        }
    }
}