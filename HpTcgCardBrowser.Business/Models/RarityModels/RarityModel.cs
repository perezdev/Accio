using System;

namespace HpTcgCardBrowser.Business.Models.RarityModels
{
    public class RarityModel
    {
        public Guid RarityId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string ImageName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
