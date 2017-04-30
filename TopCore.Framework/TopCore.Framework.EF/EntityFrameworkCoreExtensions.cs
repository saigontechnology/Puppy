#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Framework.EF </Project>
//     <File>
//         <Name> EntityFrameworkCoreExtensions </Name>
//         <Created> 06 Apr 17 2:43:16 AM </Created>
//         <Key> 256b95f5-6d1d-403d-9502-b55e1560f92d </Key>
//     </File>
//     <Summary>
//         Keep namespace Microsoft.EntityFrameworkCore for resolve the conflict or don't met it.
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
	public abstract class EntityTypeConfiguration<TEntity> where TEntity : class
	{
		public abstract void Map(EntityTypeBuilder<TEntity> builder);
	}

	public static class ModelBuilderExtensions
	{
		public static void AddConfiguration<TEntity>(this ModelBuilder modelBuilder,
			EntityTypeConfiguration<TEntity> configuration) where TEntity : class
		{
			configuration.Map(modelBuilder.Entity<TEntity>());
		}
	}
}