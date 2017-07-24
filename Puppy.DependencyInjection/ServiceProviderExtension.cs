#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ServiceProviderExtension.cs </Name>
//         <Created> 27 Apr 17 10:19:15 AM </Created>
//         <Key> 54d16db5-cbfe-43cc-8326-453257bdca32 </Key>
//     </File>
//     <Summary>
//         ServiceProviderExtension.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.Extensions.DependencyInjection;
using System;

namespace Puppy.DependencyInjection
{
    public static class ServiceProviderExtension
    {
        public static T Resolve<T>(this IServiceProvider services) where T : class
        {
            return services.GetService<T>();
        }
    }
}