#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> TypeExtensions.cs </Name>
//         <Created> 23/07/17 11:57:36 PM </Created>
//         <Key> d5ea3f05-1597-40ce-9388-ffadcc9af058 </Key>
//     </File>
//     <Summary>
//         TypeExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Puppy.Core.ReflectionUtils
{
    public static class TypeExtensions
    {
        public static bool IsGenericEnumerable(this Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }
    }
}