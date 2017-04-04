#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.SSO </Project>
//     <File>
//         <Name> Dummy </Name>
//         <Created> 02 Apr 17 11:18:50 PM </Created>
//         <Key> fd8dc6ee-e1d6-4d07-8a3f-9218cc3fe6bc </Key>
//     </File>
//     <Summary>
//         Dummy
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace TopCore.SSO.Identity
{
    public class Dummy
    {
        // scopes define the API resources in your system
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "UserApi",
                    DisplayName = "User API",
                    Description = "User API Access",
                    Scopes = {new Scope("User.Get") }
                }
            };
        }

        public static List<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = new List<string> {"role" }
                }
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    RequireConsent = false,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = {"http://localhost:5002/signin-oidc"},
                    PostLogoutRedirectUris = {"http://localhost:5002/signout-callback-oidc"},

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "User.Get"
                    },
                    AllowOfflineAccess = true
                }
            };
        }

        public static List<TopCoreIdentityUser> GetUsers()
        {
            return new List<TopCoreIdentityUser>
            {
                new TopCoreIdentityUser("topnguyen")
                {
                    Password = "123456"
                }
            };
        }
    }
}