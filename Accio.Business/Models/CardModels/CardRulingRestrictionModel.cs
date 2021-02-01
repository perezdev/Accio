using Accio.Business.Models.FormatModels;
using Accio.Business.Models.RulingRestrictionModels;
using System;

namespace Accio.Business.Models.CardModels
{
    public class CardRulingRestrictionModel
    {
        public Guid CardRulingRestrictionId { get; set; }
        public Guid CardId { get; set; }
        public RulingRestrictionModel RulingRestriction { get; set; }
        public FormatType Format { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
