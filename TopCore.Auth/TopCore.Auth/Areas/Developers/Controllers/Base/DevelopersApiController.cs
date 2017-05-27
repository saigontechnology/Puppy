using Microsoft.AspNetCore.Mvc;
using TopCore.Auth.Filters;
using TopCore.Framework.Web;

// ReSharper disable once CheckNamespace
namespace TopCore.Auth.Areas.Developers.Controllers
{
    [Area("Developers")]
    [ServiceFilter(typeof(ApiExceptionFilter))]
    [ServiceFilter(typeof(DeveloperAccessFilter))]
    [HideInDocs]
    [Produces("application/json", "application/xml")]
    public class DevelopersApiController : Controller
    {
    }
}