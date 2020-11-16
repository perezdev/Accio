using Accio.Business.Models.TypeModels;
using Accio.Business.Services.TypeServices;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.CardServices
{
    public class CardSubTypeService
    {
        private AccioContext _context { get; set; }

        public CardSubTypeService(AccioContext context)
        {
            _context = context;
        }

        public List<CardSubTypeModel> GetCardSubTypes(Guid cardId)
        {
            var cardSubTypes = (from subType in _context.SubTypes
                                join cardSubType in _context.CardSubTypes on subType.SubTypeId equals cardSubType.SubTypeId
                                join card in _context.Cards on cardSubType.CardId equals card.CardId
                                where !subType.Deleted && !cardSubType.Deleted && !card.Deleted && card.CardId == cardId
                                select GetCardSubTypeModel(cardSubType, subType)).ToList();

            return cardSubTypes;
        }
        public List<CardSubTypeModel> GetAllCardSubTypes()
        {
            var cardSubTypes = (from subType in _context.SubTypes
                                join cardSubType in _context.CardSubTypes on subType.SubTypeId equals cardSubType.SubTypeId
                                join card in _context.Cards on cardSubType.CardId equals card.CardId
                                where !subType.Deleted && !cardSubType.Deleted && !card.Deleted
                                select GetCardSubTypeModel(cardSubType, subType)).ToList();

            return cardSubTypes;
        }

        public void PersistCardSubType(Guid cardId, Guid subTypeId)
        {
            var cardSubType = new CardSubType() 
            {
                CardSubTypeId = Guid.NewGuid(),
                CardId = cardId,
                SubTypeId = subTypeId,
                CreatedById = Guid.Empty,
                CreatedDate = DateTime.UtcNow,
                UpdatedById = Guid.Empty,
                UpdatedDate = DateTime.UtcNow,
                Deleted = false,
            };

            _context.CardSubTypes.Add(cardSubType);
            _context.SaveChanges();
        }

        public static CardSubTypeModel GetCardSubTypeModel(CardSubType cardSubType, SubType subType)
        {
            return new CardSubTypeModel()
            {
                CardSubTypeId = cardSubType.CardSubTypeId,
                CardId = cardSubType.CardId,
                SubType = SubTypeService.GetSubTypeModel(subType),
                CreatedById = cardSubType.CreatedById,
                CreatedDate = cardSubType.CreatedDate,
                UpdatedById = cardSubType.UpdatedById,
                UpdatedDate = cardSubType.UpdatedDate,
                Deleted = cardSubType.Deleted,
            };
        }

        //public List<>
    }
}
