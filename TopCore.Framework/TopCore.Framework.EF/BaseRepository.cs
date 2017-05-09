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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TopCore.Framework.EF.Interfaces;

namespace TopCore.Framework.EF
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IBaseDbContext _baseDbContext;

        private DbSet<T> _dbSet;

        public BaseRepository(IBaseDbContext baseDbContext)
        {
            _baseDbContext = baseDbContext;
        }

        private DbSet<T> DbSet
        {
            get
            {
                if (_dbSet != null)
                    return _dbSet;
                _dbSet = _baseDbContext.Set<T>();
                return _dbSet;
            }
        }

        public IQueryable<T> Include(params Expression<Func<T, object>>[] includeProperties)
        {
            var query = DbSet.AsNoTracking();
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);
            return query;
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate = null,
            params Expression<Func<T, object>>[] includeProperties)
        {
            var query = DbSet.AsNoTracking();
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);
            return predicate == null ? query : query.Where(predicate);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return Get(predicate, includeProperties).FirstOrDefault();
        }

        public T Add(T entity)
        {
            entity = DbSet.Add(entity).Entity;
            return entity;
        }

        public void Update(T entity, params Expression<Func<T, object>>[] changedProperties)
        {
            DbSet.Attach(entity);

            if (changedProperties != null && changedProperties.Any())
                foreach (var property in changedProperties)
                {
                    var expression = (MemberExpression)property.Body;
                    var name = expression.Member.Name;

                    _baseDbContext.Entry(entity).Property(property).IsModified = true;
                }
            else
                _baseDbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            try
            {
                if (_baseDbContext.Entry(entity).State == EntityState.Detached)
                    DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
            catch (Exception)
            {
                RefreshEntity(entity);
                throw;
            }
        }

        public void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            var entities = Get(predicate).AsEnumerable();

            foreach (var entity in entities)
                Delete(entity);
        }

        public int SaveChanges()
        {
            return _baseDbContext.SaveChanges();
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return _baseDbContext.SaveChanges(acceptAllChangesOnSuccess);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _baseDbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _baseDbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public virtual void RefreshEntity(T entityToReload)
        {
            _baseDbContext.Entry(entityToReload).Reload();
        }
    }
}