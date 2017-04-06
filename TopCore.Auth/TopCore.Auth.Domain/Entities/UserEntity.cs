#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Domain.Entity </Project>
//     <File>
//         <Name> UserEntity </Name>
//         <Created> 06 Apr 17 1:07:54 AM </Created>
//         <Key> 1bf15074-54a1-4feb-ac8a-13141334a86d </Key>
//     </File>
//     <Summary>
//         UserEntity
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using TopCore.Framework.EF;

namespace TopCore.Auth.Domain.Entities
{
    public class UserEntity : IdentityUserEntityBase
    {
        public UserEntity()
        {
        }

        public UserEntity(string userName) : base(userName)
        {
        }
    }
}