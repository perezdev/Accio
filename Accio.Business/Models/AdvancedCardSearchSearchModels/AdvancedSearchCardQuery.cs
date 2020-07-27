using Accio.Data;

namespace Accio.Business.Models.AdvancedCardSearchSearchModels
{
    public class AdvancedSearchCardQuery
    {
        public Card Card { get; set; }
        public Set Set { get; set; }
        public Rarity Rarity { get; set; }
        public CardType CardType { get; set; }
        public CardDetail CardDetail { get; set; }
        public Language Language { get; set; }
        public LessonType LessonType { get; set; }
        public LessonType CardProvidesLessonLessonType { get; set; }
        public CardProvidesLesson CardProvidesLesson { get; set; }
        public SubType SubType { get; set; }
    }
}
