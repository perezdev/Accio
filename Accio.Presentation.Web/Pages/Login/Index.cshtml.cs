using System;
using System.Linq;
using System.Security.Claims;
using Accio.Business.Models.AuthenticationModels;
using auth = Accio.Business.Services.AuthenticationServices;
using Accio.Presentation.Web.PresentationServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace Accio.Presentation.Web.Pages.Login
{
    public class IndexModel : PageModel
    {
        private auth.AuthenticationService _authenticationService { get; set; }
        private ClaimService _claimService { get; set; }

        public IndexModel(auth.AuthenticationService authenticationService, ClaimService claimService)
        {
            _authenticationService = authenticationService;
            _claimService = claimService;
        }

        public void OnGet()
        {
        }

        public JsonResult OnPostLoginAsync(string emailAddress, string password)
        {
            try
            {
                var authenticate = _authenticationService.Authenticate(emailAddress, password);
                if (authenticate.ResultItems.Any(x => x.Type == AuthenticationResultItemType.EmailAddressInvalid ||
                                                      x.Type == AuthenticationResultItemType.PasswordInvalid))
                {
                    return new JsonResult(new { success = false, json = string.Join(" ", authenticate.ResultItems.Select(x => x.Message).ToList()) });
                }
                else
                {
                    var claims = _claimService.GetClaimsFromAccountModel(authenticate.Account);
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(3650),
                        IsPersistent = true,
                    };

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties).GetAwaiter().GetResult();

                    return new JsonResult(new { success = true }); 
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }
    }
}
