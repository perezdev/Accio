using Accio.Business.Models.LanguageModels;
using Accio.Business.Models.RulingModels;
using Accio.Business.Services.LanguageServices;
using Accio.Business.Services.RulingServices;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.CardServices
{
    public class CardRulingService
    {
        private HpTcgContext _context { get; set; }
        private LanguageService _languageService { get; set; }

        public CardRulingService(HpTcgContext context, LanguageService languageService)
        {
            _context = context;
            _languageService = languageService;
        }

        /// <summary>
        /// Get card rules that only apply to the card. Defaults to English rules.
        /// </summary>
        public List<RulingModel> GetCardRules(Guid cardId, Guid? languageId = null)
        {
            if (languageId == null || languageId == Guid.Empty)
            {
                var englishLanguageId = _languageService.GetLanguageId(TypeOfLanguage.English);
                languageId = englishLanguageId;
            }

            //Grab rules that apply to the card
            var rules = (from ruling in _context.Ruling
                          join source in _context.RulingSource on ruling.RulingSourceId equals source.RulingSourceId
                          join type in _context.RulingType on ruling.RulingTypeId equals type.RulingTypeId
                          join cardRuling in _context.CardRuling on ruling.RulingId equals cardRuling.RulingId
                          where !ruling.Deleted && !source.Deleted && !type.Deleted && !cardRuling.Deleted &&
                                cardRuling.CardId == cardId && ruling.LanguageId == languageId
                          select RulingService.GetRulingModel(ruling, source, type, null)).ToList();

            return rules;
        }

        /// <summary>
        /// Get all rules directly for the card as well as rules that apply to the card type. Defaults to English rules.
        /// </summary>
        public List<RulingModel> GetCardRules(Guid cardId, Guid cardTypeId, Guid? languageId = null)
        {
            var cardRuleModels = new List<RulingModel>();

            /*
             * Different ruling types have different rules on how we pull them from the DB. So we'll
             * do multiple queries to pull the rules. It's less complicated than trying to do a complex
             * join query
             */

            if (languageId == null || languageId == Guid.Empty)
            {
                var englishLanguageId = _languageService.GetLanguageId(TypeOfLanguage.English);
                languageId = englishLanguageId;
            }

            //Grab rules that apply to the card
            var rules1 = (from ruling in _context.Ruling
                          join source in _context.RulingSource on ruling.RulingSourceId equals source.RulingSourceId
                          join type in _context.RulingType on ruling.RulingTypeId equals type.RulingTypeId
                          join cardRuling in _context.CardRuling on ruling.RulingId equals cardRuling.RulingId
                          where !ruling.Deleted && !source.Deleted && !type.Deleted && !cardRuling.Deleted &&
                                cardRuling.CardId == cardId && ruling.LanguageId == languageId
                          select RulingService.GetRulingModel(ruling, source, type, null)).ToList();
            if (rules1 != null)
            {
                cardRuleModels.AddRange(rules1);
            }


            //Grab rules that apply to the card type
            var rules2 = (from ruling in _context.Ruling
                          join source in _context.RulingSource on ruling.RulingSourceId equals source.RulingSourceId
                          join type in _context.RulingType on ruling.RulingTypeId equals type.RulingTypeId
                          join cardType in _context.CardType on ruling.CardTypeId equals cardType.CardTypeId
                          where !ruling.Deleted && !source.Deleted && !type.Deleted && !cardType.Deleted &&
                                 cardType.CardTypeId == cardTypeId && ruling.LanguageId == languageId
                          select RulingService.GetRulingModel(ruling, source, type, cardType)).ToList();
            if (rules2 != null)
            {
                cardRuleModels.AddRange(rules2);
            }

            return cardRuleModels;
        }
    }
}
