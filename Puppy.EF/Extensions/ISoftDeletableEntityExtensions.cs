#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ISoftDeletableEntityExtensions.cs </Name>
//         <Created> 25/08/17 1:06:00 AM </Created>
//         <Key> 466d138e-5754-4a36-b7c9-b47a12f27f19 </Key>
//     </File>
//     <Summary>
//         ISoftDeletableEntityExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.EF.Interfaces.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Puppy.EF.Extensions
{
    public static class ISoftDeletableEntityExtensions
    {
        /// <summary>
        ///     Filter entities is not deleted by <c> DeletedTime != null </c> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereNotDeleted<T>(this IQueryable<T> query) where T : class, ISoftDeletableEntity
        {
            query = query.Where(x => x.DeletedTime == null);
            return query;
        }

        /// <summary>
        ///     Filter entities is not deleted by <c> DeletedTime != null </c> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iEnumerable"></param>
        /// <returns></returns>
        public static IEnumerable<T> WhereNotDeleted<T>(this IEnumerable<T> iEnumerable) where T : class, ISoftDeletableEntity
        {
            iEnumerable = iEnumerable.AsQueryable().WhereNotDeleted();
            return iEnumerable;
        }
    }
}