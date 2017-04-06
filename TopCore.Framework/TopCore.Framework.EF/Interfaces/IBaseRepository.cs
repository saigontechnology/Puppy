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
using System.Linq;
using System.Linq.Expressions;

namespace TopCore.Framework.EF.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        bool Any(Expression<Func<TEntity, bool>> predicate);

        int Count(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);

        TEntity Add(TEntity entity);

        ICollection<TEntity> AddRange(ICollection<TEntity> entities);

        TEntity Update(TEntity entity);

        ICollection<TEntity> UpdateRange(ICollection<TEntity> entities);

        TEntity Delete(TEntity entity);

        ICollection<TEntity> DeleteRange(ICollection<TEntity> entities);
    }
}