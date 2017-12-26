#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IAuditableEntity.cs </Name>
//         <Created> 31/07/17 9:06:10 AM </Created>
//         <Key> 77d18bfa-8844-42a6-8bab-8e607419fbf5 </Key>
//     </File>
//     <Summary>
//         IAuditableEntity.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;

namespace Puppy.EF.Interfaces.Entities
{
    public interface IAuditableEntity
    {
        DateTimeOffset CreatedTime { get; set; }

        DateTimeOffset LastUpdatedTime { get; set; }
    }

    /// <summary>
    ///     Audiable entity by CreatedTime and Nullable LastUpdatedTime 
    /// </summary>
    /// <typeparam name="TKey"> User Id </typeparam>
    public interface IAuditableEntity<TKey> : IAuditableEntity where TKey : struct
    {
        TKey? CreatedBy { get; set; }

        TKey? LastUpdatedBy { get; set; }
    }

    /// <summary>
    ///     Audiable entity by CreatedTime and Nullable LastUpdatedTime 
    /// </summary>
    public interface IAuditableEntityString : IAuditableEntity
    {
        string CreatedBy { get; set; }

        string LastUpdatedBy { get; set; }
    }
}