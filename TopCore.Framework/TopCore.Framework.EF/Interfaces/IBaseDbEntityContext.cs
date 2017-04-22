#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> IBaseDbEntityContext.cs </Name>
//         <Created> 20 Apr 17 9:56:25 AM </Created>
//         <Key> 9376890c-9bf2-4b49-b7fd-3915e2934240 </Key>
//     </File>
//     <Summary>
//         IBaseDbEntityContext.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TopCore.Framework.EF.Interfaces
{
    public interface IBaseDbEntityContext
    {
        #region Get

        IQueryable<TEntity> GetEntity<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false) where TEntity : class, IBaseEntity;

        IQueryable<TEntity> GetEntity<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class, IBaseEntity;

        TEntity GetSingleEntity<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false) where TEntity : class, IBaseEntity;

        Task<TEntity> GetSingleEntityAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false) where TEntity : class, IBaseEntity;

        TEntity GetSingleEntity<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class, IBaseEntity;

        Task<TEntity> GetSingleEntityAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class, IBaseEntity;

        #endregion

        #region Delete

        void DeleteEntityWhere<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false) where TEntity : class, IBaseEntity;

        void DeleteEntity<TEntity>(TEntity entity, bool isPhysicalDelete = false) where TEntity : class, IBaseEntity;

        void DeleteEntityRange<TEntity>(IEnumerable<TEntity> entities, bool isPhysicalDelete = false) where TEntity : class, IBaseEntity;

        void DeleteEntityRange<TEntity>(bool isPhysicalDelete = false, params TEntity[] entities) where TEntity : class, IBaseEntity;

        #endregion
    }
}