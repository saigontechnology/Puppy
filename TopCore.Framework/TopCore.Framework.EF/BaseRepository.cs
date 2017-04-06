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
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly IBaseDbContext _dbContext;

        public BaseRepository(IBaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().Any(predicate);
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().Count(predicate);
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().Where(predicate);
        }

        public TEntity Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public ICollection<TEntity> AddRange(ICollection<TEntity> entities)
        {
            _dbContext.Set<TEntity>().AddRange(entities);
            _dbContext.SaveChanges();
            return entities;
        }

        public TEntity Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public ICollection<TEntity> UpdateRange(ICollection<TEntity> entities)
        {
            _dbContext.Set<TEntity>().UpdateRange(entities);
            _dbContext.SaveChanges();
            return entities;
        }

        public TEntity Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public ICollection<TEntity> DeleteRange(ICollection<TEntity> entities)
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
            _dbContext.SaveChanges();
            return entities;
        }
    }
}