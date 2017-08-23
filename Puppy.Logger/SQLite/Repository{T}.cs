#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Repository.cs </Name>
//         <Created> 23/08/17 5:58:04 PM </Created>
//         <Key> eaeaf865-5a0a-4de4-b8c4-ed035e2b0500 </Key>
//     </File>
//     <Summary>
//         Repository.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.EF.Interfaces;

namespace Puppy.Logger.SQLite
{
    public class Repository<T> : EF.Repository<T> where T : class
    {
        public Repository(IBaseDbContext baseDbContext) : base(baseDbContext)
        {
        }
    }
}