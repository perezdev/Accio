using System;
using System.Collections.Generic;

#nullable disable

namespace Accio.Data
{
    public partial class Card
    {
        public Guid CardId { get; set; }
        public Guid? CardSetId { get; set; }
        public Guid? CardTypeId { get; set; }
        public Guid? CardRarityId { get; set; }
        public Guid? LessonTypeId { get; set; }
        public int? LessonCost { get; set; }
        public int? ActionCost { get; set; }
        public string CardNumber { get; set; }
        public string Orientation { get; set; }
        public int? Health { get; set; }
        public int? Damage { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
