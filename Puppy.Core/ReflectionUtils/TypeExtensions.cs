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
using System.Linq;
using System.Reflection;

namespace Puppy.Core.ReflectionUtils
{
    public static class TypeExtensions
    {
        public static bool IsGenericEnumerable(this Type type) => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);

        public static bool IsGenericType(this Type type, Type genericType) => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;

        public static bool IsImplementGenericInterface(this Type type, Type interfaceType) => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));

        public static Assembly GetAssembly(this Type type) => type.GetTypeInfo().Assembly;

        public static IEnumerable<Assembly> GetAssemblies(this ICollection<Type> types) => types.Select(x => x.GetAssembly());

        public static IEnumerable<Assembly> GetAssemblies(this IEnumerable<Type> types) => types.Select(x => x.GetAssembly());
    }
}