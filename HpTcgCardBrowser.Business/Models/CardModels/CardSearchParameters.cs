using System;

namespace HpTcgCardBrowser.Business.Models.CardModels
{
    public class CardSearchParameters
    {
        public string SearchText { get; set; } = null;
        public Guid? SetId { get; set; } = null;
        public Guid? TypeId { get; set; } = null;
        public Guid? RarityId { get; set; } = null;
        public Guid? LanguageId { get; set; } = null;
        public int? LessonCost { get; set; } = null;
        public string SortBy { get; set; } = null;
        public string SortOrder { get; set; } = null;
        public Guid? PerformedByUserId { get; set; } = null;
    }
}
