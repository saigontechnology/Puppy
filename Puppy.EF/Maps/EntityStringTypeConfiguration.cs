#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EntityStringTypeConfiguration.cs </Name>
//         <Created> 24/08/17 5:02:32 PM </Created>
//         <Key> 1c6e63f1-0f6d-4cb2-8ba0-fc423354adf0 </Key>
//     </File>
//     <Summary>
//         EntityStringTypeConfiguration.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Puppy.EF.Entities;
using Puppy.EF.Interfaces.Entities;

namespace Puppy.EF.Maps
{
    public abstract class EntityStringTypeConfiguration<TEntity> : ITypeConfiguration<TEntity> where TEntity : EntityString
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

            // Length
            builder.Property(x => x.GlobalId).HasMaxLength(Constants.Maxlength.GlobalId).IsRequired();
        }
    }

    public abstract class EntityStringVersionTypeConfiguration<TEntity> : ITypeConfiguration<TEntity> where TEntity : EntityString, IVersionEntity
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
}