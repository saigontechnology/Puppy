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

namespace Puppy.EF.Interfaces.Entity
{
    /// <summary>
    ///     Audiable entity by CreatedTime and Nullable LastUpdatedTime 
    /// </summary>
    /// <typeparam name="TCreatedTime">Refer use <c> DateTimeOffset </c></typeparam>
    /// <typeparam name="TLastUpdatedTime"> Refer use <c> DateTimeOffset </c> </typeparam>
    /// <remarks> Refer use <c> DateTimeOffset </c> for both </remarks>
    public interface IAuditableEntity<TCreatedTime, TLastUpdatedTime> where TCreatedTime : struct where TLastUpdatedTime : struct
    {
        TCreatedTime CreatedTime { get; set; }

        TLastUpdatedTime? LastUpdatedTime { get; set; }
    }
}