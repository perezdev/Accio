using Accio.Business.Models.CardModels;
using Accio.Business.Models.CardSearchHistoryModels;
using Accio.Data;
using System;

namespace Accio.Business.Services.CardSearchHistoryServices
{
    public class CardSearchHistoryService
    {
        private HpTcgContext _context { get; set; }

        public CardSearchHistoryService(HpTcgContext context)
        {
            _context = context;
        }

        public void PersistCardSearchHistory(CardSearchParameters searchParams, DateTime createdDateTime, DateTime updatedDateTime)
        {
            var cardSearchHistoryModel = GetCardSearchHistoryModel(searchParams, createdDateTime, updatedDateTime);
            var cardSearchHistory = GetCardSearchHistory(cardSearchHistoryModel);

            _context.CardSearchHistory.Add(cardSearchHistory);
            _context.SaveChanges();
        }

        private CardSearchHistoryModel GetCardSearchHistoryModel(CardSearchParameters searchParams, DateTime createdDateTime, DateTime updatedDateTime)
        {
            var userId = searchParams.PerformedByUserId == null ? Guid.Empty : (Guid)searchParams.PerformedByUserId;

            return new CardSearchHistoryModel()
            {
                CardId = searchParams.CardId,
                CardSearchHistoryId = Guid.NewGuid(),
                UserId = userId,
                SearchText = searchParams.SearchText,
                SetId = searchParams.SetId,
                CardTypeId = searchParams.TypeId,
                CardRarityId = searchParams.RarityId,
                LanguageId = searchParams.LanguageId,
                LessonCost = searchParams.LessonCost,
                SortBy = searchParams.SortBy,
                SortOrder = searchParams.SortOrder,
                SourceId = searchParams.SourceId,
                CreatedById = userId,
                CreatedDate = createdDateTime,
                UpdatedById = userId,
                UpdatedDate = updatedDateTime,
                Deleted = false,
            };
        }

        private CardSearchHistory GetCardSearchHistory(CardSearchHistoryModel cardSearchHistoryModel)
        {
            return new CardSearchHistory()
            {
                CardSearchHistoryId = cardSearchHistoryModel.CardSearchHistoryId,
                CardId = cardSearchHistoryModel.CardId,
                UserId = cardSearchHistoryModel.UserId,
                SearchText = cardSearchHistoryModel.SearchText,
                SetId = cardSearchHistoryModel.SetId,
                CardTypeId = cardSearchHistoryModel.CardTypeId,
                CardRarityId = cardSearchHistoryModel.CardRarityId,
                LanguageId = cardSearchHistoryModel.LanguageId,
                LessonCost = cardSearchHistoryModel.LessonCost,
                SortBy = cardSearchHistoryModel.SortBy,
                SortOrder = cardSearchHistoryModel.SortOrder,
                SourceId = cardSearchHistoryModel.SourceId,
                CreatedById = cardSearchHistoryModel.CreatedById,
                CreatedDate = cardSearchHistoryModel.CreatedDate,
                UpdatedById = cardSearchHistoryModel.UpdatedById,
                UpdatedDate = cardSearchHistoryModel.UpdatedDate,
                Deleted = cardSearchHistoryModel.Deleted,
            };
        }
    }


}
