using System;
using Accio.Business.Models.CardModels;
using Accio.Business.Models.SourceModels;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.SourceServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accio.Web.Pages.Search
{
    public class IndexModel : PageModel
    {
        private SetService _setService { get; set; }
        private CardService _cardService { get; set; }
        private SourceService _sourceService { get; set; }

        public IndexModel(SetService setService, CardService cardService, SourceService sourceService)
        {
            _setService = setService;
            _cardService = cardService;
            _sourceService = sourceService;
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
        public JsonResult OnPostGetSetBySetIdAsync(Guid setId)
        {
            try
            {
                var set = _setService.GetSet(setId);
                return new JsonResult(new { success = true, json = set });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }
        public JsonResult OnPostSearchCardsAsync(Guid setId, string searchText, string sortBy, string sortOrder)
        {
            try
            {
                var websiteSource = _sourceService.GetSource(SourceType.Website);

                var cardSearchParameters = new CardSearchParameters()
                {
                    SetId = setId,
                    SearchText = searchText,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    //Harding coding English for now, as we don't have other languages atm
                    LanguageId = new Guid("4F5CC98D-4315-4410-809F-E2CC428E0C9B"),
                    SourceId = websiteSource.SourceId,
                };

                var cards = _cardService.SearchCards(cardSearchParameters);
                return new JsonResult(new { success = true, json = cards });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }
        public JsonResult OnPostGetPopularCardsAsync()
        {
            try
            {
                var cards = _cardService.GetMostPopularCardsFromSearchHistory();
                return new JsonResult(new { success = true, json = cards });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }
    }
}