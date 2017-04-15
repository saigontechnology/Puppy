#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI.Data.EF.Mapping </Project>
//     <File>
//         <Name> UserMapping </Name>
//         <Created> 15/04/2017 08:26:42 AM </Created>
//         <Key> 9c2b2868-92dd-4569-a59d-563a1fb1d1fe </Key>
//     </File>
//     <Summary>
//         UserMapping
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using TopCore.WebAPI.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TopCore.WebAPI.Data.EF.Mapping
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Map(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Version).IsRowVersion();
        }
    }
}