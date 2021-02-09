using System;

namespace Accio.Business.Models.AccountModels
{
    public class AccountPersistParams
    {
        public string AccountName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string BogusData { get; set; }
        public Guid ClientId { get; set; }
    }
}
