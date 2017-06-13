#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> NoLowercaseQueryStringAttribute.cs </Name>
//         <Created> 07/06/2017 11:05:07 PM </Created>
//         <Key> f2568547-b3d7-40ad-bd5b-b236fabc8f49 </Key>
//     </File>
//     <Summary>
//         NoLowercaseQueryStringAttribute.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Puppy.Web.Attributes
{
    /// <summary>
    ///     Ensures that a HTTP request URL can contain query string parameters with both upper-case
    ///     and lower-case characters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class NoLowercaseQueryStringAttribute : System.Attribute, IFilterMetadata
    {
    }
}