using System.Collections.Generic;

namespace Accio.Business.Models.AccountModels
{
    public class AccountPersistResult
    {
        public bool Result { get; set; }
        public List<AccountValidateErrorType> Messages { get; set; } = new List<AccountValidateErrorType>();
        public AccountModel Account { get; set; }
    }
}
