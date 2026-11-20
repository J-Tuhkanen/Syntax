namespace Syntax.API
{
    public static class ApplicationConstants
    {
        public static class UserRoles
        {
            public static readonly string Admin = "Admin";
            public static readonly string User = "User";

            public static IEnumerable<string> GetRoles()
                => new[] { Admin, User };
        }

        public static class RateLimitPolicies
        {
            /// <summary>Sign-in and registration: few attempts per client per minute.</summary>
            public const string Authentication = "authentication";
        }
    }
}
