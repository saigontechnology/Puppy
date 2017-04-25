#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI </Project>
//     <File>
//         <Name> BaseRepository.cs </Name>
//         <Created> 23 Apr 17 3:57:10 PM </Created>
//         <Key> 3b818713-4d97-47d0-8844-ec124b27f1e0 </Key>
//     </File>
//     <Summary>
//         BaseRepository.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TopCore.Framework.EF.Interfaces;

namespace TopCore.Framework.EF
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IBaseDbContext _baseDbContext;

        public BaseRepository(IBaseDbContext baseDbContext)
        {
            _baseDbContext = baseDbContext;
        }

        public IQueryable<T> Include(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _baseDbContext.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _baseDbContext.Set<T>();
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return predicate == null ? query : query.Where(predicate);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return Get(predicate, includeProperties).FirstOrDefault();
        }

        public T Add(T entity)
        {
            entity = _baseDbContext.Set<T>().Add(entity).Entity;
            _baseDbContext.SaveChanges();
            return entity;
        }

        public T Update(T entity)
        {
            EntityEntry dbEntityEntry = _baseDbContext.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;

            _baseDbContext.SaveChanges();

            return entity;
        }

        public void Delete(T entity)
        {
            EntityEntry dbEntityEntry = _baseDbContext.Entry(entity);
            dbEntityEntry.State = EntityState.Deleted;
            _baseDbContext.SaveChanges();
        }

        public void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = Get(predicate).AsEnumerable();

            foreach (T entity in entities)
            {
                _baseDbContext.Entry(entity).State = EntityState.Deleted;
            }

            _baseDbContext.SaveChanges();
        }
    }
}