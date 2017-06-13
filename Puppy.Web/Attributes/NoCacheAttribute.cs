#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> NoCacheAttribute.cs </Name>
//         <Created> 07/06/2017 11:00:28 PM </Created>
//         <Key> 0353e02e-8a03-4009-b011-bf25c565d4ce </Key>
//     </File>
//     <Summary>
//         NoCacheAttribute.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Mvc;
using System;

namespace Puppy.Web.Attributes
{
    /// <summary>
    ///     Represents an attribute that is used to mark an action method whose output will not be cached.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoCacheAttribute : ResponseCacheAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NoCacheAttribute" /> class. 
        /// </summary>
        public NoCacheAttribute()
        {
            NoStore = true; // Duration = 0 and VaryByParam = "*" by default.
        }
    }
}