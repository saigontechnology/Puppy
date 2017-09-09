#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IEntity.cs </Name>
//         <Created> 09/09/17 5:53:30 PM </Created>
//         <Key> d0c44f3a-557f-4191-aba4-14f06371c54a </Key>
//     </File>
//     <Summary>
//         IEntity.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Puppy.EF.Interfaces.Repository
{
    public interface IEntityRepository<TEntity, TKey> : IEntityBaseRepository<TEntity> where TEntity : Entity<TKey>, new() where TKey : struct
    {
        TEntity Add(TEntity entity, TKey? createdBy = null);

        List<TEntity> AddRange(TKey? createdBy = null, params TEntity[] listEntity);

        void UpdateWhere(Expression<Func<TEntity, bool>> predicate, TEntity entityNewData, TKey? updatedBy = null, params Expression<Func<TEntity, object>>[] changedProperties);

        void DeleteWhere(Expression<Func<TEntity, bool>> predicate, TKey? deletedBy = null, bool isPhysicalDelete = false);

        void DeleteWhere(List<TKey> listId, TKey? deletedBy = null, bool isPhysicalDelete = false);
    }
}