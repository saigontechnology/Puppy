#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> TypeHelper.cs </Name>
//         <Created> 02/09/17 10:13:27 PM </Created>
//         <Key> 0d6f6083-838b-4f00-9f20-79f0080ab5d6 </Key>
//     </File>
//     <Summary>
//         TypeHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puppy.Core.TypeUtils
{
    public static class TypeHelper
    {
        private static IEnumerable<Type> GetTypes(IEnumerable<Assembly> assemblies, bool skipOnError = true)
        {
            return assemblies
                .SelectMany(a =>
                {
                    IEnumerable<TypeInfo> types;

                    try
                    {
                        types = a.DefinedTypes;
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        if (!skipOnError)
                        {
                            throw;
                        }

                        types = e.Types.TakeWhile(t => t != null).Select(t => t.GetTypeInfo());
                    }

                    return types.Where(ti => ti.IsClass & !ti.IsAbstract && !ti.IsValueType && ti.IsVisible).Select(ti => ti.AsType());
                });
        }
    }
}