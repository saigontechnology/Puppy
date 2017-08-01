#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> ISoftDeletableEntity.cs </Name>
//         <Created> 31/07/17 8:55:56 AM </Created>
//         <Key> 8889b570-7288-4c08-a39d-1b3ce07c4570 </Key>
//     </File>
//     <Summary>
//         ISoftDeletableEntity.cs
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
    /// <typeparam name="TKey"> User Id </typeparam>
    public interface ISoftDeletableEntity<TKey> where TKey : struct
    {
        bool IsDeleted { get; set; }

        DateTimeOffset? DeletedTime { get; set; }

        TKey? DeletedBy { get; set; }
    }
}