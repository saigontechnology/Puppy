#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI.Data.EF.Mapping </Project>
//     <File>
//         <Name> IEntityTypeConfiguration </Name>
//         <Created> 15/04/2017 08:47:13 AM </Created>
//         <Key> c6eb49ec-b589-49e5-84c0-a6726e200c08 </Key>
//     </File>
//     <Summary>
//         IEntityTypeConfiguration
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TopCore.WebAPI.Data.EF.Mapping
{
    public interface IEntityTypeConfiguration<TEntity> where TEntity : class
    {
        void Map(EntityTypeBuilder<TEntity> builder);
    }
}