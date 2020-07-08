using Accio.Business.Models.CardModels;
using Accio.Business.Models.RarityModels;
using Accio.Data;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.CardServices
{
    public class RarityService
    {
        private AccioContext _context { get; set; }

        public RarityService(AccioContext context)
        {
            _context = context;
        }

        public List<RarityModel> GetCardRarities()
        {
            var rarities = (from set in _context.Rarity
                            where !set.Deleted
                            select GetRarityModel(set)).ToList();

            return rarities;
        }

        public static RarityModel GetRarityModel(Rarity cardRarity)
        {
            return new RarityModel()
            {
                RarityId = cardRarity.RarityId,
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
