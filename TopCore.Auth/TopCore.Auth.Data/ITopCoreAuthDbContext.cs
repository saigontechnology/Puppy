#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Data </Project>
//     <File>
//         <Name> ITopCoreAuthDbContext </Name>
//         <Created> 05 Apr 17 12:38:49 AM </Created>
//         <Key> e78cd1df-82a0-4ec6-bcbc-9b9f16ea8f25 </Key>
//     </File>
//     <Summary>
//         ITopCoreAuthDbContext
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.EntityFrameworkCore;
using TopCore.Auth.Data.Entity;

namespace TopCore.Auth.Data
{
    public interface ITopCoreAuthDbContext
    {
        DbSet<EntityBase> Users { get; set; }

        void OnModelCreating(ModelBuilder builder);
    }
}