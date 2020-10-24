using System;
using System.Collections.Generic;
using Accio.Business.Models.AdvancedCardSearchSearchModels;
using Accio.Business.Models.CardModels;
using Accio.Business.Models.SourceModels;
using Accio.Business.Services.AdvancedCardSearchSearchServices;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.SourceServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accio.Presentation.Web.Pages.Search
{
    public class IndexModel : PageModel
    {
        private SetService _setService { get; set; }
        private SourceService _sourceService { get; set; }
        private CardService _cardService { get; set; }
        public AdvancedCardSearchService _advancedCardSearchService { get; set; }

        public IndexModel(SetService setService, CardService cardService, SourceService sourceService, AdvancedCardSearchService advancedCardSearchService)
        {
            _setService = setService;
            _cardService = cardService;
            _sourceService = sourceService;
            _advancedCardSearchService = advancedCardSearchService;
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

        public JsonResult OnPostGetSetByShortName(string setShortName)
        {
            try
            {
                var setByShortName = _setService.GetSetByShortName(setShortName);
                return new JsonResult(new { success = true, json = setByShortName });
            }
            catch (Exception exception)
            {
                return new JsonResult(new {
                    SourceService = false,
                    json = exception.Message
                });
            }
        }

        public JsonResult OnPostSearchCardsAsync(string set, string searchText, string sortBy, string sortOrder)
        {
            try
            {
                // Harding coding English for now, as we don't have other languages atm
                var englishLanguageId = new Guid("4F5CC98D-4315-4410-809F-E2CC428E0C9B");
                var websiteSource = _sourceService.GetSource(SourceType.Website);
                var cards = new List<CardModel>();

                var isAdvancedSearch = _advancedCardSearchService.IsAdvancedSearch(searchText);
                if (isAdvancedSearch)
                {
                    var advancedSearchParamters = new AdvancedSearchParameters() 
                    {
                        AdvancedSearchText = searchText,
                        SortBy = sortBy,
                        SortOrder = sortOrder,
                        LanguageId = englishLanguageId,
                        SourceId = websiteSource.SourceId,
                    };
                    cards = _advancedCardSearchService.SearchCards(advancedSearchParamters);
                }
                else
                {
                    var cardSearchParameters = new CardSearchParameters()
                    {
                        SetShortName = set,
                        SearchText = searchText,
                        SortBy = sortBy,
                        SortOrder = sortOrder,
                        LanguageId = englishLanguageId,
                        SourceId = websiteSource.SourceId,
                    };
                    cards = _cardService.SearchCards(cardSearchParameters);
                }

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