#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EntityString.cs </Name>
//         <Created> 15/08/17 9:30:01 AM </Created>
//         <Key> cf93f9e5-1bb4-4c2a-b5ad-8aa9aace4e6a </Key>
//     </File>
//     <Summary>
//         EntityString.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.EF.Interfaces.Entity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Puppy.EF
{
    public class EntityString : ISoftDeletableEntityString, IAuditableEntityString, IVersionEntity, IGlobalIdentityEntity
    {
        public virtual string GlobalId { get; set; } = Guid.NewGuid().ToString("N");

        [Key]
        public virtual string Id { get; set; }

        [Timestamp]
        public virtual byte[] Version { get; set; }

        // Create Info

        public virtual string CreatedBy { get; set; }

        public virtual DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;

        // Update Info

        public virtual string LastUpdatedBy { get; set; }

        public virtual DateTimeOffset? LastUpdatedTime { get; set; }

        // Delete Info

        public virtual bool IsDeleted { get; set; }

        public virtual string DeletedBy { get; set; }

        public virtual DateTimeOffset? DeletedTime { get; set; }
    }
}