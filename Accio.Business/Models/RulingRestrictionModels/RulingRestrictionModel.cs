using System;

namespace Accio.Business.Models.RulingRestrictionModels
{
    public class RulingRestrictionModel
    {
        public Guid RulingRestrictionId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public RulingRestrictionType Type { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
