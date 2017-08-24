#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> TypeConfiguration.cs </Name>
//         <Created> 31/07/17 9:28:22 AM </Created>
//         <Key> 8e9298f2-098b-4f95-97d3-16d055e35d3e </Key>
//     </File>
//     <Summary>
//         TypeConfiguration.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Puppy.EF.Mapping
{
    public abstract class TypeConfiguration<T> : ITypeConfiguration<T> where T : class
    {
        public virtual void Map(EntityTypeBuilder<T> builder)
        {
        }
    }
}