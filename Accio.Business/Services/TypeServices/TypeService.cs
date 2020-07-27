using Accio.Business.Models.TypeModels;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.CardServices
{
    public class TypeService
    {
        private AccioContext _context { get; set; }

        private Guid AdventureId { get; set; } = Guid.Parse("2BEB71E4-AAA2-40D7-81EB-F6BCE2AF9B16");
        private Guid CharacterId { get; set; } = Guid.Parse("C4384FDE-508B-4FF3-B411-290E4E2C7A66");
        private Guid CreatureId { get; set; } = Guid.Parse("74F04C31-9957-4DE0-91B0-746B23C5705A");
        private Guid ItemId { get; set; } = Guid.Parse("9EC5EC6F-0283-4FAD-8BF0-1F18AEE11978");
        private Guid LessonId { get; set; } = Guid.Parse("0CC59795-A56F-43E7-A2EC-56D55EBF4425");
        private Guid LocationId { get; set; } = Guid.Parse("8B6FE704-2954-4687-AC7C-769AEA8ADB49");
        private Guid MatchId { get; set; } = Guid.Parse("7DB36B51-0E8D-4DDE-B9B3-3A0F4D717E5D");
        private Guid SpellId { get; set; } = Guid.Parse("6040B58A-154D-40E8-AD6D-868A0B6BB2E8");

        private string AdventureName { get; set; } = "Adventure";
        private string CharacterName { get; set; } = "Character";
        private string CreatureName { get; set; } = "Creature";
        private string ItemName { get; set; } = "Item";
        private string LessonName { get; set; } = "Lesson";
        private string LocationName { get; set; } = "Location";
        private string MatchName { get; set; } = "Match";
        private string SpellName { get; set; } = "Spell";

        public TypeService(AccioContext context)
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
        public TypeOfCard GetTypeOfCard(Guid cardTypeId)
        {
            if (cardTypeId == AdventureId)
                return TypeOfCard.Adventure;
            else if (cardTypeId == CharacterId)
                return TypeOfCard.Character;
            else if (cardTypeId == CreatureId)
                return TypeOfCard.Creature;
            else if (cardTypeId == ItemId)
                return TypeOfCard.Item;
            else if (cardTypeId == LessonId)
                return TypeOfCard.Lesson;
            else if (cardTypeId == LocationId)
                return TypeOfCard.Location;
            else if (cardTypeId == MatchId)
                return TypeOfCard.Match;
            else if (cardTypeId == SpellId)
                return TypeOfCard.Spell;
            else
            {
                throw new Exception($"Card type ID {cardTypeId} is not valid.");
            }
        }

        public CardTypeModel GetCardType(TypeOfCard typeOfCard)
        {
            switch (typeOfCard)
            {
                case TypeOfCard.Adventure:
                    return GetCardTypeModel(AdventureId, AdventureName);
                case TypeOfCard.Character:
                    return GetCardTypeModel(CharacterId, CharacterName);
                case TypeOfCard.Creature:
                    return GetCardTypeModel(CreatureId, CreatureName);
                case TypeOfCard.Item:
                    return GetCardTypeModel(ItemId, ItemName);
                case TypeOfCard.Lesson:
                    return GetCardTypeModel(LessonId, LessonName);
                case TypeOfCard.Location:
                    return GetCardTypeModel(LocationId, LocationName);
                case TypeOfCard.Match:
                    return GetCardTypeModel(MatchId, MatchName);
                case TypeOfCard.Spell:
                    return GetCardTypeModel(SpellId, SpellName);
                default:
                    throw new Exception("Card type not recognized.");
            }
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
        public static CardTypeModel GetCardTypeModel(Guid cardTypeId, string name)
        {
            return new CardTypeModel()
            {
                CardTypeId = cardTypeId,
                Name = name,
                Deleted = false,
            };
        }
    }
}
