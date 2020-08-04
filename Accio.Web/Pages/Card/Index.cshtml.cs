using System;
using System.Collections.Generic;
using Accio.Business.Models.CardModels;
using Accio.Business.Models.RulingModels;
using Accio.Business.Models.SourceModels;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.LessonServices;
using Accio.Business.Services.SourceServices;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Accio.Web.Pages.Card
{
    public class IndexModel : PageModel
    {
        public CardModel Card { get; set; }
        public List<RulingModel> Rules { get; set; }

        public string SetShortName { get; set; }
        public string CardNumber { get; set; }
        public string CardName { get; set; }

        public bool ShowCardData { get; set; } = false;

        private SourceService _sourceService { get; set; }
        private CardRulingService _cardRulingService { get; set; }
        private SingleCardService _singleCardService { get; set; }
        public LessonService _lessonService { get; set; }
        public TypeService _cardTypeService { get; set; }

        public IndexModel(SingleCardService singleCardService, SourceService sourceService, CardRulingService cardRulingService,
                          LessonService lessonService, TypeService typeService)
        {
            _singleCardService = singleCardService;
            _sourceService = sourceService;
            _cardRulingService = cardRulingService;
            _lessonService = lessonService;
            _cardTypeService = typeService;
        }

        public void OnGet(string setShortName, string cardNumber, string cardName)
        {
            SetShortName = setShortName;
            CardNumber = cardNumber;
            CardName = cardName;

            if (string.IsNullOrEmpty(SetShortName) || string.IsNullOrEmpty(CardNumber) || string.IsNullOrEmpty(CardName))
            {
                ShowCardData = false;
            }
            else
            {
                try
                {
                    var websiteSource = _sourceService.GetSource(SourceType.Website);
                    var param = new SingleCardParameters()
                    {
                        SetShortName = SetShortName,
                        CardNumber = CardNumber,
                        CardName = CardName,
                    };

                    Card = _singleCardService.GetCard(param);
                    Rules = _cardRulingService.GetCardRules(Card.CardId);

                    ShowCardData = Card != null;
                }
                catch (Exception ex)
                {
                    //TODO: idk, something
                }
            }
        }

        //public JsonResult OnPostSearchSingleCardAsync(Guid cardId)
        //{
        //    try
        //    {
        //        var websiteSource = _sourceService.GetSource(SourceType.Website);
        //        var cardSearchParameters = new CardSearchParameters()
        //        {
        //            CardId = cardId,
        //            SourceId = websiteSource.SourceId,
        //        };

        //        var card = _cardService.SearchSingleCard(cardSearchParameters);
        //        return new JsonResult(new { success = true, json = card });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new { success = false, json = ex.Message });
        //    }
        //}
        //public JsonResult OnPostGetCardRulingsAsync(Guid cardId)
        //{
        //    try
        //    {
        //        var rules = _cardRulingService.GetCardRules(cardId);
        //        return new JsonResult(new { success = true, json = rules });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new { success = false, json = ex.Message });
        //    }
        //}
    }
}