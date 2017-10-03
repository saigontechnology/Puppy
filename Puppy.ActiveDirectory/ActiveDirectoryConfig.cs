#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ActiveDirectoryConfig.cs </Name>
//         <Created> 03/10/17 8:59:24 PM </Created>
//         <Key> 17f7d29e-24f6-4e07-8c41-6f3d3c20bd5d </Key>
//     </File>
//     <Summary>
//         ActiveDirectoryConfig.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Novell.Directory.Ldap;

namespace Puppy.ActiveDirectory
{
    public class ActiveDirectoryConfig
    {
        public static string Host { get; set; }

        public static int Port { get; set; } = LdapConnection.DEFAULT_PORT;

        public static bool IsSecure { get; set; }

        public static string UserName { get; set; }

        public static string Password { get; set; }

        public static string BaseDc { get; set; }

        protected static ILdapConnection Connection;

        internal static ILdapConnection GetConnection()
        {
            return GetConnection(Host, Port, IsSecure, UserName, Password);
        }

        internal static ILdapConnection GetConnection(string host, int port, bool isSecure, string userName, string password)
        {
            Host = host;
            Port = port;
            IsSecure = isSecure;
            UserName = userName;
            Password = password;

            if (Connection is LdapConnection ldapConnection)
            {
                return ldapConnection;
            }

            // Creating an LdapConnection instance
            ldapConnection = new LdapConnection { SecureSocketLayer = isSecure };

            // Connect function will create a socket connection to the server - Port 389 for insecure
            // and 3269 for secure
            ldapConnection.Connect(host, port);

            // Bind function with null user dn and password value will perform anonymous bind to LDAP server
            ldapConnection.Bind(userName, password);

            return ldapConnection;
        }
    }
}