using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TopCore.Auth.Domain.Entities;

namespace TopCore.Auth.Data.EntityMapping
{
    public class UserEntityMapping : EntityTypeConfiguration<UserEntity>
    {
        public override void Map(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable(nameof(UserEntityMapping));
            builder.HasKey(x => x.Id);
        }
    }
}