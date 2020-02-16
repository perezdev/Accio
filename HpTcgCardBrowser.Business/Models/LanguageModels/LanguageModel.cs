using System;

namespace HpTcgCardBrowser.Business.Models.LanguageModels
{
    public class LanguageModel
    {
        public Guid LanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string FlagImagePath { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
