#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EntityStringVersion.cs </Name>
//         <Created> 09/09/17 6:11:20 PM </Created>
//         <Key> 9e92fb48-c9b1-4820-bea1-5fac1d2aa7de </Key>
//     </File>
//     <Summary>
//         EntityStringVersion.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.EF.Interfaces.Entities;

namespace Puppy.EF
{
    /// <inheritdoc cref="EntityString" />
    /// <summary>
    ///     Entity Version String 
    /// </summary>
    public class EntityVersionString : EntityString, IVersionEntity
    {
        public byte[] Version { get; set; }
    }
}