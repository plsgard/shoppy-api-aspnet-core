namespace Shoppy.Core
{
    public static class AppConsts
    {
        public static class Roles
        {
            public const string Administrator = "Administrator";
        }

        public static class Policies
        {
            public static class Claims
            {
                public const string UsersRights = "Users";
                public const string AccountsRights = "Accounts";
            }

            public enum ManageActions
            {
                All,
                Create,
                Edit,
                Delete,
                List
            }

            public static class Accounts
            {
                public const string Register = "AccountRegister";
            }

            public static class Users
            {
                public const string Manager = "UserManager";
            }
        }
    }
}
