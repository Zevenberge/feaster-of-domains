using System.Collections.Generic;
//using IdentityServer4.Models;

namespace FeasterOfDomains.Users.Web
{
    public static class AccountOptions
    {
        public static bool AllowLocalLogin { get; set; }

        public static bool ShowLogoutPrompt { get; set; }
        public static bool AutomaticRedirectAfterSignOut { get; set; }

        // to enable windows authentication, the host (IIS or IIS Express) also must have windows auth enabled.
        public static bool WindowsAuthenticationEnabled { get; set; }

        public static bool IncludeWindowsGroups { get; set; }
        
        // specify the Windows authentication scheme and display name
        public static string WindowsAuthenticationSchemeName { get; set; }

        public const string InvalidCredentialsErrorMessage = "Unknown username or password";

        /*public static IEnumerable<Client> Clients 
        {
            get
            {
                yield return new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "feaster-of-domains" }
                };
            }
        }*/
    }
}
