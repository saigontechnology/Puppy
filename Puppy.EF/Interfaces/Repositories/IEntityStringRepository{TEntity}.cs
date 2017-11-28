#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IEntityString.cs </Name>
//         <Created> 09/09/17 6:05:42 PM </Created>
//         <Key> dae04cd9-3347-4168-b32f-dd47afa5b60e </Key>
//     </File>
//     <Summary>
//         IEntityString.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Puppy.EF.Interfaces.Repositories
{
    public interface IEntityStringRepository<TEntity> : IEntityBaseRepository<TEntity> where TEntity : EntityString, new()
    {
        void UpdateWhere(Expression<Func<TEntity, bool>> predicate, TEntity entityData, params Expression<Func<TEntity, object>>[] changedProperties);

        void UpdateWhere(Expression<Func<TEntity, bool>> predicate, TEntity entityNewData, params string[] changedProperties);

        void DeleteWhere(List<string> listId, bool isPhysicalDelete = false);
    }
}