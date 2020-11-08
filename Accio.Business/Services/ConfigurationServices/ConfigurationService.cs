using Accio.Business.Models.ConfigurationModels;
using Accio.Business.Models.EmailModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Mail;

namespace Accio.Business.Services.ConfigurationServices
{
    public class ConfigurationService
    {
        private IConfiguration _config { get; }

        public ConfigurationService(IConfiguration config)
        {
            _config = config;
        }

        public EnvironmentType GetEnvironment()
        {
            var envName = _config["Environment"];
            if (envName == "Development")
            {
                return EnvironmentType.Development;
            }
            else if (envName == "Production")
            {
                return EnvironmentType.Production;
            }
            else
            {
                throw new Exception($"{envName} is an invalid environment.");
            }
        }

        public EmailCredentialModel GetAccountsEmailCredentials()
        {
            return new EmailCredentialModel() 
            {
                Name = "Accio Account Support",
                EmailAddress = new MailAddress(_config["AccioEmailAccounts:AccountsEmail:Address"]),
                SendGridApiKey = _config["AccioEmailAccounts:AccountsEmail:SendGridApiKey"],
            };
        }
    }
}
