using System;

namespace HpTcgCardBrowser.Business.Models.CardSearchHistoryModels
{
    public class CardSearchHistoryModel
    {
        public Guid CardSearchHistoryId { get; set; }
        public Guid? UserId { get; set; }
        public string SearchText { get; set; }
        public Guid? SetId { get; set; }
        public Guid? CardTypeId { get; set; }
        public Guid? CardRarityId { get; set; }
        public Guid? LanguageId { get; set; }
        public int? LessonCost { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public Guid SourceId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
