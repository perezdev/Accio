using System;
using Accio.Business.Services.CardServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accio.Presentation.Web.Pages.Sets
{
    public class IndexModel : PageModel
    {
        private SetService _setService { get; set; }

        public IndexModel(SetService setService)
        {
            _setService = setService;
        }

        public void OnGet()
        {

        }

        public JsonResult OnPostGetSetsAsync()
        {
            try
            {
                var sets = _setService.GetSets();
                return new JsonResult(new { success = true, json = sets });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }
    }
}