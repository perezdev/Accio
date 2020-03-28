using HpTcgCardBrowser.Business.Models.CardModels;
using HpTcgCardBrowser.Data;
using System.Collections.Generic;
using System.Linq;

namespace HpTcgCardBrowser.Business.Services.CardServices
{
    public class CardRarityService
    {
        private HpTcgContext _context { get; set; }

        public CardRarityService(HpTcgContext context)
        {
            _context = context;
        }

        public List<CardRarityModel> GetCardRarities()
        {
            var rarities = (from set in _context.CardRarity
                            where !set.Deleted
                            select GetCardRarityModel(set)).ToList();

            return rarities;
        }

        public static CardRarityModel GetCardRarityModel(CardRarity cardRarity)
        {
            return new CardRarityModel()
            {
                CardRarityId = cardRarity.CardRarityId,
                Name = cardRarity.Name,
                Symbol = cardRarity.Symbol,
                ImageName = cardRarity.ImageName,
                CreatedById = cardRarity.CreatedById,
                CreatedDate = cardRarity.CreatedDate,
                UpdatedById = cardRarity.UpdatedById,
                UpdatedDate = cardRarity.UpdatedDate,
                Deleted = cardRarity.Deleted,
            };
        }
    }
}
