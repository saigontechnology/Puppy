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
using Puppy.EF.Interfaces.Entity;

namespace Puppy.EF.Mapping
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
        }
    }

    public abstract class EntityTypeConfiguration<TEntity> : EntityTypeConfiguration<TEntity, int> where TEntity : Entity
    {
    }

    public abstract class EntityVersionTypeConfiguration<TEntity, TKey> : EntityTypeConfiguration<TEntity, TKey> where TEntity : Entity<TKey>, IVersionEntity where TKey : struct
    {
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            base.Map(builder);

            // Version
            builder.Property(x => x.Version).IsRowVersion();
        }
    }
}