using HpTcgCardBrowser.Business.Models.LanguageModels;
using System;

namespace HpTcgCardBrowser.Business.Models.CardModels
{
    public class CardDetailModel
    {
        public Guid CardDetailId { get; set; }
        public Guid CardId { get; set; }
        public LanguageModel Language { get; set; } = new LanguageModel();
        public string Name { get; set; }
        public string Text { get; set; }
        public string TextHtml { get; set; }
        public string Url { get; set; }
        public string FlavorText { get; set; }
        public string Illustrator { get; set; }
        public string Copyright { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
