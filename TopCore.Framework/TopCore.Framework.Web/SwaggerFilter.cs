#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> SwaggerFilter.cs </Name>
//         <Created> 03 May 17 5:59:35 PM </Created>
//         <Key> 87f1c0d0-88af-47c7-a3c8-9a24469057e1 </Key>
//     </File>
//     <Summary>
//         SwaggerFilter.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TopCore.Framework.Web
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HideInDocsAttribute : Attribute
    {
    }

    public class HideInDocsFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var apiDescriptionGroup in context.ApiDescriptionsGroups.Items)
                foreach (var apiDescription in apiDescriptionGroup.Items)
                {
                    var controllerActionDescriptor = apiDescription.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        var isHideInDocAttributeInType = controllerActionDescriptor.ControllerTypeInfo
                            .GetCustomAttributes<HideInDocsAttribute>(true)
                            .Any();
                        var isHideInDocAttributeInMethod = controllerActionDescriptor.MethodInfo
                            .GetCustomAttributes<HideInDocsAttribute>(true)
                            .Any();

                        if (!isHideInDocAttributeInType && !isHideInDocAttributeInMethod) continue;
                        var route = "/" + controllerActionDescriptor.AttributeRouteInfo.Template.TrimEnd('/');
                        swaggerDoc.Paths.Remove(route);
                    }
                }
        }
    }
}