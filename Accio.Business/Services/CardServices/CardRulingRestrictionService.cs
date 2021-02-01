using Accio.Business.Models.CardModels;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Accio.Business.Services.RulingRestrictionServices;
using Accio.Business.Services.FormatServices;

namespace Accio.Business.Services.CardServices
{
    public class CardRulingRestrictionService
    {
        private AccioContext _context { get; set; }
        private static FormatService _formatService { get; set; }

        public CardRulingRestrictionService(AccioContext context, FormatService formatService)
        {
            _context = context;
            _formatService = formatService;
        }

        public List<CardRulingRestrictionModel> GetCardRulingRestrictionsByCardId(Guid cardId)
        {
            var restrictions = (from cardRulingRestriction in _context.CardRulingRestrictions
                                join rulingRestriction in _context.RulingRestrictions on cardRulingRestriction.RulingRestrictionId equals rulingRestriction.RulingRestrictionId
                                where cardRulingRestriction.CardId == cardId
                                select GetCardRulingRestrictionModel(cardRulingRestriction, rulingRestriction)).ToList();

            return restrictions;
        }

        public static CardRulingRestrictionModel GetCardRulingRestrictionModel(CardRulingRestriction cardRulingRestriction, RulingRestriction rulingRestriction)
        {
            return new CardRulingRestrictionModel()
            {
                CardRulingRestrictionId = cardRulingRestriction.CardRulingRestrictionId,
                CardId = cardRulingRestriction.CardId,
                RulingRestriction = RulingRestrictionService.GetRulingRestrictionModel(rulingRestriction),
                Format = _formatService.GetFormatTypeById(cardRulingRestriction.FormatId),
                CreatedById = cardRulingRestriction.CreatedById,
                CreatedDate = cardRulingRestriction.CreatedDate,
                UpdatedById = cardRulingRestriction.UpdatedById,
                UpdatedDate = cardRulingRestriction.UpdatedDate,
                Deleted = cardRulingRestriction.Deleted,
            };
        }
    }
}
