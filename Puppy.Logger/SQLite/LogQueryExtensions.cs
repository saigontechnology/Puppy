#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> LogQueryExtensions.cs </Name>
//         <Created> 23/08/17 9:13:22 PM </Created>
//         <Key> 7f383467-708d-457f-b865-bf65110fdac5 </Key>
//     </File>
//     <Summary>
//         LogQueryExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.Logger.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Puppy.Logger.SQLite
{
    public static class LogQueryExtensions
    {
        public static IQueryable<LogEntity> Where(Expression<Func<LogEntity, bool>> predicate = null)
        {
            var db = new SqliteDbContext();
            Repository<LogEntity> logRepository = new Repository<LogEntity>(db);
            var query = logRepository.Get(predicate);
            return query;
        }

        /// <summary>
        ///     Get result with fill info for HttpContext and Exception from Json 
        /// </summary>
        /// <param name="query">    </param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<LogEntity> Get(this IQueryable<LogEntity> query, Expression<Func<LogEntity, bool>> predicate = null)
        {
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var result = query.ToList().Select(x => x.FillInfo()).ToList();

            return result;
        }
    }
}