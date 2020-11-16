using Accio.Business.Models.AccountModels;
using System.Collections.Generic;
using System.Security.Claims;

namespace Accio.Presentation.Web.PresentationServices
{
    public class ClaimService
    {
        private string AccountIdTypeName { get; set; } = "AccountId";
        private string AccountNameTypeName { get; set; } = "AccountName";
        private string FirstNameTypeName { get; set; } = "FirstName";
        private string LastNameTypeName { get; set; } = "LastName";
        private string EmailAddressTypeName { get; set; } = "EmailAddress";

        public List<Claim> GetClaimsFromAccountModel(AccountModel accountModel)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(AccountIdTypeName, accountModel.AccountId.ToString().ToUpper()));
            if (!string.IsNullOrEmpty(accountModel.FirstName))
            {
                claims.Add(new Claim(AccountNameTypeName, accountModel.FirstName));
            }
            if (!string.IsNullOrEmpty(accountModel.LastName))
            {
                claims.Add(new Claim(LastNameTypeName, accountModel.LastName));
            }
            claims.Add(new Claim(ClaimTypes.Name, accountModel.AccountName));
            claims.Add(new Claim(EmailAddressTypeName, accountModel.EmailAddress));

            foreach (var roleModel in accountModel.Roles)
            {
                var roleClaim = new Claim(ClaimTypes.Role, roleModel.Name);
                claims.Add(roleClaim);
            }
            
            return claims;
        }
    }
}
