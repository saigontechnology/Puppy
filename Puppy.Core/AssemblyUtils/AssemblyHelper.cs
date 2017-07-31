#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> AssemblyHelper.cs </Name>
//         <Created> 01/08/17 12:07:12 AM </Created>
//         <Key> 4224e24e-8af9-43b7-b56a-2274afc014ae </Key>
//     </File>
//     <Summary>
//         AssemblyHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puppy.Core.AssemblyUtils
{
    public static class AssemblyHelper
    {
        public static IEnumerable<Assembly> GetRuntimeAssemblies()
        {
            var runtimeId = RuntimeEnvironment.GetRuntimeIdentifier();
            var assemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId);
            return assemblyNames.Select(Assembly.Load).ToList();
        }
    }
}