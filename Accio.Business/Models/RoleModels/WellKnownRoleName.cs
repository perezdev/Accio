namespace Accio.Business.Models.RoleModels
{
    /// <summary>
    /// Single source of truth of the role names. Used wherever the names need to be referenced.
    /// Thhis is typically going to be in the role service and web project, for permissions
    /// </summary>
    public static class WellKnownRoleName
    {
        public readonly static string AdminRole = "Admin";
        public readonly static string UserRole = "User";
    }
}
