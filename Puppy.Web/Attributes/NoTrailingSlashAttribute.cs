#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> NoTrailingSlashAttribute.cs </Name>
//         <Created> 07/06/2017 11:02:25 PM </Created>
//         <Key> 759cdd37-d76f-4300-9625-78df10b0db49 </Key>
//     </File>
//     <Summary>
//         NoTrailingSlashAttribute.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Puppy.Web.Attributes
{
    /// <summary>
    ///     Requires that a HTTP request does not contain a trailing slash. If it does, return a 404
    ///     Not Found. This is useful if you are dynamically generating something which acts like
    ///     it's a file on the web server. E.g. /Robots.txt/ should not have a trailing slash and
    ///     should be /Robots.txt. Note, that we also don't care if it is upper-case or lower-case in
    ///     this instance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NoTrailingSlashAttribute : Attribute, IResourceFilter
    {
        private const char SlashCharacter = '/';

        /// <summary>
        ///     Executes the resource filter. Called after execution of the remainder of the pipeline. 
        /// </summary>
        /// <param name="context"> The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ResourceExecutedContext" />. </param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        /// <summary>
        ///     Executes the resource filter. Called before execution of the remainder of the
        ///     pipeline. Determines whether a request contains a trailing slash and, if it does,
        ///     calls the <see cref="HandleTrailingSlashRequest" /> method.
        /// </summary>
        /// <param name="context"> The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ResourceExecutingContext" />. </param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var path = context.HttpContext.Request.Path;
            if (path.HasValue)
                if (path.Value[path.Value.Length - 1] == SlashCharacter)
                    HandleTrailingSlashRequest(context);
        }

        /// <summary>
        ///     Handles HTTP requests that have a trailing slash but are not meant to. 
        /// </summary>
        /// <param name="context"> The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ResourceExecutingContext" />. </param>
        protected virtual void HandleTrailingSlashRequest(ResourceExecutingContext context)
        {
            context.Result = new NotFoundResult();
        }
    }
}