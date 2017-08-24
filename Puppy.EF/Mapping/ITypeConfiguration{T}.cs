#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> Puppy.EF </Project>
//     <File>
//         <Name> ITypeConfiguration </Name>
//         <Created> 15/04/2017 08:47:13 AM </Created>
//         <Key> c6eb49ec-b589-49e5-84c0-a6726e200c08 </Key>
//     </File>
//     <Summary>
//         ITypeConfiguration
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Puppy.EF.Mapping
{
    public interface ITypeConfiguration<T> where T : class
    {
        void Map(EntityTypeBuilder<T> builder);
    }
}