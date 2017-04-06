using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TopCore.Auth.Domain.Entities;

namespace TopCore.Auth.Data.EntityMapping
{
    public class ApiResourceEntityMapping : EntityTypeConfiguration<ApiResourceEntity>
    {
        public override void Map(EntityTypeBuilder<ApiResourceEntity> builder)
        {
            builder.ToTable(nameof(ApiResourceEntityMapping));
        }
    }
}