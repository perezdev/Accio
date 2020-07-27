namespace Accio.Business.Models.AdvancedCardSearchSearchModels
{
    public class AdvancedSearchFieldValue
    {
        public AdvancedSearchField Field { get; set; } = AdvancedSearchField.NONE;
        public AdvancedSearchFieldExpression Expression { get; set; } = AdvancedSearchFieldExpression.NONE;
        public string Value { get; set; }
        public string DatabaseColumnName { get; set; }
    }
}
