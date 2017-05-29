#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI </Project>
//     <File>
//         <Name> OtpTrackingMapping.cs </Name>
//         <Created> 29/05/2017 4:10:21 PM </Created>
//         <Key> 4f5e8a29-c497-48bb-96ac-dd1a25580573 </Key>
//     </File>
//     <Summary>
//         OtpTrackingMapping.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TopCore.Auth.Domain.Entities;
using TopCore.Framework.EF.Mapping;

namespace TopCore.Auth.Data.Mapping
{
    public class OtpTrackingMapping : IEntityTypeConfiguration<OtpTracking>
    {
        public void Map(EntityTypeBuilder<OtpTracking> builder)
        {
            builder.ToTable(nameof(OtpTracking));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Version).IsRowVersion();
        }
    }
}