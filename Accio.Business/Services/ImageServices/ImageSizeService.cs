using Accio.Business.Models.ImageModels;
using Accio.Data;
using System;

namespace Accio.Business.Services.ImageServices
{
    public class ImageSizeService
    {
        private static Guid SmallImageSizeId { get; set; } = Guid.Parse("CDCCC301-F858-4E35-931A-E94A5F87F592");
        private static Guid MediumImageSizeId { get; set; } = Guid.Parse("64F43248-9E38-424D-9C0F-38463674B4DC");
        private static Guid LargeImageSizeId { get; set; } = Guid.Parse("CACF965B-49BD-4D10-8115-F45ED4BF99C1");

        private string SmallImageSizeName { get; set; } = "Small";
        private string MediumImageSizeName { get; set; } = "Medium";
        private string LargeImageSizeName { get; set; } = "Large";

        public ImageSizeType GetImageSizeById(Guid imageId)
        {
            if (imageId == SmallImageSizeId)
            {
                return ImageSizeType.Small;
            }
            else if (imageId == MediumImageSizeId)
            {
                return ImageSizeType.Medium;
            }
            else if (imageId == LargeImageSizeId)
            {
                return ImageSizeType.Large;
            }
            else
            {
                throw new Exception("Invalid image size type.");
            }
        }

        public static ImageSizeType GetImageSizeType(ImageSize imageSize)
        {
            if (imageSize.ImageSizeId == SmallImageSizeId)
            {
                return ImageSizeType.Small;
            }
            else if (imageSize.ImageSizeId == MediumImageSizeId)
            {
                return ImageSizeType.Medium;
            }
            else if (imageSize.ImageSizeId == LargeImageSizeId)
            {
                return ImageSizeType.Large;
            }
            else
            {
                throw new Exception("Invalid image size type.");
            }
        }
    }
}
