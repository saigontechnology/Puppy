using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Configuration;

namespace TopCore.Auth.Areas.Developers.Controllers
{
    [Route("developers")]
    public class DevelopersController : DevelopersMvcController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfigurationRoot _configurationRoot;

        public DevelopersController(IHttpContextAccessor contextAccessor, IConfigurationRoot configurationRoot)
        {
            _contextAccessor = contextAccessor;
            _configurationRoot = configurationRoot;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Index(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                key = string.Empty;

            var documentName = _configurationRoot.GetValue<string>("Developers:ApiDocumentName");
            var documentApiBaseUrl = _configurationRoot.GetValue<string>("Developers:ApiDocumentUrl") +
                                     documentName;
            var documentJsonFileName = _configurationRoot.GetValue<string>("Developers:ApiDocumentJsonFile");
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
    }
}