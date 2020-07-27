using System;

namespace Accio.Business.Models.AdvancedCardSearchSearchModels
{
    public class AdvancedSearchParameters
    {
        public string AdvancedSearchText { get; set; } = null;
        public string SortBy { get; set; } = null;
        public string SortOrder { get; set; } = null;
        public Guid? LanguageId { get; set; } = null;
    }
}
