using System;
using System.Collections.Generic;

namespace Accio.Data
{
    public partial class CardDetailRuling
    {
        public Guid CardDetailRuleId { get; set; }
        public Guid CardDetailId { get; set; }
        public string Rule { get; set; }
        public DateTime? DateOfRuling { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
