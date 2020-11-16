using Accio.Business.Models.AccountModels;
using Accio.Business.Models.AuthenticationModels;
using Accio.Business.Models.RoleModels;
using Accio.Business.Services.AccountRoleServices;
using Accio.Business.Services.AuthenticationHistoryServices;
using Accio.Business.Services.RoleServices;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.AccountServices
{
    public class AccountService
    {
        private AccioContext _context { get; set; }
        private AuthenticationHistoryService _authenticationHistoryService { get; set; }
        private AccountVerificationService _accountVerificationService { get; set; }
        private AccountRoleService _accountRoleService { get; set; }
        private RoleService _roleService { get; set; }

        public AccountService(AccioContext context, AuthenticationHistoryService authenticationHistoryService, AccountVerificationService accountVerificationService,
                              AccountRoleService accountRoleService, RoleService roleService)
        {
            _context = context;
            _authenticationHistoryService = authenticationHistoryService;
            _accountVerificationService = accountVerificationService;
            _accountRoleService = accountRoleService;
            _roleService = roleService;
        }

        public AccountPersistResult CreateAccount(AccountPersistParams accountParams)
        {
            if (!string.IsNullOrEmpty(accountParams.BogusData))
            {
                _authenticationHistoryService.LogAuthentication(AuthAttemptType.Bot, accountParams.AccountName, accountParams.EmailAddress,
                                                                accountParams.ClientId, accountParams.BogusData);
                return null;
            }

            var result = new AccountPersistResult();
            var validationMessages = GetValidationMessages(accountParams);
            if (validationMessages.Count > 0)
            {
                result.Result = false;
                result.Messages = validationMessages;
                return result;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(accountParams.Password, BCrypt.Net.SaltRevision.Revision2Y);
            var account = GetAccountForCreate(accountParams.FirstName, accountParams.FirstName, accountParams.AccountName,
                                              accountParams.EmailAddress, hashedPassword);
            _context.Accounts.Add(account);
            _context.SaveChanges();
            var userRole = _roleService.GetRoleByType(RoleType.User);
            _accountRoleService.AddRoleToAccount(userRole.RoleId, account.AccountId, account.AccountId);

            _accountVerificationService.SendAccountVerificationEmail(account.AccountId, accountParams.EmailAddress, accountParams.AccountName);

            var accountModel = GetAccountModel(account);
            accountModel.Roles.Add(userRole);

            result.Result = true;
            result.Account = accountModel;
            return result;
        }

        public AccountModel GetAccountByEmailAddress(string emailAddress)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.EmailAddress == emailAddress);
            var accountModel = GetAccountModel(account);
            accountModel.Roles = _accountRoleService.GetAccountRoles(account.AccountId);
            return account == null ? null : accountModel;
        }

        private List<string> GetValidationMessages(AccountPersistParams accountParams)
        {
            var messages = new List<string>();

            if (string.IsNullOrEmpty(accountParams.EmailAddress))
            {
                messages.Add("Email address cannot be emppty.");
            }
            if (string.IsNullOrEmpty(accountParams.Password))
            {
                messages.Add("Password cannot be empty.");
            }
            if (string.IsNullOrEmpty(accountParams.AccountName))
            {
                messages.Add("Account name cannot be empty.");
            }
            if (_context.Accounts.Any(x => x.EmailAddress == accountParams.EmailAddress && !x.Deleted))
            {
                messages.Add("An account with that email address already exists.");
            }
            if (_context.Accounts.Any(x => x.AccountName == accountParams.AccountName && !x.Deleted))
            {
                messages.Add("An account with that account name already exists.");
            }

            return messages;
        }
        private static AccountModel GetAccountModel(Account account)
        {
            return new AccountModel()
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                FirstName = account.FirstName,
                LastName = account.LastName,
                EmailAddress = account.EmailAddress,
                Active = account.Active,
                PasswordHash = account.PasswordHash,
                Verified = account.Verified,
                CreatedById = account.CreatedById,
                CreatedDate = account.CreatedDate,
                UpdatedById = account.UpdatedById,
                UpdatedDate = account.UpdatedDate,
                Deleted = account.Deleted,
            };
        }
        private Account GetAccountForCreate(string firstName, string lastName, string accountName, string emailAddress, string passwordHash)
        {
            var now = DateTime.UtcNow;
            return new Account()
            {
                AccountId = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                AccountName = accountName,
                EmailAddress = emailAddress,
                PasswordHash = passwordHash,
                Active = false,
                Verified = false,
                CreatedById = Guid.Empty,
                CreatedDate = now,
                UpdatedById = Guid.Empty,
                UpdatedDate = now,
                Deleted = false,
            };
        }
    }
}
