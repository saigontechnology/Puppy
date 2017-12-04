#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> UrlHelperExtensions.cs </Name>
//         <Created> 07/06/2017 9:55:19 PM </Created>
//         <Key> 31bca843-09fb-4e4a-b083-b99a8642b6d8 </Key>
//     </File>
//     <Summary>
//         UrlHelperExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Mvc;
using Puppy.Web.Models.Api;
using System;

namespace Puppy.Web
{
    /// <summary>
    ///     <see cref="IUrlHelper" /> extension methods. 
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        ///     Generates a fully qualified URL to an action method by using the specified action
        ///     name, controller name and route values.
        /// </summary>
        /// <param name="url">            The URL helper. </param>
        /// <param name="actionName">     The name of the action method. </param>
        /// <param name="controllerName"> The name of the controller. </param>
        /// <param name="routeValues">    The route values. </param>
        /// <returns> The absolute URL. </returns>
        public static string AbsoluteAction(
            this IUrlHelper url,
            string actionName,
            string controllerName,
            object routeValues = null)
        {
            return url.Action(actionName, controllerName, routeValues, url.ActionContext.HttpContext.Request.Scheme);
        }

        /// <summary>
        ///     Generates a fully qualified URL to the specified content by using the specified
        ///     content path. Converts a virtual (relative) path to an application absolute path.
        /// </summary>
        /// <param name="url">         The URL helper. </param>
        /// <param name="contentPath"> The content path. </param>
        /// <returns> The absolute URL. </returns>
        public static string AbsoluteContent(
            this IUrlHelper url,
            string contentPath)
        {
            var request = url.ActionContext.HttpContext.Request;
            return new Uri(new Uri(request.Scheme + "://" + request.Host.Value), url.Content(contentPath)).ToString();
        }

        /// <summary>
        ///     Generates a fully qualified URL to the specified route by using the route name and
        ///     route values.
        /// </summary>
        /// <param name="url">         The URL helper. </param>
        /// <param name="routeName">   Name of the route. </param>
        /// <param name="routeValues"> The route values. </param>
        /// <returns> The absolute URL. </returns>
        public static string AbsoluteRouteUrl(
            this IUrlHelper url,
            string routeName,
            object routeValues = null)
        {
            return url.RouteUrl(routeName, routeValues, url.ActionContext.HttpContext.Request.Scheme);
        }

        public static PagedCollectionModel<T> GeneratePagedCollectionResult<T>(this IUrlHelper urlHelper, PagedCollectionResultModel<T> pagedCollectionResult, string method = "GET") where T : class, new()
        {
            var responseData = new PagedCollectionFactoryModel<T>(urlHelper, pagedCollectionResult.Skip,
                    pagedCollectionResult.Take, pagedCollectionResult.Terms, pagedCollectionResult.Total,
                    pagedCollectionResult.Items, pagedCollectionResult.AdditionalData, method)
                .Generate();

            return responseData;
        }
    }
}