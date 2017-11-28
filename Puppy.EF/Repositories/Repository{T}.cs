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
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    public abstract class Repository<T> : IRepository<T> where T : class, new()
    {
        protected readonly IBaseDbContext DbContext;

        private DbSet<T> _dbSet;

        protected DbSet<T> DbSet
        {
            get
            {
                if (_dbSet != null)
                {
                    return _dbSet;
                }

                _dbSet = DbContext.Set<T>();

                return _dbSet;
            }
        }

        protected Repository(IBaseDbContext dbContext)
        {
            DbContext = dbContext;
        }

        protected void SplitEntityEntry(out List<EntityEntry> listEntryAdded, out List<EntityEntry> listEntryModified, out List<EntityEntry> listEntryDeleted)
        {
            var listState = new List<EntityState>
            {
                EntityState.Added,
                EntityState.Modified,
                EntityState.Deleted
            };

            var listEntry = DbContext.ChangeTracker.Entries()
                .Where(x => x.Entity is T && listState.Contains(x.State))
                .ToList();

            listEntryAdded = listEntry.Where(x => x.State == EntityState.Added).ToList();
            listEntryModified = listEntry.Where(x => x.State == EntityState.Modified).ToList();
            listEntryDeleted = listEntry.Where(x => x.State == EntityState.Deleted).ToList();
        }

        protected void SplitEntity(out List<T> listEntityAdded, out List<T> listEntityModified, out List<T> listEntityDeleted)
        {
            var listState = new List<EntityState>
            {
                EntityState.Added,
                EntityState.Modified,
                EntityState.Deleted
            };

            var listEntry = DbContext.ChangeTracker.Entries()
                .Where(x => x.Entity is T && listState.Contains(x.State))
                .ToList();

            listEntityAdded = listEntry.Where(x => x.State == EntityState.Added).Select(x => x.Entity).Cast<T>().ToList();
            listEntityModified = listEntry.Where(x => x.State == EntityState.Modified).Select(x => x.Entity).Cast<T>().ToList();
            listEntityDeleted = listEntry.Where(x => x.State == EntityState.Deleted).Select(x => x.Entity).Cast<T>().ToList();
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
            TryAttach(entity);

            changedProperties = changedProperties?.Distinct().ToArray();

            if (changedProperties?.Any() == true)
            {
                foreach (var property in changedProperties)
                {
                    DbContext.Entry(entity).Property(property).IsModified = true;
                }
            }
            else
                DbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Update(T entity, params string[] changedProperties)
        {
            TryAttach(entity);

            changedProperties = changedProperties?.Distinct().ToArray();

            if (changedProperties?.Any() == true)
            {
                foreach (var property in changedProperties)
                {
                    DbContext.Entry(entity).Property(property).IsModified = true;
                }
            }
            else
                DbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Update(T entity)
        {
            TryAttach(entity);

            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            try
            {
                TryAttach(entity);

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
            DbContext.Entry(entity).Reload();
        }

        public virtual bool TryAttach(T entity)
        {
            try
            {
                if (DbContext.Entry(entity).State == EntityState.Detached)
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
            return DbContext.SaveChanges();
        }

        public virtual int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return DbContext.SaveChanges(acceptAllChangesOnSuccess);
        }

        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return DbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            return DbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}