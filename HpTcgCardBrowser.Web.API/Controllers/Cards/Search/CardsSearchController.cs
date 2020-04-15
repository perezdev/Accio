using System;
using System.Collections.Generic;
using HpTcgCardBrowser.Business.Models.CardModels;
using HpTcgCardBrowser.Business.Models.SourceModels;
using HpTcgCardBrowser.Business.Services.CardServices;
using HpTcgCardBrowser.Business.Services.SourceServices;
using Microsoft.AspNetCore.Mvc;

namespace HpTcgCardBrowser.Web.API.Controllers.Cards.Search
{
    [Route("Cards/Search")]
    [ApiController]
    public class CardsSearchController : ControllerBase
    {
        private CardService _cardService { get; set; }
        private SourceService _sourceService { get; set; }

        public CardsSearchController(CardService cardService, SourceService sourceService)
        {
            _cardService = cardService;
            _sourceService = sourceService;
        }

        [HttpGet]
        public IEnumerable<CardModel> Get(string searchText, Guid? setId, Guid? typeId, Guid? rarityId, Guid? languageId,
                                          int? lessonCost, string sortBy, string sortOrder)
        {
            var apiSource = _sourceService.GetSource(SourceType.API);
            var cardparams = new CardSearchParameters() 
            {
                SearchText = searchText,
                SetId = setId,
                TypeId = typeId,
                RarityId = rarityId,
                LanguageId = languageId,
                LessonCost = lessonCost,
                SortBy = sortBy,
                SortOrder = sortOrder,
                SourceId = apiSource.SourceId,
            };

            var cards = _cardService.SearchCards(cardparams);
            return cards;
        }
    }
}
