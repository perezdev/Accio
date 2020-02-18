using HpTcgCardBrowser.Business.Models.CardModels;
using HpTcgCardBrowser.Business.Models.LanguageModels;
using HpTcgCardBrowser.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HpTcgCardBrowser.Business.Services.CardServices
{
    public class CardService
    {
        private HpTcgContext _context { get; set; }

        public CardService(HpTcgContext context)
        {
            _context = context;
        }

        public List<CardModel> SearchCards(Guid? cardSetId, Guid? cardTypeId, Guid? cardRarityId, Guid languageId, int? lessonCost, string searchText)
        {
            var cards = (from card in _context.Card
                         join cardDetail in _context.CardDetail on card.CardId equals cardDetail.CardId
                         join language in _context.Language on cardDetail.LanguageId equals language.LanguageId
                         join cardSet in _context.CardSet on card.CardSetId equals cardSet.CardSetId
                         join cardRarity in _context.CardRarity on card.CardRarityId equals cardRarity.CardRarityId
                         join cardType in _context.CardType on card.CardTypeId equals cardType.CardTypeId
                         where !card.Deleted && !cardSet.Deleted && !cardRarity.Deleted && !cardType.Deleted &&
                               language.LanguageId == languageId
                         select new
                         {
                             card,
                             cardDetail,
                             cardSet,
                             cardRarity,
                             cardType,
                             language
                         });

            if (cardSetId != null && cardSetId != Guid.Empty)
            {
                cards = cards.Where(x => x.card.CardSetId == cardSetId);
            }
            if (cardTypeId != null && cardTypeId != Guid.Empty)
            {
                cards = cards.Where(x => x.card.CardTypeId == cardTypeId);
            }
            if (cardRarityId != null && cardRarityId != Guid.Empty)
            {
                cards = cards.Where(x => x.card.CardRarityId == cardRarityId);
            }
            if (lessonCost != null && lessonCost >= 0)
            {
                cards = cards.Where(x => x.card.LessonCost == lessonCost);
            }
            if (!string.IsNullOrEmpty(searchText))
            {
                cards = from card in cards
                        where EF.Functions.Like(card.cardDetail.Name, $"%{searchText}%") || EF.Functions.Like(card.cardDetail.Text, $"%{searchText}%")
                        select card;
            }

            var cardModels = cards.Select(x => GetCardModel(x.card, x.cardSet, x.cardRarity, x.cardType, x.cardDetail, x.language)).ToList();

            return cardModels != null ? cardModels : new List<CardModel>();
        }

        public static CardModel GetCardModel(Card card, CardSet cardSet, CardRarity cardRarity, CardType cardType, CardDetail cardDetail, Language language)
        {
            //TODO: Add lesson type
            return new CardModel()
            {
                CardId = card.CardId,
                CardSet = CardSetService.GetCardSetModel(cardSet),
                CardType = CardTypeService.GetCardTypeModel(cardType),
                Rarity = CardRarityService.GetCardRarityModel(cardRarity),
                Detail = new CardDetailModel()
                {
                    CardDetailId = cardDetail.CardDetailId,
                    CardId = cardDetail.CardId,
                    Language = new LanguageModel()
                    {
                        LanguageId = language.LanguageId,
                        Name = language.Name,
                        Code = language.Code,
                        FlagImagePath = language.FlagImagePath,
                        CreatedById = language.CreatedById,
                        CreatedDate = language.CreatedDate,
                        UpdatedById = language.UpdatedById,
                        UpdatedDate = language.UpdatedDate,
                        Deleted = language.Deleted,
                    },
                    Name = cardDetail.Name,
                    Text = cardDetail.Text,
                    TextHtml = cardDetail.TextHtml,
                    Url = cardDetail.Url,
                    FlavorText = cardDetail.FlavorText,
                    Illustrator = cardDetail.Illustrator,
                    Copyright = cardDetail.Copyright,
                    CreatedById = cardDetail.CreatedById,
                    CreatedDate = cardDetail.CreatedDate,
                    UpdatedById = cardDetail.UpdatedById,
                    UpdatedDate = cardDetail.UpdatedDate,
                    Deleted = cardDetail.Deleted,
                },
                CardNumber = card.CardNumber,
                CssSizeClass = card.CssSizeClass,
                CreatedById = card.CreatedById,
                CreatedDate = card.CreatedDate,
                UpdatedById = card.UpdatedById,
                UpdatedDate = card.UpdatedDate,
                Deleted = card.Deleted,
            };
        }
    }
}
