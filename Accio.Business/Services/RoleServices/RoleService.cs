using Accio.Business.Models.RoleModels;
using Accio.Data;
using System;

namespace Accio.Business.Services.RoleServices
{
    public class RoleService
    {
        /*
         * Roles are hard coded so we can reference them directly. The IDs are also stored in the database
         */
        private Guid AdminRoleId { get; set; } = Guid.Parse("ED20603C-01B0-4CEB-9C86-B66EDC4161DD");
        private Guid UserRoleId { get; set; } = Guid.Parse("56B2C4CA-F602-49A6-8ECF-1F469D420A82");

        private string AdminRoleName { get; set; } = WellKnownRoleName.AdminRole;
        private string UserRoleName { get; set; } = WellKnownRoleName.UserRole;

        public RoleModel GetRoleByType(RoleType roleType)
        {
            switch (roleType)
            {
                case RoleType.Admin:
                    return GetRoleModel(AdminRoleId, AdminRoleName);
                case RoleType.User:
                    return GetRoleModel(UserRoleId, UserRoleName);
                default:
                    throw new Exception($"{roleType} is not a valid role.");
            }
        }

        public static RoleModel GetRoleModel(Role role)
        {
            return new RoleModel()
            {
                RoleId = role.RoleId,
                Name = role.Name,
                CreatedById = role.CreatedById,
                CreatedDate = role.CreatedDate,
                UpdatedById = role.UpdatedById,
                UpdatedDate = role.UpdatedDate,
                Deleted = role.Deleted,
            };
        }
        private  RoleModel GetRoleModel(Guid roleId, string name)
        {
            return new RoleModel() 
            {
                RoleId = roleId,
                Name = name,
            };
        }
    }
}
