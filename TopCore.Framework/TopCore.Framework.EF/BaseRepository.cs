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
using TopCore.Framework.EF.Interfaces;

namespace TopCore.Framework.EF
{
    public class BaseRepository<T> : IBaseRepository<T>, IBaseEntityRepository where T : class
    {
        private readonly IBaseDbContext _baseDbContext;

        public BaseRepository(IBaseDbContext baseDbContext)
        {
            _baseDbContext = baseDbContext;
        }

        public IQueryable<T> AllInclude(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _baseDbContext.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate = null)
        {
            var query = _baseDbContext.Set<T>();
            return predicate == null ? query : query.Where(predicate);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            return Get(predicate).FirstOrDefault();
        }

        public T Add(T entity)
        {
            entity = _baseDbContext.Set<T>().Add(entity).Entity;
            _baseDbContext.SaveChanges();
            return entity;
        }

        public T Update(T entity)
        {
            var dbEntityEntry = _baseDbContext.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;

            _baseDbContext.SaveChanges();

            return entity;
        }

        public void Delete(T entity)
        {
            var dbEntityEntry = _baseDbContext.Entry(entity);
            dbEntityEntry.State = EntityState.Deleted;
            _baseDbContext.SaveChanges();
        }

        public void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = Get(predicate).AsEnumerable();

            foreach (var entity in entities)
            {
                _baseDbContext.Entry(entity).State = EntityState.Deleted;
            }

            _baseDbContext.SaveChanges();
        }

        #region Entity

        public IQueryable<TEntity> AllInclude<TEntity>(params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class, IBaseEntity
        {
            IQueryable<TEntity> query = _baseDbContext.Set<TEntity>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public IQueryable<TEntity> GetEntity<TEntity>(Expression<Func<TEntity, bool>> predicate = null, bool isIncludeDeleted = false) where TEntity : class, IBaseEntity
        {
            var query = _baseDbContext.Set<TEntity>().Where(predicate);
            return isIncludeDeleted ? query : query.Where(x => !x.IsDeleted);
        }

        public TEntity GetSingleEntity<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false) where TEntity : class, IBaseEntity
        {
            return GetEntity(predicate, isIncludeDeleted).FirstOrDefault();
        }

        public TEntity AddEntity<TEntity>(TEntity entity) where TEntity : class, IBaseEntity
        {
            entity.IsDeleted = false;
            entity.LastUpdatedOnUtc = null;
            entity.CreatedOnUtc = DateTime.UtcNow;

            entity = _baseDbContext.Set<TEntity>().Add(entity).Entity;
            _baseDbContext.SaveChanges();
            return entity;
        }

        public TEntity UpdateEntity<TEntity>(TEntity entity) where TEntity : class, IBaseEntity
        {
            entity.LastUpdatedOnUtc = DateTime.UtcNow;

            var dbEntityEntry = _baseDbContext.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;

            _baseDbContext.SaveChanges();

            return entity;
        }

        public void DeleteEntity<TEntity>(TEntity entity, bool isPhysicalDelete = false) where TEntity : class, IBaseEntity
        {
            if (!isPhysicalDelete)
            {
                entity.IsDeleted = true;
                entity.DeletedOnUtc = DateTime.UtcNow;
                UpdateEntity(entity);
            }
            _baseDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public void DeleteEntityWhere<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false) where TEntity : class, IBaseEntity
        {
            IEnumerable<TEntity> entities = GetEntity(predicate, isPhysicalDelete).AsEnumerable();
            foreach (var entity in entities)
            {
                _baseDbContext.Entry(entity).State = EntityState.Deleted;
            }
            _baseDbContext.SaveChanges();
        }

        #endregion
    }
}