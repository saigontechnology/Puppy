#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Framework.EF </Project>
//     <File>
//         <Name> IdentityUserEntityBase </Name>
//         <Created> 06 Apr 17 2:44:21 PM </Created>
//         <Key> a59cdd15-3d2d-480b-a1d9-52c7e787a6ef </Key>
//     </File>
//     <Summary>
//         IdentityUserEntityBase
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace TopCore.Framework.EF
{
    public abstract class IdentityUserEntityBase : IdentityUser
    {
        protected IdentityUserEntityBase()
        {
        }

        protected IdentityUserEntityBase(string userName) : base(userName)
        {
        }

        public Guid Key { get; set; } = Guid.NewGuid();

        public bool IsDeleted { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? LastUpdatedOnUtc { get; set; }

        public DateTime? DeletedOnUtc { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }
}