using System;
using System.Collections.Generic;

#nullable disable

namespace Accio.Data
{
    public partial class CardSubType
    {
        public Guid CardSubTypeId { get; set; }
        public Guid SubTypeId { get; set; }
        public Guid CardId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
