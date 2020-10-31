﻿using Accio.Business.Models.AccountModels;
using Accio.Business.Models.AuthenticationModels;
using Accio.Business.Services.AuthenticationHistoryServices;
using Accio.Data;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.AccountServices
{
    public class AccountService
    {
        private AccioContext _context { get; set; }
        private AuthenticationHistoryService _authenticationHistoryService { get; set; }

        public AccountService(AccioContext context, AuthenticationHistoryService authenticationHistoryService)
        {
            _context = context;
            _authenticationHistoryService = authenticationHistoryService;
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

            var account = GetAccountForCreate(accountParams.FirstName, accountParams.FirstName, accountParams.AccountName,
                                              accountParams.EmailAddress, accountParams.Password);
            _context.Account.Add(account);
            _context.SaveChanges();

            result.Result = true;
            result.Account = GetAccountModel(account);
            return result;
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
            if (_context.Account.Any(x => x.EmailAddress == accountParams.EmailAddress && !x.Deleted))
            {
                messages.Add("An account with that email address already exists.");
            }
            if (_context.Account.Any(x => x.AccountName == accountParams.AccountName && !x.Deleted))
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