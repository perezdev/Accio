using Accio.Business.Models.RoleModels;
using Accio.Business.Services.RoleServices;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.AccountRoleServices
{
    public class AccountRoleService
    {
        private AccioContext _context { get; set; }

        public AccountRoleService(AccioContext context)
        {
            _context = context;
        }

        public void AddRoleToAccount(Guid roleId, Guid accountId, Guid persistedById)
        {
            var accountRole = _context.AccountRoles.SingleOrDefault(x => x.AccountRoleId == roleId && x.AccountId == accountId);
            if (accountRole == null)
            {
                var now = DateTime.UtcNow;
                accountRole = new AccountRole()
                {
                    AccountRoleId = Guid.NewGuid(),
                    RoleId = roleId,
                    AccountId = accountId,
                    CreatedById = persistedById,
                    CreatedDate = now,
                    UpdatedById = persistedById,
                    UpdatedDate = now,
                    Deleted = false,
                };
                _context.AccountRoles.Add(accountRole);
                _context.SaveChanges();
            }
        }

        public List<RoleModel> GetAccountRoles(Guid accountId)
        {
            var roles = (from accountRole in _context.AccountRoles
                         join role in _context.Roles on accountRole.RoleId equals role.RoleId
                         where accountRole.AccountId == accountId
                         select RoleService.GetRoleModel(role)).ToList();
            return roles;
        }
    }
}
