#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Framework.EF </Project>
//     <File>
//         <Name> Repository </Name>
//         <Created> 06 Apr 17 1:10:29 PM </Created>
//         <Key> f6b6d77b-b212-4ca2-96d1-1d029ef23e04 </Key>
//     </File>
//     <Summary>
//         BaseRepository
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TopCore.Framework.EF.Interfaces;

namespace TopCore.Framework.EF
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
    {
        private readonly IBaseDbContext _dbContext;

        public BaseRepository(IBaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TEntity Get(Guid key, bool isIncludeDeleted = false)
        {
            Expression<Func<TEntity, bool>> filter = x => x.Key == key && ConditionGetDeleted(x, isIncludeDeleted);
            return _dbContext.Set<TEntity>().FirstOrDefault(filter);
        }

        public TEntity Get(string key, bool isIncludeDeleted = false)
        {
            return Get(new Guid(key), isIncludeDeleted);
        }

        public IEnumerable<TEntity> GetAll(bool isIncludeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false)
        {
            return _dbContext.Set<TEntity>()
                .Where(x => ConditionGetDeleted(x, isIncludeDeleted))
                .Where(predicate)
                .AsEnumerable();
        }

        public void Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
            _dbContext.SaveChanges();
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity, bool isPhysicalDiskDelete = false)
        {
            throw new NotImplementedException();
        }

        public void DeleteRange(IEnumerable<TEntity> entities, bool isPhysicalDiskDelete = false)
        {
            throw new NotImplementedException();
        }

        private bool ConditionGetDeleted(TEntity entity, bool isIncludeDeleted)
        {
            return isIncludeDeleted || entity.IsDeleted == false;
        }
    }
}