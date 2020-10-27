using System.Collections.Generic;

namespace Accio.Business.Models.AccountModels
{
    public class AccountPersistResult
    {
        public bool Result { get; set; }
        public List<string> Messages { get; set; }
        public AccountModel Account { get; set; }
    }
}
