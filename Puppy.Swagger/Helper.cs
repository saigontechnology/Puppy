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
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Puppy.Swagger
{
    public static class Helper
    {
        /// <summary>
        ///     Get API DOC Html 
        /// </summary>
        /// <param name="urlHelper">
        ///     Serve for update asset URL in Api Doc at first time, check by <c>
        ///     SwaggerConfig.IsApiDocUpdated </c>
        /// </param>
        /// <param name="viewerUrl">
        ///     Update Json Viewer by AspNetCore project route config at first time, check by <c>
        ///     SwaggerConfig.ViewerUrl </c>
        /// </param>
        /// <returns></returns>
        public static ContentResult GetApiDocHtml(IUrlHelper urlHelper, string viewerUrl)
        {
            if (SwaggerConfig.ViewerUrl != viewerUrl)
            {
                UpdateIndexHtml(new Dictionary<string, string>
                {
                    { "@ViewerUrl", viewerUrl }
                });

                SwaggerConfig.ViewerUrl = viewerUrl;
            }

            if (!SwaggerConfig.IsApiDocUpdated)
            {
                UpdateIndexHtml(new Dictionary<string, string>
                {
                    {"@asset", urlHelper.AbsoluteContent(Constant.ApiDocAssetRequestPath)},
                    {"@ApiDocumentHtmlTitle", SwaggerConfig.ApiDocumentHtmlTitle},
                    {"@SwaggerEndpoint", SwaggerConfig.SwaggerEndpoint},
                    {"@AuthTokenKeyPrefix", SwaggerConfig.AuthTokenKeyName}
                });

                SwaggerConfig.IsApiDocUpdated = true;
            }

            string executedFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string indexFileFullPath = Path.Combine(executedFolder, Constant.IndexHtmlPath);
            var indexFileContent = File.ReadAllText(indexFileFullPath);

            ContentResult contentResult = new ContentResult
            {
                ContentType = "html",
                StatusCode = 200,
                Content = indexFileContent
            };

            return contentResult;
        }

        public static ContentResult GetApiViewerHtml(IUrlHelper urlHelper)
        {
            if (!SwaggerConfig.IsViewerUpdated)
            {
                UpdateIndexHtml(new Dictionary<string, string>
                {
                    {"@asset", urlHelper.AbsoluteContent(Constant.ApiDocAssetRequestPath)},
                    {"@ApiDocumentHtmlTitle", SwaggerConfig.ApiDocumentHtmlTitle}
                });

                SwaggerConfig.IsViewerUpdated = true;
            }

            string executedFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string jsonViewerFileFullPath = Path.Combine(executedFolder, Constant.ViewerHtmlPath);
            var jsonViewerFileContent = File.ReadAllText(jsonViewerFileFullPath);

            ContentResult contentResult = new ContentResult
            {
                ContentType = "html",
                StatusCode = 200,
                Content = jsonViewerFileContent
            };

            return contentResult;
        }

        public static bool IsCanAccessSwagger(HttpContext httpContext)
        {
            string requestKey = httpContext.Request.Query[SwaggerConfig.AccessKeyQueryParam];
            var isCanAccess = String.IsNullOrWhiteSpace(SwaggerConfig.AccessKey) || SwaggerConfig.AccessKey == requestKey;
            return isCanAccess;
        }

        public static void UpdateIndexHtml(Dictionary<string, string> replaceDictionary)
        {
            string executedFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string indexFileFullPath = Path.Combine(executedFolder, Constant.IndexHtmlPath);

            var indexFileContent = File.ReadAllText(indexFileFullPath);

            foreach (var key in replaceDictionary.Keys)
            {
                indexFileContent = indexFileContent.Replace(key, replaceDictionary[key]);
            }

            File.WriteAllText(indexFileFullPath, indexFileContent);
        }

        public static void UpdateViewHtml(Dictionary<string, string> replaceDictionary)
        {
            string executedFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string viewerFileFullPath = Path.Combine(executedFolder, Constant.ViewerHtmlPath);
            var viewerFileContent = File.ReadAllText(viewerFileFullPath);

            foreach (var key in replaceDictionary.Keys)
            {
                viewerFileContent = viewerFileContent.Replace(key, replaceDictionary[key]);
            }
            File.WriteAllText(viewerFileFullPath, viewerFileContent);
        }

        private static string AbsoluteContent(this IUrlHelper url, string contentPath)
        {
            var request = url.ActionContext.HttpContext.Request;
            return new Uri(new Uri(request.Scheme + "://" + request.Host.Value), url.Content(contentPath)).ToString();
        }
    }
}