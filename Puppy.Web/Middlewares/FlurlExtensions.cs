#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> FlurlExtensions.cs </Name>
//         <Created> 19/11/2017 12:22:27 AM </Created>
//         <Key> ac48621b-6f3f-4d7b-9ac5-693eb0534428 </Key>
//     </File>
//     <Summary>
//         FlurlExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Flurl.Http.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puppy.Core.ServiceCollectionUtils;

namespace Puppy.Web.Middlewares
{
    public static class FlurlExtensions
    {
        /// <summary>
        ///     [Http Client] Flurl, see more: https://github.com/tmenier/Flurl 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFlurl(this IServiceCollection services)
        {
            services.AddSingletonIfNotExist<IFlurlClientFactory, PerBaseUrlFlurlClientFactory>();
            return services;
        }
    }
}