using System;
using System.Collections.Generic;
using Accio.Business.Models.CardModels;
using Accio.Business.Models.SourceModels;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.SourceServices;
using Microsoft.AspNetCore.Mvc;

namespace Accio.Presentation.Web.API.Controllers.Cards.Search
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
        public IEnumerable<CardModel> Get(string text, Guid? setId, Guid? typeId, Guid? rarityId, Guid? languageId,
                                          int? lessonCost, string sortBy, string sortOrder)
        {
            var apiSource = _sourceService.GetSource(SourceType.API);
            var cardparams = new CardSearchParameters() 
            {
                SearchText = text,
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

        [HttpGet]
        [Route("All")]
        public IEnumerable<CardModel> Get()
        {
            var source = _sourceService.GetSource(SourceType.API);
            var cards = _cardService.GetAllCards(source);
            return cards;
        }
    }
}
