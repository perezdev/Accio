using HpTcgCardBrowser.Business.Models.CardModels;
using HpTcgCardBrowser.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HpTcgCardBrowser.Business.Services.CardServices
{
    public class CardTypeService
    {
        private HpTcgContext _context { get; set; }

        public CardTypeService(HpTcgContext context)
        {
            _context = context;
        }

        public List<CardTypeModel> GetCardTypes()
        {
            var types = (from cardType in _context.CardType
                         where !cardType.Deleted
                         select GetCardTypeModel(cardType)).ToList();

            return types;
        }

        public static CardTypeModel GetCardTypeModel(CardType cardType)
        {
            return new CardTypeModel() 
            {
                CardTypeId = cardType.CardTypeId,
                Name = cardType.Name,
                CreatedById = cardType.CreatedById,
                CreatedDate = cardType.CreatedDate,
                UpdatedById = cardType.UpdatedById,
                UpdatedDate = cardType.UpdatedDate,
                Deleted = cardType.Deleted,
            };
        }
    }
}
