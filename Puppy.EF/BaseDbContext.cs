#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> Puppy.EF </Project>
//     <File>
//         <Name> BaseDbContext </Name>
//         <Created> 06 Apr 17 1:09:03 AM </Created>
//         <Key> 698007b1-38ad-45b0-b7d0-f22069fe2fab </Key>
//     </File>
//     <Summary>
//         BaseDbContext
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.EntityFrameworkCore;
using Puppy.EF.Interfaces;

namespace Puppy.EF
{
    public class BaseDbContext : DbContext, IBaseDbContext
    {
        protected BaseDbContext()
        {
        }

        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}