#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> Repository.cs </Name>
//         <Created> 23 Apr 17 11:07:18 PM </Created>
//         <Key> 88fc9749-b1a3-401a-946c-519b4e3f7241 </Key>
//     </File>
//     <Summary>
//         Repository.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using TopCore.Auth.Domain.Interfaces.Data;
using TopCore.Framework.DependencyInjection.Attributes;
using TopCore.Framework.EF;

namespace TopCore.Auth.Data
{
    [PerRequestDependency(ServiceType = typeof(IRepository<>))]
    public class Repository<T> : BaseRepository<T>, IRepository<T> where T : class
    {
        private readonly IDbContext _dbContext;

        public Repository(IDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}