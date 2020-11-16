using Accio.Business.Models.AccountModels;
using System.Collections.Generic;

namespace Accio.Business.Models.AuthenticationModels
{
    public class AuthenticationResult
    {
        public List<AuthenticationResultItem> ResultItems { get; set; } = new List<AuthenticationResultItem>();
        public AccountModel Account { get; set; }
    }
}
