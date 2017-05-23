#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Domain.Entity </Project>
//     <File>
//         <Name> User </Name>
//         <Created> 06 Apr 17 1:07:54 AM </Created>
//         <Key> 1bf15074-54a1-4feb-ac8a-13141334a86d </Key>
//     </File>
//     <Summary>
//         User
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using TopCore.Framework.EF;

namespace TopCore.Auth.Domain.Entities
{
    public class User : IdentityUser, IBaseEntity
    {
        public DateTimeOffset? DeletedOnUtc { get; set; }
        public string GlobalId { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public DateTimeOffset? LastUpdatedTime { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset? DeletedTime { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }
}