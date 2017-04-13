using Microsoft.AspNetCore.Mvc;
using TopCore.Auth.Filters;

// ReSharper disable once CheckNamespace
namespace TopCore.Auth.Controllers
{
    [ApiExceptionFilter]
    [Produces("application/json", "application/xml")]
    public class ApiController : Controller
    {
    }
}