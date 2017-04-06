using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TopCore.Auth.Domain.Entities;

namespace TopCore.Auth.Data.EntityMapping
{
    public class ClientEntityMapping : EntityTypeConfiguration<ClientEntity>
    {
        public override void Map(EntityTypeBuilder<ClientEntity> builder)
        {
            builder.ToTable(nameof(ClientEntityMapping));
        }
    }
}