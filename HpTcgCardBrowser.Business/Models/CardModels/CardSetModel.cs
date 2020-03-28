using System;

namespace HpTcgCardBrowser.Business.Models.CardModels
{
    public class CardSetModel
    {
        public Guid CardSetId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string IconFileName { get; set; }
        public int? Order { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
