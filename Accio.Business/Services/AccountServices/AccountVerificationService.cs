using Accio.Business.Models.AccountModels;
using Accio.Business.Models.EmailModels;
using Accio.Business.Services.EmailServices;
using Accio.Data;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Accio.Business.Services.AccountServices
{
    public class AccountVerificationService
    {
        private AccioContext _context { get; set; }
        private AccountVerificationNumberService _accountVerificationNumberService { get; set; }
        private EmailService _emailService { get; set; }

        public AccountVerificationService(AccountVerificationNumberService accountVerificationNumberService, EmailService emailService,
                                          AccioContext context)
        {
            _accountVerificationNumberService = accountVerificationNumberService;
            _emailService = emailService;
            _context = context;
        }

        public void SendAccountVerificationEmail(Guid accountId, string toEmailAddress, string accountName)
        {
            var accountVerificationNumberModel = _accountVerificationNumberService.GetAccountVerificationNumber(accountId);
            _accountVerificationNumberService.SaveAccountVerificationNumber(accountVerificationNumberModel);
            var plainTextBody = GetAccountPlainTextBody(accountVerificationNumberModel.Number);
            var htmlTextBody = GetAccountHtmlBody(accountVerificationNumberModel.Number);

            _emailService.SendEmail(EmailFromType.Accounts, toEmailAddress, accountName,
                                    $"{accountVerificationNumberModel.Number} is your Accio verification code",
                                    plainTextBody, htmlTextBody);
        }

        public VerifyAccountResult VerifyAccount(string emailAddress, int code)
        {
            var now = DateTime.UtcNow;

            var verificationNumber = (from number in _context.AccountVerificationNumbers
                                      join account in _context.Accounts on number.AccountId equals account.AccountId
                                      where !number.Deleted && number.Number == code && account.EmailAddress == emailAddress
                                      select number).SingleOrDefault();
            var result = new VerifyAccountResult();

            if (verificationNumber == null)
            {
                result.Messages.Add($"{code} is an invalid code.");
                result.Result = false;
            }
            if (verificationNumber != null && (now.Subtract(verificationNumber.Expires).Hours > 1))
            {
                result.Messages.Add($"{code} has expired.");
                result.Result = false;
            }

            if (result.Result)
            {
                //At this point, the account has been verified. The code is valid and is hasn't expired
                var userAccount = _context.Accounts.Single(x => x.EmailAddress == emailAddress);
                userAccount.Verified = true;
                userAccount.Active = true;
                //Hard delte the verification row to free up the number
                _context.AccountVerificationNumbers.Remove(verificationNumber);

                _context.SaveChanges();
            }

            //Result is true by default. If the checks don't fail, just return default object with no messages
            return result;
        }

        private string GetAccountPlainTextBody(int verificationNumber)
        {
            var body = $@"Confirm your email address. There's one small step you need to complete to activate your Accio account.
                          Please enter this verification code to verify your account: {verificationNumber}. This code expires after one hour.
                          Thanks, Accio Support Team.";

            return body;
        }
        private string GetAccountHtmlBody(int verificationNumber)
        {
            var body = $@"<html>
                            <body>
                                <h3>Confirm your email address</h3>
                                <p>There's one small step you need to complete to activate your Accio account.</p>
                                <p>Please enter this verification code to verify your account:</p>
                                <h3>{verificationNumber}</h3>
                                <p>This code expires after one hour.</p>
                                <p>Thanks,<br />Accio Support Team</p>
                            </body>
                          </html>";

            return body;
        }
    }
}
