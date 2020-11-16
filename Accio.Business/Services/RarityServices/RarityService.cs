using Accio.Business.Models.CardModels;
using Accio.Business.Models.RarityModels;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.CardServices
{
    public class RarityService
    {
        private Guid CommonRarityId { get; set; } = Guid.Parse("A2B927BD-4727-4EE6-9ED5-13337DAA2548");
        private Guid RareRarityId { get; set; } = Guid.Parse("D2B325E3-7933-4A4C-A65D-16E40E437200");
        private Guid UncommonRarityId { get; set; } = Guid.Parse("7A433764-E062-43BD-9DC4-5F30D8731B15");
        private Guid FoilPremiumRarityId { get; set; } = Guid.Parse("F250F6DB-1999-48E0-B371-77AC949E35F3");
        private Guid HoloPortraitPremiumRarityId { get; set; } = Guid.Parse("B3AE82F8-237E-41A1-B466-AD64E67A05A8");

        private string CommonRarityName { get; set; } = "Common";
        private string RareRarityName { get; set; } = "Rare";
        private string UncommonRarityName { get; set; } = "Uncommon";
        private string FoilPremiumRarityName { get; set; } = "Foil Premium";
        private string HoloPortraitPremiumRarityName { get; set; } = "Holo Portrait Premium";


        private AccioContext _context { get; set; }

        public RarityService(AccioContext context)
        {
            _context = context;
        }

        public List<RarityModel> GetCardRarities()
        {
            var rarities = (from set in _context.Rarities
                            where !set.Deleted
                            select GetRarityModel(set)).ToList();

            return rarities;
        }

        public RarityModel GetRarity(TypeOfRare type)
        {
            switch (type)
            {
                case TypeOfRare.Common:
                    return GetRarityModel(CommonRarityId, CommonRarityName);
                case TypeOfRare.Rare:
                    return GetRarityModel(RareRarityId, RareRarityName);
                case TypeOfRare.Uncommon:
                    return GetRarityModel(UncommonRarityId, UncommonRarityName);
                case TypeOfRare.FoilPremium:
                    return GetRarityModel(FoilPremiumRarityId, FoilPremiumRarityName);
                case TypeOfRare.HoloPortraitPremium:
                    return GetRarityModel(HoloPortraitPremiumRarityId, HoloPortraitPremiumRarityName);
                default:
                    throw new Exception("Rarity not recognized");
            }
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
        public static RarityModel GetRarityModel(Guid rarityId, string name)
        {
            return new RarityModel()
            {
                RarityId = rarityId,
                Name = name,
                Deleted = false,
            };
        }
    }
}
