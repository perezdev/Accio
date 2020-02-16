using HpTcgCardBrowser.Business.Models.CardModels;
using HpTcgCardBrowser.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HpTcgCardBrowser.Business.Services.CardServices
{
    public class CardSetService
    {
        private HpTcgContext _context { get; set; }

        public CardSetService(HpTcgContext context)
        {
            _context = context;
        }

        public List<CardSetModel> GetSets()
        {
            var sets = (from set in _context.CardSet
                        where !set.Deleted
                        orderby set.Order
                        select GetCardSetModel(set)).ToList();

            return sets;
        }

        public static CardSetModel GetCardSetModel(CardSet set)
        {
            return new CardSetModel()
            {
                CardSetId = set.CardSetId,
                Name = set.Name,
                Description = set.Description,
                IconFileName = set.IconFileName,
                Order = set.Order,
                CreatedById = set.CreatedById,
                CreatedDate = set.CreatedDate,
                UpdatedById = set.UpdatedById,
                UpdatedDate = set.UpdatedDate,
                Deleted = set.Deleted,
            };
        }
    }
}
