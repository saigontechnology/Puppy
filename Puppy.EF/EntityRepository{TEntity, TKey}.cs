#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EntityRepository.cs </Name>
//         <Created> 25 Apr 17 10:52:19 PM </Created>
//         <Key> 901d3a41-e746-400a-83df-6150d206c1b5 </Key>
//     </File>
//     <Summary>
//         EntityRepository.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.EntityFrameworkCore;
using Puppy.EF.Interfaces;
using Puppy.EF.Interfaces.Entity;
using Puppy.EF.Interfaces.Repository;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Puppy.EF
{
    public abstract class EntityRepository<TEntity, TKey> : IEntityRepository<TEntity> where TEntity : class, ISoftDeletableEntity<TKey>, IAuditableEntity<TKey> where TKey : struct
    {
        protected readonly IBaseDbContext DbContext;

        private DbSet<TEntity> _dbSet;

        protected DbSet<TEntity> DbSet
        {
            get
            {
                if (_dbSet != null)
                    return _dbSet;
                _dbSet = DbContext.Set<TEntity>();
                return _dbSet;
            }
        }

        protected EntityRepository(IBaseDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = DbSet.AsNoTracking();
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);
            return query;
        }

        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = DbSet.AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);

            return isIncludeDeleted ? query : query.Where(x => !x.IsDeleted);
        }

        public virtual TEntity GetSingle(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return Get(predicate, isIncludeDeleted, includeProperties).FirstOrDefault();
        }

        public virtual TEntity Add(TEntity entity)
        {
            entity.IsDeleted = false;
            entity.LastUpdatedTime = null;
            entity.CreatedTime = entity.CreatedTime == default(DateTimeOffset)
                ? DateTimeOffset.UtcNow
                : entity.CreatedTime;
            entity = DbSet.Add(entity).Entity;
            return entity;
        }

        public virtual void Update(TEntity entity, params Expression<Func<TEntity, object>>[] changedProperties)
        {
            entity.LastUpdatedTime = entity.LastUpdatedTime == default(DateTimeOffset)
                ? DateTimeOffset.UtcNow
                : entity.LastUpdatedTime;
            DbSet.Attach(entity);

            if (changedProperties != null && changedProperties.Any())
                foreach (var property in changedProperties)
                {
                    var expression = (MemberExpression)property.Body;
                    var name = expression.Member.Name;
                    DbContext.Entry(entity).Property(name).IsModified = true;
                }
            else
                DbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity, bool isPhysicalDelete = false)
        {
            try
            {
                if (DbContext.Entry(entity).State == EntityState.Detached)
                    DbSet.Attach(entity);

                if (!isPhysicalDelete)
                {
                    entity.IsDeleted = true;
                    entity.DeletedTime = entity.DeletedTime == default(DateTimeOffset)
                        ? DateTimeOffset.UtcNow
                        : entity.DeletedTime;
                    Update(entity, x => x.IsDeleted, x => x.DeletedTime);
                }
                else
                {
                    DbSet.Remove(entity);
                }
            }
            catch (Exception)
            {
                RefreshEntity(entity);
                throw;
            }
        }

        public virtual void DeleteWhere(Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false)
        {
            var entities = Get(predicate).AsEnumerable();
            foreach (var entity in entities)
                Delete(entity, isPhysicalDelete);
        }

        public virtual void RefreshEntity(TEntity entity)
        {
            DbContext.Entry(entity).Reload();
        }

        [DebuggerStepThrough]
        public virtual int SaveChanges()
        {
            StandardizeEntities();
            return DbContext.SaveChanges();
        }

        [DebuggerStepThrough]
        public virtual int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            StandardizeEntities();
            return DbContext.SaveChanges(acceptAllChangesOnSuccess);
        }

        [DebuggerStepThrough]
        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            StandardizeEntities();
            return DbContext.SaveChangesAsync(cancellationToken);
        }

        [DebuggerStepThrough]
        public virtual Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            StandardizeEntities();
            return DbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public void StandardizeEntities()
        {
            var listEntryAddUpdate = DbContext.ChangeTracker.Entries()
                .Where(x => x.Entity is Entity && (x.State == EntityState.Added || x.State == EntityState.Modified))
                .Select(x => x).ToList();

            foreach (var entry in listEntryAddUpdate)
            {
                var entity = entry.Entity as TEntity;

                if (entity == null)
                    continue;

                if (entry.State == EntityState.Added)
                {
                    entity.IsDeleted = false;
                    entity.LastUpdatedTime = null;
                    entity.CreatedTime = entity.CreatedTime == default(DateTimeOffset)
                        ? DateTimeOffset.UtcNow
                        : entity.CreatedTime;
                }
                else
                {
                    if (entity.IsDeleted)
                        entity.DeletedTime = entity.DeletedTime == default(DateTimeOffset)
                            ? DateTimeOffset.UtcNow
                            : entity.DeletedTime;
                    else
                        entity.LastUpdatedTime = entity.LastUpdatedTime == default(DateTimeOffset)
                            ? DateTimeOffset.UtcNow
                            : entity.LastUpdatedTime;
                }
            }
        }
    }
}