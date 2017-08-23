#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> LogEntityMap.cs </Name>
//         <Created> 23/08/17 10:15:52 AM </Created>
//         <Key> b067bfc2-29a6-4cf7-a414-0644c26edef2 </Key>
//     </File>
//     <Summary>
//         LogEntityMap.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Puppy.EF.Mapping;
using Puppy.Logger.Core.Models;

namespace Puppy.Logger.SQLite.Map
{
    public class LogEntityMap : IEntityTypeConfiguration<LogEntity>
    {
        public void Map(EntityTypeBuilder<LogEntity> builder)
        {
            builder.ToTable(nameof(LogEntity));
            builder.HasKey(x => x.Id);
            builder.Ignore(x => x.Exception).Ignore(x => x.HttpContext);

            builder.HasIndex(x => x.Id);
            builder.HasIndex(x => x.CreatedTime);
            builder.HasIndex(x => x.Level);
        }
    }
}