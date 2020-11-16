using System;
using System.Collections.Generic;

#nullable disable

namespace Accio.Data
{
    public partial class SetLanguage
    {
        public Guid SetLanguageId { get; set; }
        public Guid SetId { get; set; }
        public Guid LanguageId { get; set; }
        public bool Enabled { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
