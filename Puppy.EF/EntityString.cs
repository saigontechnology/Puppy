#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EntityString.cs </Name>
//         <Created> 09/09/17 6:07:47 PM </Created>
//         <Key> ab8e293f-e553-4e58-bf60-60903d3510b7 </Key>
//     </File>
//     <Summary>
//         EntityString.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.EF.Interfaces.Entities;

namespace Puppy.EF
{
    public class EntityString : EntityBase, ISoftDeletableEntityString, IAuditableEntityString
    {
        public virtual string Id { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual string LastUpdatedBy { get; set; }

        public virtual string DeletedBy { get; set; }
    }
}