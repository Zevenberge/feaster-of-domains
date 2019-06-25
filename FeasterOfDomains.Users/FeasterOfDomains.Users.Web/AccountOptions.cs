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
    }
}
