using Accio.Business.Models.TypeModels;
using System;

namespace Accio.Business.Models.RulingModels
{
    public class RulingModel
    {
        public Guid RulingId { get; set; }
        public RulingTypeModel RulingType { get; set; } = new RulingTypeModel();
        public RulingSourceModel RulingSource { get; set; } = new RulingSourceModel();
        public CardTypeModel CardType { get; set; } = new CardTypeModel();
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Ruling { get; set; }
        public string GeneralInfo { get; set; }
        public DateTime? RulingDate { get; set; }
        public Guid? LanguageId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
