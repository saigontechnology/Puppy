using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TopCore.Auth.Domain.Entities;

namespace TopCore.Auth.Data.EntityMapping
{
    public class UserEntityMapping : EntityTypeConfiguration<User>
    {
        public override void Map(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(UserEntityMapping));
            builder.Property(x => x.Version).IsRowVersion();
        }
    }
}