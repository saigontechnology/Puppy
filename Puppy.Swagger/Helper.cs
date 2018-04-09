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
using Puppy.Core.StringUtils;
using Puppy.Web.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Puppy.Swagger
{
    internal static class Helper
    {
        internal const string CookieAccessKeyName = "Swagger_AccessKey";

        /// <summary>
        ///     Get API DOC Html 
        /// </summary>
        /// <returns></returns>
        internal static ContentResult GetApiDocHtml()
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

            string indexFileFullPath = Constants.IndexHtmlPath.GetFullPath(null);
            var indexFileContent = File.ReadAllText(indexFileFullPath);

            ContentResult contentResult = new ContentResult
            {
                ContentType = ContentType.Html,
                StatusCode = StatusCodes.Status200OK,
                Content = indexFileContent
            };

            return contentResult;
        }

        internal static ContentResult GetApiJsonViewerHtml()
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

            string jsonViewerFileFullPath = Constants.ViewerHtmlPath.GetFullPath(null);
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
        internal static bool IsCanAccessSwagger(HttpContext httpContext)
        {
            // Null access key is allow anonymous
            if (string.IsNullOrWhiteSpace(SwaggerConfig.AccessKey))
            {
                return true;
            }

            string requestKey = httpContext.Request.Query[SwaggerConfig.AccessKeyQueryParam];

            if (string.IsNullOrWhiteSpace(requestKey))
            {
                if (httpContext.Request.Cookies.TryGetValue(CookieAccessKeyName, out var cookieRequestKey))
                {
                    requestKey = cookieRequestKey;
                }
            }

            // Case sensitive compare
            var isCanAccess = SwaggerConfig.AccessKey == requestKey;

            return isCanAccess;
        }

        internal static bool IsSwaggerUi(HttpContext httpContext)
        {
            var pathQuery = httpContext.Request.Path.Value?.Trim('/').ToLower() ?? String.Empty;
            pathQuery = pathQuery.ToLowerInvariant();

            var documentApiBaseUrl = SwaggerConfig.RoutePrefix ?? String.Empty;
            documentApiBaseUrl = documentApiBaseUrl.ToLowerInvariant();

            var isSwaggerUi = pathQuery == documentApiBaseUrl || pathQuery == $"{documentApiBaseUrl}/index.html";
            return isSwaggerUi;
        }

        internal static void UpdateFileContent(Dictionary<string, string> replaceDictionary, string filePath)
        {
            string fileFullPath = filePath.GetFullPath(null);

            var viewerFileContent = File.ReadAllText(fileFullPath);

            foreach (var key in replaceDictionary.Keys)
            {
                viewerFileContent = viewerFileContent.Replace(key, replaceDictionary[key]);
            }

            File.WriteAllText(fileFullPath, viewerFileContent, Encoding.UTF8);
        }
    }
}