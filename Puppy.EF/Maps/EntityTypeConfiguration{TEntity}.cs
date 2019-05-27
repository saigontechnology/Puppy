#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EntityTypeConfiguration.cs </Name>
//         <Created> 24/08/17 4:36:20 PM </Created>
//         <Key> 0040d619-71b4-4f7d-9913-0f9a080831de </Key>
//     </File>
//     <Summary>
//         EntityTypeConfiguration.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Puppy.EF.Entities;
using Puppy.EF.Interfaces.Entities;

namespace Puppy.EF.Maps
{
    public abstract class EntityTypeConfiguration<TEntity, TKey> : ITypeConfiguration<TEntity> where TEntity : Entity<TKey> where TKey : struct
    {
        public virtual void Map(EntityTypeBuilder<TEntity> builder)
        {
            // Key
            builder.HasKey(x => x.Id);

            // Index
            builder.HasIndex(x => x.Id);
            builder.HasIndex(x => x.GlobalId);
            builder.HasIndex(x => x.DeletedTime);

            // NOTE: Don't use Query Filter, it affect to load data business logic
            // Filter
            //builder.HasQueryFilter(x => x.DeletedTime == null);

            // Length
            builder.Property(x => x.GlobalId).HasMaxLength(Constants.Maxlength.GlobalId).IsRequired();

            builder.HasQueryFilter(x => x.DeletedTime == null);
        }
    }

    public abstract class EntityVersionTypeConfiguration<TEntity, TKey> : ITypeConfiguration<TEntity> where TEntity : Entity<TKey>, IVersionEntity where TKey : struct
    {
        public virtual void Map(EntityTypeBuilder<TEntity> builder)
        {
            // Key
            builder.HasKey(x => x.Id);

            // Index
            builder.HasIndex(x => x.Id);
            builder.HasIndex(x => x.GlobalId);
            builder.HasIndex(x => x.DeletedTime);

            // Filter
            builder.HasQueryFilter(x => x.DeletedTime == null);

            // Version
            builder.Property(x => x.Version).IsRowVersion();

            // Length
            builder.Property(x => x.GlobalId).HasMaxLength(Constants.Maxlength.GlobalId).IsRequired();
        }
    }

    public abstract class EntityTypeConfiguration<TEntity> : EntityTypeConfiguration<TEntity, int> where TEntity : Entity
    {
    }

    public abstract class EntityVersionTypeConfiguration<TEntity> : EntityVersionTypeConfiguration<TEntity, int> where TEntity : Entity, IVersionEntity
    {
    }
}