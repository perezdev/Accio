using Accio.Business.Models.CardModels;
using Accio.Business.Models.LanguageModels;
using Accio.Business.Services.CardSearchHistoryServices;
using Accio.Business.Services.LanguageServices;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.CardServices
{
    public class SingleCardService
    {
        private AccioContext _context { get; set; }
        private TypeService _cardTypeService { get; set; }
        private LanguageService _languageService { get; set; }
        private CardSearchHistoryService _cardSearchHistoryService { get; set; }
        private CardSubTypeService _cardSubTypeService { get; set; }
        private CardImageService _cardImageService { get; set; }

        public SingleCardService(AccioContext context, TypeService cardTypeService, LanguageService languageService,
                                 CardSearchHistoryService cardSearchHistoryService, CardSubTypeService cardSubTypeService,
                                 CardImageService cardImageService)
        {
            _context = context;
            _cardTypeService = cardTypeService;
            _languageService = languageService;
            _cardSearchHistoryService = cardSearchHistoryService;
            _cardSubTypeService = cardSubTypeService;
            _cardImageService = cardImageService;
        }

        public CardModel GetCard(SingleCardParameters singleCardParameters)
        {
            var param = singleCardParameters;
            var utcNow = DateTime.UtcNow;

            if (param.LanguageId == null || param.LanguageId == Guid.Empty)
            {
                var englishLanguageId = _languageService.GetLanguageId(TypeOfLanguage.English);
                param.LanguageId = englishLanguageId;
            }

            var cards = (from card in _context.Card
                         join cardDetail in _context.CardDetail on card.CardId equals cardDetail.CardId
                         join language in _context.Language on cardDetail.LanguageId equals language.LanguageId
                         join cardSet in _context.Set on card.CardSetId equals cardSet.SetId
                         join cardRarity in _context.Rarity on card.CardRarityId equals cardRarity.RarityId
                         join cardType in _context.CardType on card.CardTypeId equals cardType.CardTypeId
                         join lessonType in _context.LessonType on card.LessonTypeId equals lessonType.LessonTypeId into lessonTypeDefault
                         from lessonType in lessonTypeDefault.DefaultIfEmpty()
                         join provides in _context.CardProvidesLesson on card.CardId equals provides.CardId into cardsProvidesLesson
                         from provides in cardsProvidesLesson.DefaultIfEmpty()
                         join plesson in _context.LessonType on provides.LessonId equals plesson.LessonTypeId into providesLesson
                         from plesson in providesLesson.DefaultIfEmpty()
                         where !card.Deleted && !cardSet.Deleted && !cardRarity.Deleted && !cardType.Deleted &&
                               language.LanguageId == param.LanguageId && cardSet.ShortName == param.SetShortName &&
                               card.CardNumber == param.CardNumber && cardDetail.Name == param.CardName
                         select new
                         {
                             card,
                             cardDetail,
                             cardSet,
                             cardRarity,
                             cardType,
                             language,
                             lessonType,
                             plesson,
                             provides
                         });

            var cardModel = cards.Select(x => CardService.GetCardModel(x.card, x.cardSet, x.cardRarity, x.cardType, x.cardDetail,
                                                           x.language, x.lessonType, x.plesson, x.provides)).SingleOrDefault();
            if (cardModel != null)
            {
                cardModel.SubTypes = _cardSubTypeService.GetCardSubTypes(cardModel.CardId);
                cardModel = GetCardsWithImages(new List<CardModel>() { cardModel })[0];
                cardModel.MetaDescription = GetMetaDescription(cardModel);

                _cardSearchHistoryService.PersistCardSearchHistory(param, utcNow, utcNow);
            }

            return cardModel;
        }

        private List<CardModel> GetCardsWithImages(List<CardModel> cards)
        {
            var cardImages = _cardImageService.GetCardImages(cards.Select(x => x.CardId).ToList());
            foreach (var card in cards)
            {
                var images = cardImages.Where(x => x.CardId == card.CardId).ToList();
                if (images?.Count > 0)
                {
                    card.Images = images.Select(x => x.Image).ToList();
                }
            }

            return cards;
        }

        private string GetMetaDescription(CardModel card)
        {
            var desc = $"{card.CardType.Name}";

            if (card.SubTypes.Count > 0)
            {
                desc += $" - {string.Join(" - ", card.SubTypes.Select(x => x.SubType.Name).ToList())} • ";
            }

            if (!desc.EndsWith(" • "))
            {
                desc += " • ";
            }

            var toc = _cardTypeService.GetTypeOfCard(card.CardType.CardTypeId);
            switch (toc)
            {
                
                case Models.TypeModels.TypeOfCard.Adventure:
                case Models.TypeModels.TypeOfCard.Match:
                    desc += $"{card.Detail.Effect} {card.Detail.ToSolve} {card.Detail.Reward} • ";
                    
                    break;
                case Models.TypeModels.TypeOfCard.Character:
                case Models.TypeModels.TypeOfCard.Creature:
                case Models.TypeModels.TypeOfCard.Item:
                case Models.TypeModels.TypeOfCard.Lesson:
                case Models.TypeModels.TypeOfCard.Location:
                case Models.TypeModels.TypeOfCard.Spell:
                    desc += $"{card.Detail.Text} • ";

                    break;
            }

            desc += $"#{card.CardNumber} • Illustrated by {card.Detail.Illustrator} • Harry Potter TCG";

            return desc;
        }
    }
}
