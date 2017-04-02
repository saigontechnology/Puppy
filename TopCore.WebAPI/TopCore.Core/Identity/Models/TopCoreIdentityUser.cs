#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Core.Identity.Models </Project>
//     <File>
//         <Name> TopCoreIdentityUser </Name>
//         <Created> 02 Apr 17 11:17:49 PM </Created>
//         <Key> 3506fdf0-366f-494b-8333-9831004a3dac </Key>
//     </File>
//     <Summary>
//         TopCoreIdentityUser
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace TopCore.Core.Identity.Models
{
    public class TopCoreIdentityUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
        public string DataEventRecordsRole { get; set; }
        public string SecuredFilesRole { get; set; }
        public DateTime AccountExpires { get; set; }
    }
}