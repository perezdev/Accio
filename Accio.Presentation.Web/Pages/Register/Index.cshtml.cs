using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accio.Business.Models.AccountModels;
using Accio.Business.Services.AccountServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accio.Presentation.Web.Pages.Register
{
    public class IndexModel : PageModel
    {
        private AccountService _accountService { get; set; }

        public IndexModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        public void OnGet()
        {
        }

        public JsonResult OnPostRegisterAsync(string userName, string emailAddress, string password, string bogusData)
        {
            try
            {
                var param = new AccountPersistParams() 
                {
                    AccountName = userName,
                    EmailAddress = emailAddress,
                    Password = password,
                    BogusData = bogusData,
                };
                _accountService.CreateAccount(param);

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }
    }
}
