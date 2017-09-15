#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EntityStringRepository.cs </Name>
//         <Created> 09/09/17 6:02:09 PM </Created>
//         <Key> db21fce2-77ff-483e-b1da-3c443068f8f0 </Key>
//     </File>
//     <Summary>
//         EntityStringRepository.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.EntityFrameworkCore;
using Puppy.Core.DateTimeUtils;
using Puppy.Core.ObjectUtils;
using Puppy.EF.Interfaces;
using Puppy.EF.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Puppy.EF.Repositories
{
    public abstract class EntityStringRepository<TEntity> : EntityBaseRepository<TEntity>, IEntityStringRepository<TEntity> where TEntity : EntityString, new()
    {
        protected EntityStringRepository(IBaseDbContext dbContext) : base(dbContext)
        {
        }

        public override void Update(TEntity entity, params Expression<Func<TEntity, object>>[] changedProperties)
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
                TryAttach(entity);

            entity.LastUpdatedTime =
                entity.LastUpdatedTime == default(DateTimeOffset) ? DateTimeOffset.UtcNow : entity.LastUpdatedTime;

            changedProperties = changedProperties?.Distinct().ToArray();

            if (changedProperties?.Any() == true)
            {
                DbContext.Entry(entity).Property(x => x.LastUpdatedTime).IsModified = true;
                DbContext.Entry(entity).Property(x => x.LastUpdatedBy).IsModified = true;

                foreach (var property in changedProperties)
                {
                    DbContext.Entry(entity).Property(property).IsModified = true;
                }
            }
            else
                DbContext.Entry(entity).State = EntityState.Modified;
        }

        public override void Delete(TEntity entity, bool isPhysicalDelete = false)
        {
            try
            {
                if (DbContext.Entry(entity).State == EntityState.Detached)
                    TryAttach(entity);

                if (!isPhysicalDelete)
                {
                    entity.DeletedTime = entity.DeletedTime == default(DateTimeOffset) ? DateTimeOffset.UtcNow : entity.DeletedTime;
                    DbContext.Entry(entity).Property(x => x.DeletedTime).IsModified = true;
                    DbContext.Entry(entity).Property(x => x.DeletedBy).IsModified = true;
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

        public override void DeleteWhere(Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            var entities = Get(predicate).Select(x => new TEntity { Id = x.Id }).ToList();

            entities.ForEach(x =>
            {
                x.DeletedTime = utcNow;
                Delete(x, isPhysicalDelete);
            });
        }

        public TEntity Add(TEntity entity, string createdBy = null)
        {
            entity.DeletedTime = null;
            entity.LastUpdatedTime = null;
            entity.CreatedBy = createdBy;
            entity.CreatedTime = DateTimeHelper.ReplaceNullOrDefault(entity.CreatedTime, DateTimeOffset.UtcNow);
            entity = DbSet.Add(entity).Entity;
            return entity;
        }

        public List<TEntity> AddRange(string createdBy = null, params TEntity[] listEntity)
        {
            var dateTimeUtcNow = DateTimeOffset.UtcNow;

            List<TEntity> listAddedEntity = new List<TEntity>();

            foreach (var entity in listEntity)
            {
                entity.CreatedTime = dateTimeUtcNow;

                var addedEntity = Add(entity, createdBy);

                listAddedEntity.Add(addedEntity);
            }

            return listAddedEntity;
        }

        public void UpdateWhere(Expression<Func<TEntity, bool>> predicate, TEntity entityNewData, string updatedBy = null, params Expression<Func<TEntity, object>>[] changedProperties)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            var entities = Get(predicate).Select(x => new TEntity { Id = x.Id }).ToList();

            foreach (var entity in entities)
            {
                var oldEntity = entityNewData.Clone();
                oldEntity.Id = entity.Id;
                oldEntity.LastUpdatedTime = utcNow;
                Update(oldEntity, changedProperties);
            }
        }

        public void DeleteWhere(Expression<Func<TEntity, bool>> predicate, string deletedBy = null, bool isPhysicalDelete = false)
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

        public void DeleteWhere(List<string> listId, string deletedBy = null, bool isPhysicalDelete = false)
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