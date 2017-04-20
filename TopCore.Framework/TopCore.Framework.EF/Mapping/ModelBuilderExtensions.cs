#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Framework.EF </Project>
//     <File>
//         <Name> ModelBuilderExtensions </Name>
//         <Created> 15/04/2017 08:31:59 AM </Created>
//         <Key> 2b234db0-bc6c-476e-afeb-580ec40bc9f2 </Key>
//     </File>
//     <Summary>
//         ModelBuilderExtensions
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace TopCore.Framework.EF.Mapping
{
    public static class ModelBuilderExtensions
    {
        public static void AddConfigFromAssembly(this ModelBuilder builder, Assembly assembly)
        {
            // Types that do entity mapping
            var mappingTypes = assembly.GetTypes()
                .Where(x => x.GetInterfaces()
                    .Any(y => y.GetTypeInfo().IsGenericType
                              && y.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            // Get the generic Entity method of the ModelBuilder type
            var entityMethod = typeof(ModelBuilder).GetMethods()
                .Single(x => x.Name == "Entity" &&
                             x.IsGenericMethod &&
                             x.ReturnType.Name == "EntityTypeBuilder`1");

            foreach (var mappingType in mappingTypes)
            {
                // Get the type of entity to be mapped
                var genericTypeArg = mappingType.GetInterfaces().Single().GenericTypeArguments.Single();

                // Get the method builder.Entity<TEntity>
                var genericEntityMethod = entityMethod.MakeGenericMethod(genericTypeArg);

                // Invoke builder.Entity<TEntity> to get a builder for the entity to be mapped
                var entityBuilder = genericEntityMethod.Invoke(builder, null);

                // Create the mapping type and do the mapping
                var mapper = Activator.CreateInstance(mappingType);
                mapper.GetType().GetMethod("Map").Invoke(mapper, new[] { entityBuilder });
            }
        }
    }
}