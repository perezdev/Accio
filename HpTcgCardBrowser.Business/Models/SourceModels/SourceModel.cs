using System;

namespace HpTcgCardBrowser.Business.Models.SourceModels
{
    public class SourceModel
    {
        public Guid SourceId { get; set; }
        public string Name { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
