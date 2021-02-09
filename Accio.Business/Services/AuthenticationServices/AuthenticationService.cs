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

        public AuthenticationResult Authenticate(string emailAddressOrUsername, string password)
        {
            var result = new AuthenticationResult();

            if (!_accountService.AccountExists(emailAddressOrUsername))
            {
                result.ResultItems.Add(new AuthenticationResultItem()
                {
                    Type = AuthenticationResultItemType.EmailAddressInvalid,
                    Message = "An account with that email address or username doesn't exist.",
                });
                return result;
            }
            if (!_accountService.AccountVerified(emailAddressOrUsername))
            {
                result.ResultItems.Add(new AuthenticationResultItem()
                {
                    Type = AuthenticationResultItemType.Unverified,
                    Message = "Your account has not been verified. Please check your email address for the verification code.",
                });
                return result;
            }

            var account = _accountService.GetAccountByEmailAddressOrUsername(emailAddressOrUsername);
            var verifyPassword = VerifyPassword(password, account.PasswordHash);

            if (string.IsNullOrEmpty(emailAddressOrUsername))
            {
                result.ResultItems.Add(new AuthenticationResultItem()
                {
                    Type = AuthenticationResultItemType.EmailAddressInvalid,
                    Message = "Email address or username cannot be empty.",
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
                _authenticationHistoryService.LogAuthentication(AuthAttemptType.Fail, null, emailAddressOrUsername, Guid.Empty);
                return result;
            }

            result.ResultItems.Add(new AuthenticationResultItem() { Type = AuthenticationResultItemType.Authenticated });
            result.Account = account;
            _authenticationHistoryService.LogAuthentication(AuthAttemptType.Success, null, emailAddressOrUsername, Guid.Empty);

            return result;
        }
    }
}
