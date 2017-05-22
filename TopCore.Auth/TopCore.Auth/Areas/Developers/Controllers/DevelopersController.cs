using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TopCore.Framework.Core;

namespace TopCore.Auth.Areas.Developers.Controllers
{
    [Route("developers")]
    public class DevelopersController : DevelopersMvcController
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public DevelopersController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Index(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                key = string.Empty;

            var documentName = ConfigHelper.GetValue("appsettings.json", "Developers:ApiDocumentName");
            var documentApiBaseUrl = ConfigHelper.GetValue("appsettings.json", "Developers:ApiDocumentUrl") +
                                     documentName;
            var documentJsonFileName = ConfigHelper.GetValue("appsettings.json", "Developers:ApiDocumentJsonFile");
            var documentUrlBase = documentApiBaseUrl.Replace(documentName, string.Empty).TrimEnd('/');
            var swaggerEndpoint = $"{documentUrlBase}/{documentName}/{documentJsonFileName}" + "?key=" + key;
            ViewBag.ApiDocumentPath = _contextAccessor.HttpContext.Request.Scheme + "://" +
                                      _contextAccessor.HttpContext.Request.Host.Value + swaggerEndpoint;
            ViewBag.ApiKey = key;

            return View();
        }

        [Route("Viewer")]
        [HttpGet]
        public IActionResult Viewer(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                key = string.Empty;
            ViewBag.ApiKey = key;

            return View();
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult DownloadErrorHandleJavaFile(string key)
        {
            throw new NotImplementedException();
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult DownloadErrorHandleCSharpFile(string key)
        {
            throw new NotImplementedException();
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult DownloadErrorHandleJavascriptFile(string key)
        {
            throw new NotImplementedException();
        }
    }
}