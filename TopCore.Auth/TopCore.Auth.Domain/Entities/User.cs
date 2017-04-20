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
        public DateTime CreatedOnUtc { get; set; }

        public DateTime? LastUpdatedOnUtc { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOnUtc { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }
}