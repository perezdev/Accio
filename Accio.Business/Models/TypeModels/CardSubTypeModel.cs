using System;

namespace Accio.Business.Models.TypeModels
{
    public class CardSubTypeModel
    {
        public Guid CardSubTypeId { get; set; }
        public SubTypeModel SubType { get; set; } = new SubTypeModel();
        public Guid CardId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
