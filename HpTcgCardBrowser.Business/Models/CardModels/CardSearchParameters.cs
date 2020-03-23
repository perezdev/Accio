using System;

namespace HpTcgCardBrowser.Business.Models.CardModels
{
    public class CardSearchParameters
    {
        public string SearchText { get; set; }
        public Guid? CardSetId { get; set; }
        public Guid? CardTypeId { get; set; }
        public Guid? CardRarityId { get; set; }
        public Guid? LanguageId { get; set; }
        public int? LessonCost { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
    }
}
