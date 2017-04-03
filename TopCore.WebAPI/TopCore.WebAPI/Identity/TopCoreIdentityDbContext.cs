#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Core.Identity </Project>
//     <File>
//         <Name> TopCoreIdentityDbContext </Name>
//         <Created> 02 Apr 17 11:18:20 PM </Created>
//         <Key> baade8f1-3fc7-470d-9edf-51a474a3d681 </Key>
//     </File>
//     <Summary>
//         TopCoreIdentityDbContext
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TopCore.WebAPI.Identity
{
    public class TopCoreIdentityDbContext : IdentityDbContext
    {
        public TopCoreIdentityDbContext(DbContextOptions<TopCoreIdentityDbContext> options) : base(options)
        {
        }
    }
}