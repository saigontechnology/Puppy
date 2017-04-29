#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Topcore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Topcore.WebAPI → Interface </Project>
//     <File>
//         <Name> IBaseRepository.cs </Name>
//         <Created> 23 Apr 17 3:47:20 PM </Created>
//         <Key> 3e4a5d95-7957-4305-ad6f-b4452d8ccfda </Key>
//     </File>
//     <Summary>
//         IBaseRepository.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.Linq;
using System.Linq.Expressions;

namespace TopCore.Framework.EF.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        IQueryable<T> Include(params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> Get(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);

        T GetSingle(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);

        T Add(T entity);

        T Update(T entity);

        void Delete(T entity);

        void DeleteWhere(Expression<Func<T, bool>> predicate);
    }
}