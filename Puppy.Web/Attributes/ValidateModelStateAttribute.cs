#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ValidateModelStateAttribute.cs </Name>
//         <Created> 07/06/2017 11:06:49 PM </Created>
//         <Key> a5ac9a06-e155-4cfa-8e71-9cb42d751650 </Key>
//     </File>
//     <Summary>
//         ValidateModelStateAttribute.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Puppy.Web.Attributes
{
    /// <summary>
    ///     Validates the action's arguments and model state. If any of the arguments are <c> null
    ///     </c> or the model state is invalid, returns a 400 Bad Request result.
    /// </summary>
    /// <seealso cref="ActionFilterAttribute" />
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        /// <summary>
        ///     Called when the action is executing. 
        /// </summary>
        /// <param name="actionContext"> The action context. </param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (actionContext.ActionArguments.Any(x => x.Value == null))
                actionContext.Result = new BadRequestObjectResult("Arguments cannot be null.");
            else if (!actionContext.ModelState.IsValid)
                actionContext.Result = new BadRequestObjectResult(actionContext.ModelState);
        }
    }
}