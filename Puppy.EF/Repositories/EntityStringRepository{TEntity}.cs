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
using Puppy.EF.Entities;
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
            TryAttach(entity);

            changedProperties = changedProperties?.Distinct().ToArray();

            entity.LastUpdatedTime = DateTimeHelper.ReplaceNullOrDefault(entity.LastUpdatedTime, DateTimeOffset.UtcNow);

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

        public override void Update(TEntity entity, params string[] changedProperties)
        {
            TryAttach(entity);

            changedProperties = changedProperties?.Distinct().ToArray();

            entity.LastUpdatedTime = DateTimeHelper.ReplaceNullOrDefault(entity.LastUpdatedTime, DateTimeOffset.UtcNow);

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

        public void UpdateWhere(Expression<Func<TEntity, bool>> predicate, TEntity entityNewData, params Expression<Func<TEntity, object>>[] changedProperties)
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

        public void UpdateWhere(Expression<Func<TEntity, bool>> predicate, TEntity entityNewData, params string[] changedProperties)
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

        public override void Delete(TEntity entity, bool isPhysicalDelete = false)
        {
            try
            {
                TryAttach(entity);

                if (!isPhysicalDelete)
                {
                    entity.DeletedTime = DateTimeHelper.ReplaceNullOrDefault(entity.LastUpdatedTime, DateTimeOffset.UtcNow);

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

        public void DeleteWhere(List<string> listId, bool isPhysicalDelete = false)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            foreach (var id in listId)
            {
                var entity = new TEntity
                {
                    Id = id,
                    DeletedTime = utcNow
                };

                Delete(entity, isPhysicalDelete);
            }
        }
    }
}