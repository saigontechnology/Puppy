#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Helper.cs </Name>
//         <Created> 08/08/17 4:37:02 PM </Created>
//         <Key> 4b1d16d8-8a80-4385-ab69-174b03ab0942 </Key>
//     </File>
//     <Summary>
//         Helper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Puppy.Web.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Puppy.Swagger
{
    public static class Helper
    {
        /// <summary>
        ///     Get API DOC Html 
        /// </summary>
        /// <returns></returns>
        public static ContentResult GetApiDocHtml()
        {
            if (!SwaggerConfig.IsApiDocumentUiUpdated)
            {
                UpdateFileContent(new Dictionary<string, string>
                {
                    {"@AssetPath", Constants.ApiDocAssetRequestPath},
                    {"@ApiDocumentHtmlTitle", SwaggerConfig.ApiDocumentHtmlTitle},
                    {"@SwaggerEndpoint", SwaggerConfig.SwaggerEndpoint},
                    {"@AuthTokenKeyPrefix", SwaggerConfig.AuthTokenKeyName},
                    { "@JsonViewerUrl", SwaggerConfig.JsonViewerUiUrl }
                }, Constants.IndexHtmlPath);

                SwaggerConfig.IsApiDocumentUiUpdated = true;
            }

            string executedFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string indexFileFullPath = Path.Combine(executedFolder, Constants.IndexHtmlPath);
            var indexFileContent = File.ReadAllText(indexFileFullPath);

            ContentResult contentResult = new ContentResult
            {
                ContentType = ContentType.Html,
                StatusCode = StatusCodes.Status200OK,
                Content = indexFileContent
            };

            return contentResult;
        }

        public static ContentResult GetApiJsonViewerHtml()
        {
            if (!SwaggerConfig.IsJsonViewerUrlUpdated)
            {
                UpdateFileContent(new Dictionary<string, string>
                {
                    {"@AssetPath", Constants.ApiDocAssetRequestPath},
                    {"@ApiDocumentHtmlTitle", SwaggerConfig.ApiDocumentHtmlTitle}
                }, Constants.ViewerHtmlPath);

                SwaggerConfig.IsJsonViewerUrlUpdated = true;
            }

            string executedFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string jsonViewerFileFullPath = Path.Combine(executedFolder, Constants.ViewerHtmlPath);
            var jsonViewerFileContent = File.ReadAllText(jsonViewerFileFullPath);

            ContentResult contentResult = new ContentResult
            {
                ContentType = ContentType.Html,
                StatusCode = StatusCodes.Status200OK,
                Content = jsonViewerFileContent
            };

            return contentResult;
        }

        /// <summary>
        ///     Case sensitive compare for key access 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static bool IsCanAccessSwagger(HttpContext httpContext)
        {
            if (String.IsNullOrWhiteSpace(SwaggerConfig.AccessKeyQueryParam))
            {
                return true;
            }

            string paramKeyValue = httpContext.Request.Query[SwaggerConfig.AccessKeyQueryParam];

            if (String.IsNullOrWhiteSpace(SwaggerConfig.AccessKey))
            {
                return true;
            }

            if (String.IsNullOrWhiteSpace(SwaggerConfig.AccessKey) && String.IsNullOrWhiteSpace(paramKeyValue))
            {
                return true;
            }

            // Case sensitive compare
            var isCanAccess = SwaggerConfig.AccessKey == paramKeyValue;
            return isCanAccess;
        }

        public static bool IsSwaggerUi(HttpContext httpContext)
        {
            var pathQuery = httpContext.Request.Path.Value?.Trim('/').ToLower() ?? String.Empty;
            pathQuery = pathQuery.ToLowerInvariant();

            var documentApiBaseUrl = SwaggerConfig.RoutePrefix ?? String.Empty;
            documentApiBaseUrl = documentApiBaseUrl.ToLowerInvariant();

            var isSwaggerUi = pathQuery == documentApiBaseUrl || pathQuery == $"{documentApiBaseUrl}/index.html";
            return isSwaggerUi;
        }

        public static bool IsRequestTheEndpoint(HttpContext httpContext, string endpoint)
        {
            // get path query with out query param string
            var pathQuery = httpContext.Request.Path.Value?.Trim('/').ToLower() ?? String.Empty;
            var iPathQueryWithoutParam = pathQuery.IndexOf('?');
            pathQuery = iPathQueryWithoutParam > 0 ? pathQuery.Substring(iPathQueryWithoutParam) : pathQuery;
            pathQuery = pathQuery.ToLowerInvariant();

            // get endpoint without query param string
            endpoint = endpoint.Trim('/');
            var iEndpointWithoutParam = endpoint.IndexOf('?');
            endpoint = iEndpointWithoutParam > 0 ? endpoint.Substring(0, iEndpointWithoutParam) : endpoint;
            endpoint = endpoint.ToLowerInvariant();

            // check quest is swagger endpoint
            var isSwaggerEndPoint = pathQuery == endpoint;
            return isSwaggerEndPoint;
        }

        public static void UpdateFileContent(Dictionary<string, string> replaceDictionary, string filePath)
        {
            string executedFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            string fileFullPath = Path.Combine(executedFolder, filePath);

            var viewerFileContent = File.ReadAllText(fileFullPath);

            foreach (var key in replaceDictionary.Keys)
            {
                viewerFileContent = viewerFileContent.Replace(key, replaceDictionary[key]);
            }

            File.WriteAllText(fileFullPath, viewerFileContent, Encoding.UTF8);
        }
    }
}