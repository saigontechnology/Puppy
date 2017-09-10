#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EntityBase.cs </Name>
//         <Created> 09/09/17 6:14:26 PM </Created>
//         <Key> cb094aa8-4c53-4514-8baa-9e5812c1bc42 </Key>
//     </File>
//     <Summary>
//         EntityBase.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.EF.Interfaces.Entities;
using System;

namespace Puppy.EF
{
    public abstract class EntityBase : IGlobalIdentityEntity, ISoftDeletableEntity, IAuditableEntity
    {
        public virtual string GlobalId { get; set; } = Guid.NewGuid().ToString("N");

        public virtual DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;

        public virtual DateTimeOffset? LastUpdatedTime { get; set; }

        public virtual DateTimeOffset? DeletedTime { get; set; }
    }
}