#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ActionExtensions.cs </Name>
//         <Created> 13/09/17 12:53:08 PM </Created>
//         <Key> ec940b55-9b4b-4899-bf11-14155b7625c8 </Key>
//     </File>
//     <Summary>
//         ActionExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;

namespace Puppy.Core.ActionUtils
{
    public static class ActionExtensions
    {
        /// <summary>
        ///     Get instance of T from Action 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T Get<T>(this Action<T> action) where T : class, new()
        {
            T obj = (T)Activator.CreateInstance(typeof(T));
            action.DynamicInvoke(obj);
            return obj;
        }
    }
}