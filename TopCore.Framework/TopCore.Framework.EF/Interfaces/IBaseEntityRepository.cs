#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Topcore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Topcore.WebAPI → Interface </Project>
//     <File>
//         <Name> IBaseEntityRepository.cs </Name>
//         <Created> 23 Apr 17 3:55:08 PM </Created>
//         <Key> b47adcbd-ac4a-4f10-8be1-e391588aafe4 </Key>
//     </File>
//     <Summary>
//         IBaseEntityRepository.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.Linq;
using System.Linq.Expressions;

namespace TopCore.Framework.EF.Interfaces
{
    public interface IBaseEntityRepository
    {
        IQueryable<TEntity> AllInclude<TEntity>(params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class, IBaseEntity;

        IQueryable<TEntity> GetEntity<TEntity>(Expression<Func<TEntity, bool>> predicate = null, bool isIncludeDeleted = false) where TEntity : class, IBaseEntity;

        TEntity GetSingleEntity<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false) where TEntity : class, IBaseEntity;

        TEntity AddEntity<TEntity>(TEntity entity) where TEntity : class, IBaseEntity;

        TEntity UpdateEntity<TEntity>(TEntity entity) where TEntity : class, IBaseEntity;

        void DeleteEntity<TEntity>(TEntity entity, bool isPhysicalDelete = false) where TEntity : class, IBaseEntity;

        void DeleteEntityWhere<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false) where TEntity : class, IBaseEntity;
    }
}