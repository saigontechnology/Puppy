#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SwaggerConfig.cs </Name>
//         <Created> 08/08/17 11:58:05 AM </Created>
//         <Key> d64dfc55-3429-4f25-9f20-2714e39972b8 </Key>
//     </File>
//     <Summary>
//         SwaggerConfig.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using Puppy.Core.StringUtils;
using Puppy.Swagger.Models;

namespace Puppy.Swagger
{
    /// <summary>
    ///     Swagger Config 
    /// </summary>
    public static class SwaggerConfig
    {
        /// <summary>
        ///     Api Document Title. Ex: Puppy API 
        /// </summary>
        public static string ApiDocumentHtmlTitle { get; set; } = "API Document";

        /// <summary>
        ///     Url of Api Document 
        /// </summary>
        /// <remarks> Start and End with <c> "/" </c>. Ex: "/.well-known/api-configuration/" </remarks>
        public static string ApiDocumentUrl { get; set; } = "/.well-known/api-configuration/";

        /// <summary>
        ///     Api Document Name/Version. Ex: latest, v1, v2 and so on. 
        /// </summary>
        public static string ApiDocumentName { get; set; } = "latest";

        /// <summary>
        ///     Api Document Json File Name. Ex: all => all.json
        /// </summary>
        public static string ApiDocumentJsonFile { get; set; } = "all";

        /// <summary>
        ///     Access Key read from URI 
        /// </summary>
        /// <remarks> Empty is allow <c> Anonymous </c> </remarks>
        public static string AccessKey { get; set; } = string.Empty;

        /// <summary>
        ///     Query parameter via http request 
        /// </summary>
        public static string AccessKeyQueryParam { get; set; } = "key";

        /// <summary>
        ///     Authenticate Token Key. Ex: Bearer 
        /// </summary>
        /// <remarks> Default is Bearer </remarks>
        public static string AuthTokenKeyName { get; set; } = "Bearer";

        public static SwaggerContactConfigModel Contact { get; set; }

        public static bool IsDescribeAllEnumsAsString { get; set; } = true;

        public static bool IsDescribeAllParametersInCamelCase { get; set; } = true;

        /// <summary>
        ///     Swagger Json file URL Request Endpoint 
        /// </summary>
        [JsonIgnore]
        public static string SwaggerEndpoint => $"{StringHelper.UriBuilder(ApiDocumentUrl, ApiDocumentName, ApiDocumentJsonFile)}?{AccessKeyQueryParam}={AccessKey}";

        /// <summary>
        // Route prefix for accessing the swagger-ui.
        /// </summary>
        [JsonIgnore]
        public static string RoutePrefix => ApiDocumentUrl.Trim('/');

        /// <summary>
        ///     Custom route for the Swagger JSON endpoint(s). 
        /// </summary>
        /// <remarks> Must include the <c> {documentName} </c> parameter </remarks>
        [JsonIgnore]
        public static string RouteTemplateEndpoint => StringHelper.UriBuilder(RoutePrefix, "{documentName}", ApiDocumentJsonFile);

        [JsonIgnore]
        public static string ViewerUrl { get; set; }

        /// <summary>
        ///     Flag to mark Api Doc (index.html) is already updated by first http request 
        /// </summary>
        [JsonIgnore]
        public static bool IsApiDocUpdated { get; set; }

        /// <summary>
        ///     Flag to mark Json Viewer (viewer.html) is already updated by first http request 
        /// </summary>
        public static bool IsViewerUpdated { get; set; }
    }
}