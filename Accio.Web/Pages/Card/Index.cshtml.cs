using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accio.Business.Models.CardModels;
using Accio.Business.Models.SourceModels;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.SourceServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accio.Web.Pages.Card
{
    public class IndexModel : PageModel
    {
        private CardService _cardService { get; set; }
        private SourceService _sourceService { get; set; }
        private CardRulingService _cardRulingService { get; set; }

        public IndexModel(CardService cardService, SourceService sourceService, CardRulingService cardRulingService)
        {
            _cardService = cardService;
            _sourceService = sourceService;
            _cardRulingService = cardRulingService;
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
        public JsonResult OnPostGetCardRulingsAsync(Guid cardId, Guid cardTypeId)
        {
            try
            {
                var rules = _cardRulingService.GetCardRules(cardId, cardTypeId);
                return new JsonResult(new { success = true, json = rules });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }
    }
}