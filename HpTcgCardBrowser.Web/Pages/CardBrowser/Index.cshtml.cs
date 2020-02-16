using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HpTcgCardBrowser.Business.Services.CardServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HpTcgCardBrowser.Web.Pages.Browser
{
    public class IndexModel : PageModel
    {
        private CardSetService _setService { get; set; }
        private CardService _cardService { get; set; }

        public IndexModel(CardSetService setService, CardService cardService)
        {
            _setService = setService;
            _cardService = cardService;
        }

        public void OnGet()
        {
            try
            {
                
            }
            catch (Exception ex)
            {

            }
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
        public JsonResult OnPostSearchCardsAsync(Guid setId, int? lessonCost, string searchText)
        {
            try
            {
                //Harding coding English for now, as we don't have other languages atm
                var englishLanguageId = new Guid("4F5CC98D-4315-4410-809F-E2CC428E0C9B");
                var cards = _cardService.SearchCards(setId, null, null, englishLanguageId, lessonCost, searchText);
                return new JsonResult(new { success = true, json = cards });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, json = ex.Message });
            }
        }
    }
}