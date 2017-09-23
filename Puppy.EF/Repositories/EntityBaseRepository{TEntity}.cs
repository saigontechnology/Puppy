#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EntityBaseRepository.cs </Name>
//         <Created> 09/09/17 6:01:26 PM </Created>
//         <Key> 44ee04d6-9690-4902-84b4-437c033b72df </Key>
//     </File>
//     <Summary>
//         EntityBaseRepository.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.EntityFrameworkCore;
using Puppy.Core.DateTimeUtils;
using Puppy.EF.Extensions;
using Puppy.EF.Interfaces;
using Puppy.EF.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Puppy.EF.Repositories
{
    public abstract class EntityBaseRepository<TEntity> : Repository<TEntity>, IEntityBaseRepository<TEntity> where TEntity : EntityBase
    {
        protected readonly IBaseDbContext DbContext;

        protected EntityBaseRepository(IBaseDbContext dbContext) : base(dbContext)
        {
            DbContext = dbContext;
        }

        public virtual TEntity GetSingle(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return Get(predicate, isIncludeDeleted, includeProperties).FirstOrDefault();
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

        public override TEntity Add(TEntity entity)
        {
            entity.DeletedTime = null;
            entity.LastUpdatedTime = null;
            entity.CreatedTime = DateTimeHelper.ReplaceNullOrDefault(entity.CreatedTime, DateTimeOffset.UtcNow);
            entity = DbSet.Add(entity).Entity;
            return entity;
        }

        public List<TEntity> AddRange(params TEntity[] listEntity)
        {
            var dateTimeUtcNow = DateTimeOffset.UtcNow;

            List<TEntity> listAddedEntity = new List<TEntity>();

            foreach (var entity in listEntity)
            {
                entity.CreatedTime = dateTimeUtcNow;

                var addedEntity = Add(entity);

                listAddedEntity.Add(addedEntity);
            }

            return listAddedEntity;
        }

        public override void Update(TEntity entity, params Expression<Func<TEntity, object>>[] changedProperties)
        {
            TryAttach(entity);

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
                TryAttach(entity);

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

        public override int SaveChanges()
        {
            StandardizeEntities();
            return DbContext.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            StandardizeEntities();
            return DbContext.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            StandardizeEntities();
            return DbContext.SaveChangesAsync(cancellationToken);
        }

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
}