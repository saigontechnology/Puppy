#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EntityVersion.cs </Name>
//         <Created> 09/09/17 6:10:35 PM </Created>
//         <Key> 17e36f96-7304-41e4-8fac-fbeb811f3747 </Key>
//     </File>
//     <Summary>
//         EntityVersion.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.EF.Interfaces.Entities;

namespace Puppy.EF.Entities
{
    /// <inheritdoc cref="Entity{TKey}" />
    /// <summary>
    ///     Entity for Entity Framework 
    /// </summary>
    /// <typeparam name="TKey"> Id type of this entity </typeparam>
    public abstract class EntityVersion<TKey> : Entity<TKey>, IVersionEntity where TKey : struct
    {
        public byte[] Version { get; set; }
    }

    public abstract class EntityVersion : EntityVersion<int>
    {
    }
}