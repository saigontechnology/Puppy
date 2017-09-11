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
using Puppy.EF.Interfaces.Repositories;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Puppy.EF.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly IBaseDbContext BaseDbContext;

        private DbSet<T> _dbSet;

        protected DbSet<T> DbSet
        {
            get
            {
                if (_dbSet != null)
                    return _dbSet;
                _dbSet = BaseDbContext.Set<T>();
                return _dbSet;
            }
        }

        protected Repository(IBaseDbContext baseDbContext)
        {
            BaseDbContext = baseDbContext;
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

            includeProperties = includeProperties?.Distinct().ToArray();

            if (includeProperties?.Any() == true)
            {
                foreach (var includeProperty in includeProperties)
                    query = query.Include(includeProperty);
            }

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
            TryToAttach(entity);

            changedProperties = changedProperties?.Distinct().ToArray();

            if (changedProperties?.Any() == true)
            {
                foreach (var property in changedProperties)
                {
                    BaseDbContext.Entry(entity).Property(property).IsModified = true;
                }
            }
            else
                BaseDbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            try
            {
                TryToAttach(entity);

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
            BaseDbContext.Entry(entity).Reload();
        }

        public virtual bool TryToAttach(T entity)
        {
            try
            {
                if (BaseDbContext.Entry(entity).State == EntityState.Detached)
                    DbSet.Attach(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual int SaveChanges()
        {
            return BaseDbContext.SaveChanges();
        }

        public virtual int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return BaseDbContext.SaveChanges(acceptAllChangesOnSuccess);
        }

        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return BaseDbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            return BaseDbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}