#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → I-Educator </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> I-Educator.WebAPI </Project>
//     <File>
//         <Name> IServiceProviderExtension.cs </Name>
//         <Created> 27 Apr 17 10:19:15 AM </Created>
//         <Key> 54d16db5-cbfe-43cc-8326-453257bdca32 </Key>
//     </File>
//     <Summary>
//         IServiceProviderExtension.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.Extensions.DependencyInjection;
using System;

namespace Puppy.DependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public static class IServiceProviderExtension
    {
        public static T Resolve<T>(this IServiceProvider services) where T : class
        {
            return services.GetService<T>();
        }
    }
}