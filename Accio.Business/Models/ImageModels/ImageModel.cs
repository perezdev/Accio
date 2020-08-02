using Accio.Business.Models.LanguageModels;
using System;

namespace Accio.Business.Models.ImageModels
{
    public class ImageModel
    {
        public Guid ImageId { get; set; }
        public LanguageModel Language { get; set; }
        public string Url { get; set; }
        public ImageSizeType Size { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
