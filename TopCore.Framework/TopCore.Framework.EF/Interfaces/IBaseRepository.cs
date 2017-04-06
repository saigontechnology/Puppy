#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Framework.EF.Interfaces </Project>
//     <File>
//         <Name> IBaseRepository </Name>
//         <Created> 06 Apr 17 1:10:50 PM </Created>
//         <Key> 555866df-387c-45fc-9832-32b572d932eb </Key>
//     </File>
//     <Summary>
//         IBaseRepository
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TopCore.Framework.EF.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        ///     Get by key guid 
        /// </summary>
        /// <param name="key">             </param>
        /// <param name="isIncludeDeleted"></param>
        /// <returns></returns>
        TEntity Get(Guid key, bool isIncludeDeleted = false);

        /// <summary>
        ///     Get by key guid string 
        /// </summary>
        /// <param name="key">             </param>
        /// <param name="isIncludeDeleted"></param>
        /// <returns></returns>
        TEntity Get(string key, bool isIncludeDeleted = false);

        IEnumerable<TEntity> GetAll(bool isIncludeDeleted = false);

        IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false);

        void Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void UpdateRange(IEnumerable<TEntity> entities);

        void Delete(TEntity entity, bool isPhysicalDiskDelete = false);

        void DeleteRange(IEnumerable<TEntity> entities, bool isPhysicalDiskDelete = false);
    }
}