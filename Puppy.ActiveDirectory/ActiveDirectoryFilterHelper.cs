#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ActiveDirectoryFilterHelper.cs </Name>
//         <Created> 03/10/17 11:40:45 PM </Created>
//         <Key> af807638-fb24-4d59-9d09-8ddb38c34afc </Key>
//     </File>
//     <Summary>
//         ActiveDirectoryFilterHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.ActiveDirectory
{
    /// <summary>
    ///     See more filter: https://social.technet.microsoft.com/wiki/contents/articles/5392.active-directory-ldap-syntax-filters.aspx 
    /// </summary>
    public static class ActiveDirectoryFilterHelper
    {
        // User

        public static string GetAllUserFilter() => "(&(objectCategory=person)(objectClass=user))";

        public static string GetUserFilter(string userName)
        {
            return $"({AttributeNameConsts.AccountName}={userName})";
        }

        /// <summary>
        ///     Get all user in group by Distinguished Group Name 
        /// </summary>
        /// <param name="groupDn"> Distinguished Name of Group </param>
        /// <returns></returns>
        public static string GetAllUserInGroup(string groupDn)
        {
            return $"(memberOf={groupDn})";
        }

        public static string GetAllComputerFilter() => "(objectCategory=computer)";

        public static string GetAllContactFilter() => "(objectClass=contact)";

        // Group

        public static string GetAllGroupFilter() => "(objectCategory=group)";

        public static string GetGroupFilter(string groupName)
        {
            return $"(&(objectCategory=group)(cn={groupName}))";
        }
    }
}