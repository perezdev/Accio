using HpTcgCardBrowser.Business.Models.LessonModels;
using HpTcgCardBrowser.Business.Models.RarityModels;
using HpTcgCardBrowser.Business.Models.SetModels;
using HpTcgCardBrowser.Business.Models.TypeModels;
using System;

namespace HpTcgCardBrowser.Business.Models.CardModels
{
    public class CardModel
    {
        public Guid CardId { get; set; }
        public CardDetailModel Detail { get; set; } = new CardDetailModel();
        public SetModel CardSet { get; set; } = new SetModel();
        public CardTypeModel CardType { get; set; } = new CardTypeModel();
        public RarityModel Rarity { get; set; } = new RarityModel();
        public LessonTypeModel LessonType { get; set; } = new LessonTypeModel();
        public int? LessonCost { get; set; }
        public int? ActionCost { get; set; }
        public string CardNumber { get; set; }
        public string Orientation { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
