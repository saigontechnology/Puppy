#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ResolveExtensions.cs </Name>
//         <Created> 27 Apr 17 10:19:15 AM </Created>
//         <Key> 54d16db5-cbfe-43cc-8326-453257bdca32 </Key>
//     </File>
//     <Summary>
//         ResolveExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Puppy.DependencyInjection
{
    public static class ResolveExtensions
    {
        public static T Resolve<T>(this IServiceCollection services) where T : class
        {
            return services.BuildServiceProvider().GetService<T>();
        }

        public static T Resolve<T>(this IApplicationBuilder applicationBuilder) where T : class
        {
            return applicationBuilder.ApplicationServices.Resolve<T>();
        }

        public static T Resolve<T>(this IServiceProvider services) where T : class
        {
            return services.GetService<T>();
        }
    }
}