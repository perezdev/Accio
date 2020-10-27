using Accio.Business.Models.AccountModels;
using System.Collections.Generic;

namespace Accio.Business.Models.AuthenticationModels
{
    public class AuthenticationResult
    {
        public List<AuthenticationResultItem> ResultItems { get; set; }
        public AccountModel Account { get; set; }
    }
}
