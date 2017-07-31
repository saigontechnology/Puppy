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

namespace Puppy.EF.Interfaces.Entity
{
    /// <summary>
    /// Soft Deletable Entity by IsDeleted as marker and Nullable TDeletedTime as audit log
    /// </summary>
    /// <typeparam name="TIsDeleted">Refer use <c>Boolean</c></typeparam>
    /// <typeparam name="TDeletedTime">Refer use <c> DateTimeOffset </c></typeparam>
    /// <remarks>Refer use <c>Boolean</c> for <c>TIsDeleted</c> and <c> DateTimeOffset </c> for <c>TDeletedTime</c></remarks>
    public interface ISoftDeletableEntity<TIsDeleted, TDeletedTime> where TIsDeleted : struct where TDeletedTime : struct
    {
        TIsDeleted IsDeleted { get; set; }

        TDeletedTime? DeletedTime { get; set; }
    }
}