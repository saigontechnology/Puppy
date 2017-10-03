#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> AuthenticationHelper.cs </Name>
//         <Created> 03/10/17 8:19:57 PM </Created>
//         <Key> 79fc10d0-9a61-45f1-8756-f795f94d459e </Key>
//     </File>
//     <Summary>
//         AuthenticationHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Novell.Directory.Ldap;
using System;

namespace Puppy.ActiveDirectory
{
    public static class AuthenticationHelper
    {
        public static bool VerifySignIn(string userName, string password)
        {
            try
            {
                var connection = ActiveDirectoryConfig.GetConnection();

                var entities = connection.Search(ActiveDirectoryConfig.BaseDc, LdapConnection.SCOPE_SUB, $"(sAMAccountName={userName})", new[] { "sAMAccountName" }, false);

                string userDn = null;

                while (entities.hasMore())
                {
                    var entity = entities.next();

                    var account = entity.getAttribute("sAMAccountName");

                    if (account != null && userName.Equals(account.StringValue, StringComparison.OrdinalIgnoreCase))
                    {
                        userDn = entity.DN;
                        break;
                    }
                }

                connection.Bind(userDn, password);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}