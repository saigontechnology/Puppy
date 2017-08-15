#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IAuditableEntityString.cs </Name>
//         <Created> 15/08/17 9:28:12 AM </Created>
//         <Key> ecabff40-acc6-4aac-9aa8-a0959a574fc0 </Key>
//     </File>
//     <Summary>
//         IAuditableEntityString.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;

namespace Puppy.EF.Interfaces.Entity
{
    /// <summary>
    ///     Audiable entity by CreatedTime and Nullable LastUpdatedTime 
    /// </summary>
    public interface IAuditableEntityString
    {
        DateTimeOffset CreatedTime { get; set; }

        string CreatedBy { get; set; }

        DateTimeOffset? LastUpdatedTime { get; set; }

        string LastUpdatedBy { get; set; }
    }
}