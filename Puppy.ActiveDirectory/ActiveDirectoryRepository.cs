#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ActiveDirectoryRepository.cs </Name>
//         <Created> 03/10/17 10:37:25 PM </Created>
//         <Key> 505be867-83b1-45d6-916c-fa7d066e4053 </Key>
//     </File>
//     <Summary>
//         ActiveDirectoryRepository.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Utilclass;
using System.Collections;
using System.Collections.Generic;

namespace Puppy.ActiveDirectory
{
    public static class ActiveDirectoryRepository
    {
        public static List<Dictionary<string, string>> Get(string filter)
        {
            List<Dictionary<string, string>> listInfo = new List<Dictionary<string, string>>();

            var connection = ActiveDirectoryConfig.GetConnection();

            var entities = connection.Search(ActiveDirectoryConfig.BaseDc, LdapConnection.SCOPE_SUB, filter, null, false);

            while (entities.hasMore())
            {
                try
                {
                    // Get User Info
                    Dictionary<string, string> info = new Dictionary<string, string>();

                    // Read entity and all attributes
                    var entity = entities.next();
                    LdapAttributeSet attributeSet = entity.getAttributeSet();
                    IEnumerator attributeEnumerator = attributeSet.GetEnumerator();

                    while (attributeEnumerator.MoveNext())
                    {
                        LdapAttribute attribute = (LdapAttribute)attributeEnumerator.Current;
                        string attributeName = attribute.Name;
                        string attributeVal = attribute.StringValue;

                        if (!Base64.isLDIFSafe(attributeVal))
                        {
                            byte[] bytes = SupportClass.ToByteArray(attributeVal);
                            attributeVal = Base64.encode(SupportClass.ToSByteArray(bytes));
                        }

                        info.Add(attributeName, attributeVal);
                    }

                    // Add to List User Info
                    listInfo.Add(info);
                }
                catch (LdapException)
                {
                    // Exception is thrown, go for next entry
                }
            }

            return listInfo;
        }
    }
}