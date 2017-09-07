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
using Puppy.Core.DateTimeUtils;
using Puppy.EF.Extensions;
using Puppy.EF.Interfaces;
using Puppy.EF.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Puppy.EF
{
    public abstract class EntityBaseRepository<TEntity> : Repository<TEntity> where TEntity : EntityBase
    {
        protected readonly IBaseDbContext DbContext;

        protected EntityBaseRepository(IBaseDbContext dbContext) : base(dbContext)
        {
            DbContext = dbContext;
        }

        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = DbSet.AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            includeProperties = includeProperties?.Distinct().ToArray();

            if (includeProperties?.Any() == true)
            {
                foreach (var includeProperty in includeProperties)
                    query = query.Include(includeProperty);
            }

            return isIncludeDeleted ? query : query.WhereNotDeleted();
        }

        public virtual TEntity GetSingle(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return Get(predicate, isIncludeDeleted, includeProperties).FirstOrDefault();
        }

        public override TEntity Add(TEntity entity)
        {
            entity.DeletedTime = null;
            entity.LastUpdatedTime = null;
            entity.CreatedTime = DateTimeHelper.ReplaceNullOrDefault(entity.CreatedTime, DateTimeOffset.UtcNow);

            entity = DbSet.Add(entity).Entity;
            return entity;
        }

        public override void Update(TEntity entity, params Expression<Func<TEntity, object>>[] changedProperties)
        {
            TryToAttach(entity);

            entity.LastUpdatedTime = DateTimeHelper.ReplaceNullOrDefault(entity.LastUpdatedTime, DateTimeOffset.UtcNow);

            changedProperties = changedProperties?.Distinct().ToArray();

            if (changedProperties?.Any() == true)
            {
                DbContext.Entry(entity).Property(x => x.LastUpdatedTime).IsModified = true;

                foreach (var property in changedProperties)
                {
                    DbContext.Entry(entity).Property(property).IsModified = true;
                }
            }
            else
                DbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity, bool isPhysicalDelete = false)
        {
            try
            {
                TryToAttach(entity);

                if (!isPhysicalDelete)
                {
                    entity.DeletedTime = DateTimeHelper.ReplaceNullOrDefault(entity.DeletedTime, DateTimeOffset.UtcNow);
                    DbContext.Entry(entity).Property(x => x.DeletedTime).IsModified = true;
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

        [DebuggerStepThrough]
        public override int SaveChanges()
        {
            StandardizeEntities();
            return DbContext.SaveChanges();
        }

        [DebuggerStepThrough]
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            StandardizeEntities();
            return DbContext.SaveChanges(acceptAllChangesOnSuccess);
        }

        [DebuggerStepThrough]
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            StandardizeEntities();
            return DbContext.SaveChangesAsync(cancellationToken);
        }

        [DebuggerStepThrough]
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            StandardizeEntities();
            return DbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public virtual void StandardizeEntities()
        {
            var listState = new List<EntityState>
            {
                EntityState.Added,
                EntityState.Modified
            };

            var listEntryAddUpdate = DbContext.ChangeTracker.Entries()
                .Where(x => x.Entity is TEntity && listState.Contains(x.State))
                .Select(x => x).ToList();

            var dateTimeNow = DateTimeOffset.UtcNow;

            foreach (var entry in listEntryAddUpdate)
            {
                var entity = entry.Entity as TEntity;

                if (entity == null)
                    continue;

                if (entry.State == EntityState.Added)
                {
                    entity.DeletedTime = null;
                    entity.LastUpdatedTime = null;
                    entity.CreatedTime = dateTimeNow;
                }
                else
                {
                    if (entity.DeletedTime != null)
                        entity.DeletedTime = dateTimeNow;
                    else
                        entity.LastUpdatedTime = dateTimeNow;
                }
            }
        }
    }

    public abstract class EntityRepository<TEntity, TKey> : EntityBaseRepository<TEntity>, IEntityRepository<TEntity> where TEntity : Entity<TKey>, new() where TKey : struct
    {
        protected EntityRepository(IBaseDbContext dbContext) : base(dbContext)
        {
        }

        public override void DeleteWhere(Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            var entities = Get(predicate).Select(x => new TEntity { Id = x.Id }).ToList();

            entities.ForEach(x => x.DeletedTime = utcNow);

            foreach (var entity in entities)
            {
                entity.DeletedTime = utcNow;
                Delete(entity, isPhysicalDelete);
            }
        }

        public void DeleteWhere(TKey deletedBy, Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            var entities = Get(predicate).Select(x => new TEntity
            {
                Id = x.Id
            }).ToList();

            foreach (var entity in entities)
            {
                entity.DeletedTime = utcNow;
                entity.DeletedBy = deletedBy;
                Delete(entity, isPhysicalDelete);
            }
        }

        public void DeleteWhere(TKey deletedBy, List<TKey> listId, bool isPhysicalDelete = false)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            foreach (var id in listId)
            {
                var entity = new TEntity
                {
                    Id = id,
                    DeletedTime = utcNow,
                    DeletedBy = deletedBy
                };

                Delete(entity, isPhysicalDelete);
            }
        }
    }

    public abstract class EntityStringRepository<TEntity> : EntityBaseRepository<TEntity>, IEntityRepository<TEntity> where TEntity : EntityString, new()
    {
        protected EntityStringRepository(IBaseDbContext dbContext) : base(dbContext)
        {
        }

        public override void DeleteWhere(Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            var entities = Get(predicate).Select(x => new TEntity { Id = x.Id }).ToList();

            entities.ForEach(x => x.DeletedTime = utcNow);

            foreach (var entity in entities)
            {
                entity.DeletedTime = utcNow;
                Delete(entity, isPhysicalDelete);
            }
        }

        public void DeleteWhere(string deletedBy, Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            var entities = Get(predicate).Select(x => new TEntity
            {
                Id = x.Id
            }).ToList();

            foreach (var entity in entities)
            {
                entity.DeletedTime = utcNow;
                entity.DeletedBy = deletedBy;
                Delete(entity, isPhysicalDelete);
            }
        }

        public void DeleteWhere(string deletedBy, List<string> listId, bool isPhysicalDelete = false)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            foreach (var id in listId)
            {
                var entity = new TEntity
                {
                    Id = id,
                    DeletedTime = utcNow,
                    DeletedBy = deletedBy
                };

                Delete(entity, isPhysicalDelete);
            }
        }
    }
}