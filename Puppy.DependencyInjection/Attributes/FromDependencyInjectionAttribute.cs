#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> FromDependencyInjectionAttribute.cs </Name>
//         <Created> 10/11/2017 2:05:41 PM </Created>
//         <Key> 146e68d9-7f20-4baa-8d8e-aab925f8ac9b </Key>
//     </File>
//     <Summary>
//         FromDependencyInjectionAttribute.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Puppy.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromDependencyInjectionAttribute : Attribute, IBindingSourceMetadata
    {
        public BindingSource BindingSource => BindingSource.Services;
    }
}