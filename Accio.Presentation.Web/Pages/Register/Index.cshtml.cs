using System;
using Accio.Business.Models.AccountModels;
using Accio.Business.Services.AccountServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Internal;

namespace Accio.Presentation.Web.Pages.Register
{
    public class IndexModel : PageModel
    {
        private AccountService _accountService { get; set; }
        public AccountVerificationService _accountVerificationService { get; set; }

        public IndexModel(AccountService accountService, AccountVerificationService accountVerificationService)
        {
            _accountService = accountService;
            _accountVerificationService = accountVerificationService;
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
                var result = _accountService.CreateAccount(param);

                return new JsonResult(new { success = result.Result, json = string.Join(" ", result.Messages) });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }

        public JsonResult OnPostVerifyCodeAsync(string emailAddress, int code)
        {
            try
            {
                var verificationResult = _accountVerificationService.VerifyAccount(emailAddress, code);
                return new JsonResult(new { success = verificationResult.Result, json = string.Join(" ", verificationResult.Messages) });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }
    }
}
