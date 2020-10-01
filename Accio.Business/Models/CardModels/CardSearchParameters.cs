using System;

namespace Accio.Business.Models.CardModels
{
    public class CardSearchParameters
    {
        public Guid? CardId { get; set; } = null;
        public string SearchText { get; set; } = null;
        public Guid? SetId { get; set; } = null;
        public string SetShortName { get; set; } = string.Empty;
        public Guid? TypeId { get; set; } = null;
        public Guid? RarityId { get; set; } = null;
        public Guid? LanguageId { get; set; } = null;
        public int? LessonCost { get; set; } = null;
        public string SortBy { get; set; } = null;
        public string SortOrder { get; set; } = null;
        public Guid? PerformedByUserId { get; set; } = null;
        public Guid SourceId { get; set; }
    }
}
