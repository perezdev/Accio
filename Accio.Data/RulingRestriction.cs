using System;
using System.Collections.Generic;

#nullable disable

namespace Accio.Data
{
    public partial class RulingRestriction
    {
        public Guid RulingRestrictionId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
