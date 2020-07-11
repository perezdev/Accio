using Accio.Business.Models.ConfigurationModels;
using Microsoft.Extensions.Configuration;
using System;

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
    }
}
