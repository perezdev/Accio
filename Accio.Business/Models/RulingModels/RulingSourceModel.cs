using System;

namespace Accio.Business.Models.RulingModels
{
    public class RulingSourceModel
    {
        public Guid RulingSourceId { get; set; }
        public string Name { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
