#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IGlobalIdentityEntity.cs </Name>
//         <Created> 31/07/17 9:14:17 AM </Created>
//         <Key> dbe4c0ac-df95-4819-9f38-1d918c1edb01 </Key>
//     </File>
//     <Summary>
//         IGlobalIdentityEntity.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.EF.Interfaces.Entity
{
    /// <summary>
    ///     Refer use <c> Guid.NewGuid().ToString("N") </c> to generate <c> GlobalId </c> in constructor 
    /// </summary>
    public interface IGlobalIdentityEntity
    {
        string GlobalId { get; set; }
    }
}