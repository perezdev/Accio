using Accio.Business.Models.ImageModels;
using System;

namespace Accio.Business.Models.CardModels
{
    public class CardImageModel
    {
        public Guid CardImageId { get; set; }
        public ImageModel Image { get; set; }
        public Guid CardId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
