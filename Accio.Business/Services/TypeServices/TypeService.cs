using Accio.Business.Models.TypeModels;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.CardServices
{
    public class TypeService
    {
        private HpTcgContext _context { get; set; }

        public TypeService(HpTcgContext context)
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
            var adventureId = Guid.Parse("2BEB71E4-AAA2-40D7-81EB-F6BCE2AF9B16");
            var characterId = Guid.Parse("C4384FDE-508B-4FF3-B411-290E4E2C7A66");
            var creatureId = Guid.Parse("74F04C31-9957-4DE0-91B0-746B23C5705A");
            var itemId = Guid.Parse("9EC5EC6F-0283-4FAD-8BF0-1F18AEE11978");
            var lessonId = Guid.Parse("0CC59795-A56F-43E7-A2EC-56D55EBF4425");
            var locationId = Guid.Parse("8B6FE704-2954-4687-AC7C-769AEA8ADB49");
            var matchId = Guid.Parse("7DB36B51-0E8D-4DDE-B9B3-3A0F4D717E5D");
            var spellId = Guid.Parse("6040B58A-154D-40E8-AD6D-868A0B6BB2E8");

            if (cardTypeId == adventureId)
                return TypeOfCard.Adventure;
            else if (cardTypeId == characterId)
                return TypeOfCard.Character;
            else if (cardTypeId == creatureId)
                return TypeOfCard.Creature;
            else if (cardTypeId == itemId)
                return TypeOfCard.Item;
            else if (cardTypeId == lessonId)
                return TypeOfCard.Lesson;
            else if (cardTypeId == locationId)
                return TypeOfCard.Location;
            else if (cardTypeId == matchId)
                return TypeOfCard.Match;
            else if (cardTypeId == spellId)
                return TypeOfCard.Spell;
            else
            {
                throw new Exception($"Card type ID {cardTypeId} is not valid.");
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
    }
}
