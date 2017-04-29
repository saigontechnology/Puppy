#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI </Project>
//     <File>
//         <Name> BaseEntityRepository.cs </Name>
//         <Created> 25 Apr 17 10:52:19 PM </Created>
//         <Key> 901d3a41-e746-400a-83df-6150d206c1b5 </Key>
//     </File>
//     <Summary>
//         BaseEntityRepository.cs
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
    public class BaseEntityRepository<TEntity> : IBaseEntityRepository<TEntity> where TEntity : class, IBaseEntity
    {
        private readonly IBaseDbContext _baseDbContext;

        public BaseEntityRepository(IBaseDbContext baseDbContext)
        {
            _baseDbContext = baseDbContext;
        }

        public IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _baseDbContext.Set<TEntity>().AsNoTracking();
            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _baseDbContext.Set<TEntity>().AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return isIncludeDeleted ? query : query.Where(x => !x.IsDeleted);
        }

        public TEntity GetSingle(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return Get(predicate, isIncludeDeleted, includeProperties).FirstOrDefault();
        }

        public TEntity Add(TEntity entity)
        {
            entity.IsDeleted = false;
            entity.LastUpdatedOnUtc = null;
            entity.CreatedOnUtc = entity.CreatedOnUtc == default(DateTimeOffset) ? DateTimeOffset.UtcNow : entity.CreatedOnUtc;

            entity = _baseDbContext.Set<TEntity>().Add(entity).Entity;
            _baseDbContext.SaveChanges();
            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            entity.LastUpdatedOnUtc = entity.LastUpdatedOnUtc == default(DateTimeOffset) ? DateTimeOffset.UtcNow : entity.LastUpdatedOnUtc;

            EntityEntry dbEntityEntry = _baseDbContext.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;

            _baseDbContext.SaveChanges();

            return entity;
        }

        public void Delete(TEntity entity, bool isPhysicalDelete = false)
        {
            if (!isPhysicalDelete)
            {
                entity.IsDeleted = true;
                entity.DeletedOnUtc = entity.DeletedOnUtc == default(DateTimeOffset) ? DateTimeOffset.UtcNow : entity.DeletedOnUtc;
                Update(entity);
            }
            _baseDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public void DeleteWhere(Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false)
        {
            IEnumerable<TEntity> entities = Get(predicate, isPhysicalDelete).AsEnumerable();
            foreach (TEntity entity in entities)
            {
                _baseDbContext.Entry(entity).State = EntityState.Deleted;
            }
            _baseDbContext.SaveChanges();
        }
    }
}