using Accio.Business.Models.EmailModels;
using Accio.Business.Services.ConfigurationServices;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;

namespace Accio.Business.Services.EmailServices
{
    public class EmailService
    {
        private ConfigurationService _configurationService { get; set; }

        public EmailService(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        /// <summary>
        /// Determines if an email address is valid. We don't need to do a complex email check,
        /// as we also require the user to verify their email via code.
        /// https://stackoverflow.com/a/1374644
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public bool IsEmailAddressValid(string emailAddress)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(emailAddress);
                return addr.Address == emailAddress;
            }
            catch
            {
                return false;
            }
        }

        public void SendEmail(EmailFromType type, string toEmailAddress, string accountName, string subject, string plainTextBody, string htmlBody)
        {
            var emailFromCredentials = GetEmailCredentials(type);
            var client = new SendGridClient(emailFromCredentials.SendGridApiKey);
            var from = new EmailAddress(emailFromCredentials.EmailAddress.Address, emailFromCredentials.Name);
            var to = new EmailAddress(toEmailAddress, accountName);
            
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextBody, htmlBody);
            var response = client.SendEmailAsync(msg).ConfigureAwait(true);
        }
        private EmailCredentialModel GetEmailCredentials(EmailFromType type)
        {
            
            switch (type)
            {
                case EmailFromType.Accounts:
                    return _configurationService.GetAccountsEmailCredentials();
                default:
                    throw new Exception($"{type.ToString()} is not a valid");
            }
        }
    }
}
