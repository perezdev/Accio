using Accio.Business.Models.CardModels;
using Accio.Business.Models.ImportModels;
using Accio.Business.Models.LanguageModels;
using Accio.Business.Models.LessonModels;
using Accio.Business.Models.RarityModels;
using Accio.Business.Models.SetModels;
using Accio.Business.Models.TypeModels;
using Accio.Business.Services.CardSearchHistoryServices;
using Accio.Business.Services.LanguageServices;
using Accio.Business.Services.LessonServices;
using Accio.Data;
using Microsoft.EntityFrameworkCore;
using NaturalSort.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Accio.Business.Services.CardServices
{
    public class CardService
    {
        private AccioContext _context { get; set; }
        private SetService _cardSetService { get; set; }
        private TypeService _cardTypeService { get; set; }
        private RarityService _cardRarityService { get; set; }
        private LanguageService _languageService { get; set; }
        private LessonService _lessonService { get; set; }
        private CardSearchHistoryService _cardSearchHistoryService { get; set; }
        private CardSubTypeService _cardSubTypeService { get; set; }
        private CardImageService _cardImageService { get; set; }

        public CardService(AccioContext context, SetService cardSetService, TypeService cardTypeService,
                           RarityService cardRarityService, LanguageService languageService, LessonService lessonService,
                           CardSearchHistoryService cardSearchHistoryService, CardSubTypeService cardSubTypeService,
                           CardImageService cardImageService)
        {
            _context = context;
            _cardSetService = cardSetService;
            _cardTypeService = cardTypeService;
            _cardRarityService = cardRarityService;
            _languageService = languageService;
            _lessonService = lessonService;
            _cardSearchHistoryService = cardSearchHistoryService;
            _cardSubTypeService = cardSubTypeService;
            _cardImageService = cardImageService;
        }

        public List<CardModel> SearchCards(CardSearchParameters cardSearchParameters)
        {
            var param = cardSearchParameters;
            var utcNow = DateTime.UtcNow;

            //We don't want them to pull all cards, so this will force them to search for a set and/or card text to prevent that
            if (string.IsNullOrEmpty(cardSearchParameters.SearchText) && (cardSearchParameters.SetId == null || cardSearchParameters.SetId == Guid.Empty) && string.IsNullOrWhiteSpace(cardSearchParameters.SetShortName))
            {
                return new List<CardModel>();
            }

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
                         join provides in _context.CardProvidesLesson on card.CardId equals provides.CardId into cardsProvidesLesson
                         from provides in cardsProvidesLesson.DefaultIfEmpty()
                         join plesson in _context.LessonType on provides.LessonId equals plesson.LessonTypeId into providesLesson
                         from plesson in providesLesson.DefaultIfEmpty()
                         join lessonType in _context.LessonType on card.LessonTypeId equals lessonType.LessonTypeId into lessonTypeDefault
                         from lessonType in lessonTypeDefault.DefaultIfEmpty()
                         where !card.Deleted && !cardSet.Deleted && !cardRarity.Deleted && !cardType.Deleted &&
                               language.LanguageId == param.LanguageId
                         select new
                         {
                             card,
                             cardDetail,
                             cardSet,
                             cardRarity,
                             cardType,
                             language,
                             lessonType,
                             provides,
                             plesson
                         });

            if (param.SetId != null && param.SetId != Guid.Empty)
            {
                cards = cards.Where(x => x.card.CardSetId == param.SetId);
            }

            if (!string.IsNullOrWhiteSpace(param.SetShortName))
            {
                cards = cards.Where(card => card.cardSet.ShortName == param.SetShortName);
            }

            if (param.TypeId != null && param.TypeId != Guid.Empty)
            {
                cards = cards.Where(x => x.card.CardTypeId == param.TypeId);
            }
            if (param.RarityId != null && param.RarityId != Guid.Empty)
            {
                cards = cards.Where(x => x.card.CardRarityId == param.RarityId);
            }
            if (param.LessonCost != null && param.LessonCost >= 0)
            {
                cards = cards.Where(x => x.card.LessonCost == param.LessonCost);
            }
            if (!string.IsNullOrEmpty(param.SearchText))
            {
                cards = from card in cards
                        where EF.Functions.Like(card.cardDetail.Name, $"%{param.SearchText}%") ||
                              EF.Functions.Like(card.cardDetail.Text, $"%{param.SearchText}%") ||
                              //We have to include these 3 fields for adventure cards and matches since they don't have card text
                              EF.Functions.Like(card.cardDetail.Effect, $"%{param.SearchText}%") ||
                              EF.Functions.Like(card.cardDetail.ToSolve, $"%{param.SearchText}%") ||
                              EF.Functions.Like(card.cardDetail.Reward, $"%{param.SearchText}%")
                        select card;
            }

            var cardModels = cards.Select(x => GetCardModel(x.card, x.cardSet, x.cardRarity, x.cardType, x.cardDetail,
                                                            x.language, x.lessonType, x.plesson, x.provides)).ToList();
            cardModels = GetCardsWithImages(cardModels);

            //This isn't ideal, but there aren't a ton of sub types and it's easier to just pull all and assign than to do a complicated join
            var cardSubTypeModels = _cardSubTypeService.GetAllCardSubTypes();
            foreach (var cardModel in cardModels)
            {
                var cardSubTypes = cardSubTypeModels.Where(x => x.CardId == cardModel.CardId).ToList();
                if (cardSubTypes != null && cardSubTypes.Count > 0)
                {
                    cardModel.SubTypes = cardSubTypes;
                }
            }

            _cardSearchHistoryService.PersistCardSearchHistory(param, utcNow, utcNow);

            if (cardModels?.Count > 1 && !string.IsNullOrEmpty(cardSearchParameters.SortBy) && !string.IsNullOrEmpty(cardSearchParameters.SortOrder))
            {
                cardModels = GetCardModelsSorted(cardModels, cardSearchParameters.SortBy, cardSearchParameters.SortOrder);
            }

            return cardModels != null ? cardModels : new List<CardModel>();
        }
        /// <summary>
        /// Gets the top 10 most popular cards based on which ones were clicked after performing a search
        /// </summary>
        public List<CardModel> GetMostPopularCardsFromSearchHistory()
        {
            var englishLanguageId = _languageService.GetLanguageId(TypeOfLanguage.English);
            var popularCardGuids = _cardSearchHistoryService.GetMostPopularSearchedCardIds();

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
                               language.LanguageId == englishLanguageId && popularCardGuids.Contains(card.CardId)
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
            var cardModels = cards.Select(x => GetCardModel(x.card, x.cardSet, x.cardRarity, x.cardType,
                                                            x.cardDetail, x.language, x.lessonType, x.plesson,
                                                            x.provides)).ToList();
            cardModels = GetCardsWithImages(cardModels);

            return cardModels;
        }
        public List<CardModel> GetRandomCards(int numberOfCards)
        {
            var englishLanguageId = _languageService.GetLanguageId(TypeOfLanguage.English);
            var randomCardIds = _context.Card.Where(x => x.Orientation == "Vertical").OrderBy(r => Guid.NewGuid()).Take(numberOfCards).Select(x => x.CardId).ToList();
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
                               language.LanguageId == englishLanguageId && randomCardIds.Contains(card.CardId)
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
            var cardModels = cards.Select(x => GetCardModel(x.card, x.cardSet, x.cardRarity, x.cardType,
                                                            x.cardDetail, x.language, x.lessonType, x.plesson,
                                                            x.provides)).ToList();
            cardModels = GetCardsWithImages(cardModels);

            return cardModels;
        }
        
        public List<CardModel> GetCardModelsSorted(List<CardModel> cards, string sortBy, string sortOrder)
        {
            if (sortBy == SortType.SetNumber)
            {
                if (sortOrder == SortOrder.Ascending)
                    return cards.OrderBy(x => x.CardSet.Name).ThenBy(x => x.CardNumber.PadLeft(3, '0'), StringComparison.OrdinalIgnoreCase.WithNaturalSort()).ToList();
                else if (sortOrder == SortOrder.Descending)
                    return cards.OrderByDescending(x => x.CardSet.Name).ThenByDescending(x => x.CardNumber).ToList();
                else
                    throw new Exception("Set and number were sorted by, but no valid sort order was passed in.");
            }
            else if (sortBy == SortType.Name)
            {
                if (sortOrder == SortOrder.Ascending)
                    return cards.OrderBy(x => x.Detail.Name).ToList();
                else if (sortOrder == SortOrder.Descending)
                    return cards.OrderByDescending(x => x.Detail.Name).ToList();
                else
                    throw new Exception("Name was sorted by, but no valid sort order was passed in.");
            }
            else if (sortBy == SortType.Cost)
            {
                if (sortOrder == SortOrder.Ascending)
                    return cards.OrderBy(x => x.LessonCost).ToList();
                else if (sortOrder == SortOrder.Descending)
                    return cards.OrderByDescending(x => x.LessonCost).ToList();
                else
                    throw new Exception("Lesson cost was sorted by, but no valid sort order was passed in.");
            }
            else if (sortBy == SortType.Type)
            {
                if (sortOrder == SortOrder.Ascending)
                    return cards.OrderBy(x => x.CardType.Name).ToList();
                else if (sortOrder == SortOrder.Descending)
                    return cards.OrderByDescending(x => x.CardType.Name).ToList();
                else
                    throw new Exception("Card type was sorted by, but no valid sort order was passed in.");
            }
            else if (sortBy == SortType.Rarity)
            {
                if (sortOrder == SortOrder.Ascending)
                    return cards.OrderBy(x => x.Rarity.Name).ToList();
                else if (sortOrder == SortOrder.Descending)
                    return cards.OrderByDescending(x => x.Rarity.Name).ToList();
                else
                    throw new Exception("Rarity was sorted by, but no valid sort order was passed in.");
            }
            else if (sortBy == SortType.Artist)
            {
                if (sortOrder == SortOrder.Ascending)
                    return cards.OrderBy(x => x.Detail.Illustrator).ToList();
                else if (sortOrder == SortOrder.Descending)
                    return cards.OrderByDescending(x => x.Detail.Illustrator).ToList();
                else
                    throw new Exception("Artist was sorted by, but no valid sort order was passed in.");
            }
            else if (sortBy == SortType.Lesson)
            {
                if (sortOrder == SortOrder.Ascending)
                    return cards.OrderBy(x => x.LessonType?.Name).ToList();
                else if (sortOrder == SortOrder.Descending)
                    return cards.OrderByDescending(x => x.LessonType?.Name).ToList();
                else
                    throw new Exception("Lesson type was sorted by, but no valid sort order was passed in.");
            }
            else
            {
                throw new Exception("Sort by option was passed into the method, but was invalid.");
            }
        }

        public List<CardModel> GetAllCards()
        {
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
                         where !card.Deleted && !cardDetail.Deleted && !language.Deleted && !cardSet.Deleted && !cardRarity.Deleted &&
                               !cardType.Deleted
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

            var cardModels = cards.Select(x => GetCardModel(x.card, x.cardSet, x.cardRarity, x.cardType,
                                                            x.cardDetail, x.language, x.lessonType,
                                                            x.plesson, x.provides)).ToList();

            return cardModels != null ? cardModels : new List<CardModel>();
        }
        public void ImportCardsFromSets(List<ImportSetModel> sets)
        {
            var cardCache = GetAllCards();
            var setCache = _cardSetService.GetSets();
            var cardTypeCache = _cardTypeService.GetCardTypes();
            var rarityCache = _cardRarityService.GetCardRarities();
            var lessonCache = _lessonService.GetLessonTypes();

            foreach (var set in sets)
            {
                foreach (var importCard in set.Cards)
                {
                    //Between the JSON and my DB, the only way to determine if a card is a duplicate, is to compare
                    //the names between both objects
                    var cardExists = cardCache.Any(x => x.Detail.Name.ToLower() == importCard.Name.ToLower());
                    if (!cardExists)
                    {
                        var card = GetCard(importCard, set.Name, setCache, cardTypeCache, rarityCache, lessonCache);
                        var cardDetail = GetCardDetail(card.CardId, importCard);

                        _context.Card.Add(card);
                        _context.CardDetail.Add(cardDetail);
                    }
                }
            }

            _context.SaveChanges();
        }

        public static CardModel GetCardModel(Card card, Set cardSet, Rarity cardRarity, CardType cardType, CardDetail cardDetail,
                                             Language language, LessonType lessonType, LessonType providesLesson,
                                             CardProvidesLesson cardProvidesLesson)
        {
            return new CardModel()
            {
                CardId = card.CardId,
                CardSet = SetService.GetSetModel(cardSet),
                CardType = TypeService.GetCardTypeModel(cardType),
                Rarity = RarityService.GetRarityModel(cardRarity),
                Detail = CardDetailService.GetCardDetailModel(cardDetail, language),
                LessonType = lessonType == null ? null : LessonService.GetLessonTypeModel(lessonType),
                LessonCost = card.LessonCost,
                ActionCost = card.ActionCost,
                CardNumber = card.CardNumber,
                Orientation = card.Orientation,
                ProvidesLesson = providesLesson == null && cardProvidesLesson == null ? null :
                                 CardProvidesLessonService.GetCardProvidesLessonModel(cardProvidesLesson, providesLesson),
                CardPageUrl = $"/Card/{cardSet.ShortName}/{card.CardNumber}/{cardDetail.Name.Replace(" ", "-")}",
                CreatedById = card.CreatedById,
                CreatedDate = card.CreatedDate,
                UpdatedById = card.UpdatedById,
                UpdatedDate = card.UpdatedDate,
                Deleted = card.Deleted,
            };
        }
        public Card GetCard(ImportCardModel importCardModel, string setName, List<SetModel> cardSetCache, List<CardTypeModel> cardTypesCache,
                            List<RarityModel> raritiesCache, List<LessonTypeModel> lessonTypeCache)
        {
            /*
             * As a general rule, each item's (set, type, raritiy), name should match up between the DB and the JSON.
             */

            var cardSet = cardSetCache.SingleOrDefault(x => x.Name.ToLower() == setName?.ToLower());
            var cardType = cardTypesCache.SingleOrDefault(x => x.Name.ToLower() == importCardModel.Type?.ToLower());
            var cardRarity = raritiesCache.SingleOrDefault(x => x.Name.ToLower() == importCardModel.Rarity?.ToLower());
            var lessonType = lessonTypeCache.SingleOrDefault(x => x.Name.ToLower() == importCardModel.LessonType?.ToLower());

            //Cards typically cost 1 action. But some cards cost more. From the official game, only adventures and characters
            //cost 2 actions. We don't want to assume the card costs 1 action because someone might screw up and forget to assign
            //a type to a card with more than 1 action. In the website, we'll hide the action cost if it's null.
            int? actionCost = null;
            if (cardType != null)
            {
                var typeOfCard = _cardTypeService.GetTypeOfCard(cardType.CardTypeId);
                if (typeOfCard == TypeOfCard.Adventure || typeOfCard == TypeOfCard.Character)
                    actionCost = 2;
                else
                    actionCost = 1;
            }

            return new Card()
            {
                CardId = Guid.NewGuid(),
                CardSetId = cardSet?.SetId,
                CardTypeId = cardType?.CardTypeId,
                CardRarityId = cardRarity?.RarityId,
                LessonTypeId = lessonType?.LessonTypeId,
                LessonCost = importCardModel.Cost == null ? null : (int?)Convert.ToInt32(importCardModel.Cost),
                ActionCost = actionCost,
                CardNumber = importCardModel.Number,
                Orientation = "Vertical", //Most cards are vertical. We'll set this to vertical by default and change it manually later
                CreatedById = Guid.Empty,
                CreatedDate = DateTime.UtcNow,
                UpdatedById = Guid.Empty,
                UpdatedDate = DateTime.UtcNow,
                Deleted = false,
            };
        }
        public CardDetail GetCardDetail(Guid cardId, ImportCardModel importCardModel)
        {
            //English is the only language for the moment
            var languageId = _languageService.GetLanguageId(TypeOfLanguage.English);

            return new CardDetail()
            {
                CardDetailId = Guid.NewGuid(),
                CardId = cardId,
                LanguageId = languageId,
                Name = importCardModel.Name,
                Text = importCardModel.Description.Text,
                Effect = importCardModel.Description.Effect,
                ToSolve = importCardModel.Description.ToSolve,
                Reward = importCardModel.Description.Reward,
                FlavorText = importCardModel.FlavorText,
                Illustrator = importCardModel.Artists != null ? string.Join(", ", importCardModel.Artists) : null,
                Copyright = null, //Copyright isn't maintained in the JSON
                CreatedById = Guid.Empty,
                CreatedDate = DateTime.UtcNow,
                UpdatedById = Guid.Empty,
                UpdatedDate = DateTime.UtcNow,
                Deleted = false,
            };
        }

        /// <summary>
        /// The random page will redirect to the single card display page. So we can save sql resources by just grabbing the ID
        /// </summary>
        public Guid GetRandomCardId()
        {
            //I have no idea how ordering by a random GUID produces a random row, but, it works - https://stackoverflow.com/a/7781899/1339826
            var randomCard = _context.Card.OrderBy(r => Guid.NewGuid()).Take(1).Single();
            return randomCard.CardId;
        }

        public void UpdateMatchData(Guid cardId, string toSolve, string prize)
        {
            var card = _context.CardDetail.Single(x => x.CardId == cardId);
            card.ToSolve = toSolve;
            card.Reward = prize;
            _context.SaveChanges();
        }

        public List<CardModel> GetCardsWithImages(List<CardModel> cards)
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
    }
}
