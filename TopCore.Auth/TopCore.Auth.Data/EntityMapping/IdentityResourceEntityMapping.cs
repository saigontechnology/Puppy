using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TopCore.Auth.Domain.Entities;

namespace TopCore.Auth.Data.EntityMapping
{
    public class IdentityResourceEntityMapping : EntityTypeConfiguration<IdentityResourceEntity>
    {
        public override void Map(EntityTypeBuilder<IdentityResourceEntity> builder)
        {
            builder.ToTable(nameof(IdentityResourceEntityMapping));
        }
    }
}