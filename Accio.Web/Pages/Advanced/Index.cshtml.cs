using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accio.Business.Services.AdvancedCardSearchSearchServices;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.SourceServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accio.Web.Pages.Advanced
{
    public class IndexModel : PageModel
    {
        private SourceService _sourceService { get; set; }
        public AdvancedCardSearchService _advancedCardSearchService { get; set; }

        public IndexModel(SourceService sourceService, AdvancedCardSearchService advancedCardSearchService)
        {
            _sourceService = sourceService;
            _advancedCardSearchService = advancedCardSearchService;
        }

        public void OnGet()
        {

        }

        public JsonResult OnPostGetAdvancedSearchUrlValueAsync(string cardName, string cardText)
        {
            try
            {
                var searchUrl = _advancedCardSearchService.GetAdvancedSearchUrlValue(cardName, cardText);
                return new JsonResult(new { success = true, json = searchUrl });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }
    }
}