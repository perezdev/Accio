using System;

namespace Accio.Business.Models.CardModels
{
    public class CardRulingModel
    {
        public Guid CardRulingId { get; set; }
        public Guid? CardId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
