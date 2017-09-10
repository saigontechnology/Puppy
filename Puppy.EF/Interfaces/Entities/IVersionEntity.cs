#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IVersionEntity.cs </Name>
//         <Created> 31/07/17 9:12:39 AM </Created>
//         <Key> e99503dd-e7b9-4f92-8eac-940d0321e609 </Key>
//     </File>
//     <Summary>
//         IVersionEntity.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.ComponentModel.DataAnnotations;

namespace Puppy.EF.Interfaces.Entities
{
    /// <summary>
    ///     Use version as <c> byte[] </c> with database type is <c> Timestamp </c> to resolve
    ///     concurrency issue.
    /// </summary>
    public interface IVersionEntity
    {
        [Timestamp]
        byte[] Version { get; set; }
    }
}