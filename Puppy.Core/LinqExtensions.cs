#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> LinqExtensions.cs </Name>
//         <Created> 28 Apr 17 2:50:00 PM </Created>
//         <Key> ee3bb118-da6a-4d3a-ba61-4a55207ce5ad </Key>
//     </File>
//     <Summary>
//         LinqExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Collections.Generic;
using System.Linq;

namespace Puppy.Core
{
    public static class LinqExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
                if (seenKeys.Add(keySelector(element)))
                    yield return element;
        }

        public static IEnumerable<TSource> RemoveWhere<TSource>
            (this IEnumerable<TSource> source, Predicate<TSource> predicate)
        {
            return source.Where(x => !predicate(x));
        }
    }
}