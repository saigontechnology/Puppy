using Microsoft.AspNetCore.Mvc;
using TopCore.WebAPI.Filters;

// ReSharper disable once CheckNamespace
namespace TopCore.WebAPI.Controllers
{
    [ApiExceptionFilter]
    [Produces("application/json", "application/xml")]
    public class ApiController : Controller
    {
    }
}