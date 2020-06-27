using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HpTcgCardBrowser.Business.Models.CardModels;
using HpTcgCardBrowser.Business.Models.SourceModels;
using HpTcgCardBrowser.Business.Services.CardServices;
using HpTcgCardBrowser.Business.Services.SourceServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HpTcgCardBrowser.Web.Pages.Card
{
    public class IndexModel : PageModel
    {
        private CardService _cardService { get; set; }
        private SourceService _sourceService { get; set; }

        public IndexModel(CardService cardService, SourceService sourceService)
        {
            _cardService = cardService;
            _sourceService = sourceService;
        }

        public void OnGet()
        {

        }

        public JsonResult OnPostSearchSingleCardAsync(Guid cardId)
        {
            try
            {
                var websiteSource = _sourceService.GetSource(SourceType.Website);

                var cardSearchParameters = new CardSearchParameters()
                {
                    CardId = cardId,
                    SourceId = websiteSource.SourceId,
                };

                var card = _cardService.SearchSingleCard(cardSearchParameters);
                return new JsonResult(new { success = true, json = card });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }
    }
}