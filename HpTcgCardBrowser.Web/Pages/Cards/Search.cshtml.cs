using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HpTcgCardBrowser.Business.Models.CardModels;
using HpTcgCardBrowser.Business.Services.CardServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HpTcgCardBrowser.Web
{
    public class SearchModel : PageModel
    {
        private SetService _setService { get; set; }
        private CardService _cardService { get; set; }

        public SearchModel(SetService setService, CardService cardService)
        {
            _setService = setService;
            _cardService = cardService;
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
        public JsonResult OnPostSearchCardsAsync(Guid setId, string searchText, string sortBy, string sortOrder)
        {
            try
            {
                var cardSearchParameters = new CardSearchParameters()
                {
                    SetId = setId,
                    SearchText = searchText,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    //Harding coding English for now, as we don't have other languages atm
                    LanguageId = new Guid("4F5CC98D-4315-4410-809F-E2CC428E0C9B"),
                };

                var cards = _cardService.SearchCards(cardSearchParameters);
                return new JsonResult(new { success = true, json = cards });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }
    }
}