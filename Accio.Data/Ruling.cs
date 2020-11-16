using System;
using System.Collections.Generic;

#nullable disable

namespace Accio.Data
{
    public partial class Ruling
    {
        public Guid RulingId { get; set; }
        public Guid RulingTypeId { get; set; }
        public Guid RulingSourceId { get; set; }
        public Guid? CardTypeId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Ruling1 { get; set; }
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
