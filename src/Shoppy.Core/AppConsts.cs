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
            public const string Users = "Users";
            public const string UserManager = Users + "_Manager";

            public enum ManageActions
            {
                All,
                Create,
                Edit,
                Delete,
                List
            }

            public const string Accounts = "Accounts";
            public const string AccountRegister = Accounts + "_Register";
        }
    }
}
