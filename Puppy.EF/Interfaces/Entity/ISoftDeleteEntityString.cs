#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> ISoftDeleteEntityString.cs </Name>
//         <Created> 15/08/17 9:29:05 AM </Created>
//         <Key> b69bf7fe-252b-4e1c-b5c8-d4291190bde0 </Key>
//     </File>
//     <Summary>
//         ISoftDeleteEntityString.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;

namespace Puppy.EF.Interfaces.Entity
{
    /// <summary>
    ///     Soft Deletable Entity by IsDeleted as marker and Nullable DateTimeOffset as audit log 
    /// </summary>
    public interface ISoftDeletableEntityString
    {
        bool IsDeleted { get; set; }

        DateTimeOffset? DeletedTime { get; set; }

        string DeletedBy { get; set; }
    }
}