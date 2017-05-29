using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TopCore.Auth.Domain.Entities;
using TopCore.Framework.EF.Mapping;

namespace TopCore.Auth.Data.Mapping
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Map(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));
            builder.Property(x => x.Version).IsRowVersion();
        }
    }
}