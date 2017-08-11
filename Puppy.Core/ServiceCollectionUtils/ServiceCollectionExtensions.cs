#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ServiceCollectionExtensions.cs </Name>
//         <Created> 11/08/17 11:48:11 PM </Created>
//         <Key> ad2f517d-0029-4570-a296-cc59d6d1a035 </Key>
//     </File>
//     <Summary>
//         ServiceCollectionExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Puppy.Core.ServiceCollectionUtils
{
    public static class ServiceCollectionExtensions
    {
        public static void Removes(this IServiceCollection services, params Type[] removeTypes)
        {
            foreach (var removeType in removeTypes)
            {
                var removeTypeFound = services.FirstOrDefault(x => x.ServiceType == removeType);
                if (removeTypeFound != null) services.Remove(removeTypeFound);
            }
        }
    }
}