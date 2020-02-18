using HpTcgCardBrowser.Business.Models.LessonModels;
using System;

namespace HpTcgCardBrowser.Business.Models.CardModels
{
    public class CardModel
    {
        public Guid CardId { get; set; }
        public CardDetailModel Detail { get; set; } = new CardDetailModel();
        public CardSetModel CardSet { get; set; } = new CardSetModel();
        public CardTypeModel CardType { get; set; } = new CardTypeModel();
        public CardRarityModel Rarity { get; set; } = new CardRarityModel();
        public LessonTypeModel LessonType { get; set; } = new LessonTypeModel();
        public int? LessonCost { get; set; }
        public int? ActionCost { get; set; }
        public string CardNumber { get; set; }
        public string CssSizeClass { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
