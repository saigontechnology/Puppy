#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Repository.cs </Name>
//         <Created> 23 Apr 17 3:57:10 PM </Created>
//         <Key> 3b818713-4d97-47d0-8844-ec124b27f1e0 </Key>
//     </File>
//     <Summary>
//         Repository.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.EntityFrameworkCore;
using Puppy.EF.Interfaces;
using Puppy.EF.Interfaces.Repository;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Puppy.EF
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private readonly IBaseDbContext _baseDbContext;

        private DbSet<T> _dbSet;

        protected DbSet<T> DbSet
        {
            get
            {
                if (_dbSet != null)
                    return _dbSet;
                _dbSet = _baseDbContext.Set<T>();
                return _dbSet;
            }
        }

        protected Repository(IBaseDbContext baseDbContext)
        {
            _baseDbContext = baseDbContext;
        }

        public virtual IQueryable<T> Include(params Expression<Func<T, object>>[] includeProperties)
        {
            var query = DbSet.AsNoTracking();
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);
            return query;
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = DbSet.AsNoTracking();
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);
            return predicate == null ? query : query.Where(predicate);
        }

        public virtual T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return Get(predicate, includeProperties).FirstOrDefault();
        }

        public virtual T Add(T entity)
        {
            entity = DbSet.Add(entity).Entity;
            return entity;
        }

        public virtual void Update(T entity, params Expression<Func<T, object>>[] changedProperties)
        {
            DbSet.Attach(entity);

            if (changedProperties != null && changedProperties.Any())
                foreach (var property in changedProperties)
                {
                    _baseDbContext.Entry(entity).Property(property).IsModified = true;
                }
            else
                _baseDbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
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

        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            var entities = Get(predicate).AsEnumerable();

            foreach (var entity in entities)
                Delete(entity);
        }

        public virtual void RefreshEntity(T entity)
        {
            _baseDbContext.Entry(entity).Reload();
        }

        [DebuggerStepThrough]
        public virtual int SaveChanges()
        {
            return _baseDbContext.SaveChanges();
        }

        [DebuggerStepThrough]
        public virtual int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return _baseDbContext.SaveChanges(acceptAllChangesOnSuccess);
        }

        [DebuggerStepThrough]
        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _baseDbContext.SaveChangesAsync(cancellationToken);
        }

        [DebuggerStepThrough]
        public virtual Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            return _baseDbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}