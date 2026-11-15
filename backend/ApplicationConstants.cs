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
    }
}
