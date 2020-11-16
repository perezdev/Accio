using Accio.Business.Models.AccountModels;
using Accio.Business.Models.AuthenticationModels;
using Accio.Business.Services.AccountServices;
using Accio.Business.Services.AuthenticationHistoryServices;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.AuthenticationServices
{
    public class AuthenticationService
    {
        private AccioContext _context { get; set; }
        private AccountService _accountService { get; set; }
        private AuthenticationHistoryService _authenticationHistoryService { get; set; }

        public AuthenticationService(AccioContext context, AccountService accountService, AuthenticationHistoryService authenticationHistoryService)
        {
            _context = context;
            _accountService = accountService;
            _authenticationHistoryService = authenticationHistoryService;
        }

        public bool VerifyPassword(string existingHashedPassword, string providedHashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(existingHashedPassword, providedHashedPassword);
        }

        public AuthenticationResult Authenticate(string emailAddress, string password)
        {
            var result = new AuthenticationResult();

            var account = _accountService.GetAccountByEmailAddress(emailAddress);
            var verifyPassword = VerifyPassword(password, account.PasswordHash);

            if (string.IsNullOrEmpty(emailAddress))
            {
                result.ResultItems.Add(new AuthenticationResultItem() 
                {
                    Type = AuthenticationResultItemType.EmailAddressInvalid,
                    Message = "Email address cannot be empty.",
                });
            }
            if (string.IsNullOrEmpty(password))
            {
                result.ResultItems.Add(new AuthenticationResultItem()
                {
                    Type = AuthenticationResultItemType.PasswordInvalid,
                    Message = "Password cannot be empty.",
                });
            }
            if (!verifyPassword)
            {
                result.ResultItems.Add(new AuthenticationResultItem()
                {
                    Type = AuthenticationResultItemType.PasswordInvalid,
                    Message = "Password is invalid.",
                });
            }

            if (result.ResultItems.Any(x => x.Type == AuthenticationResultItemType.EmailAddressInvalid || 
                                            x.Type == AuthenticationResultItemType.EmailAddressInvalid))
            {
                _authenticationHistoryService.LogAuthentication(AuthAttemptType.Fail, null, emailAddress, Guid.Empty);
                return result;
            }

            result.ResultItems.Add(new AuthenticationResultItem() { Type = AuthenticationResultItemType.Authenticated });
            result.Account = account;
            _authenticationHistoryService.LogAuthentication(AuthAttemptType.Success, null, emailAddress, Guid.Empty);

            return result;
        }
    }
}
