#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Resolver.cs </Name>
//         <Created> 30/08/17 2:43:55 PM </Created>
//         <Key> b8fdc9c4-b825-4f42-a310-dc9ec96384c6 </Key>
//     </File>
//     <Summary>
//         Resolver.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.Extensions.DependencyInjection;

namespace Puppy.DependencyInjection
{
    public static class Resolver
    {
        /// <summary>
        ///     Static Service Collection of System 
        /// </summary>
        public static IServiceCollection Services { get; set; }

        public static T Resolve<T>() where T : class
        {
            return Services.Resolve<T>();
        }
    }
}