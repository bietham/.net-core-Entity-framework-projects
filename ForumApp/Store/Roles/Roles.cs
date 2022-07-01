namespace ForumApp.Store.Roles
{
    
        public static class Roles
        {
            public const string Admin = "admin";
            public const string Moderator = "moderator";
            //NewRole

            public static readonly string[] AllRoles = new string[] { Admin, Moderator, /*NewRole*/ };
        }
}
