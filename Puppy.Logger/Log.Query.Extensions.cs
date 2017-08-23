using Puppy.Logger.Core.Models;
using Puppy.Logger.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Puppy.Logger
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