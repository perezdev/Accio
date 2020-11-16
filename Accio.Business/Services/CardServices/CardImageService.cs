using Accio.Business.Models.CardModels;
using Accio.Business.Models.ImageModels;
using Accio.Business.Services.ImageServices;
using Accio.Business.Services.LanguageServices;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.CardServices
{
    public class CardImageService
    {
        private AccioContext _context { get; set; }

        public CardImageService(AccioContext context)
        {
            _context = context;
        }

        public List<CardImageModel> GetCardImages(List<Guid> cardIds)
        {
            var cardImages = (from cardImage in _context.CardImages
                              join image in _context.Images on cardImage.ImageId equals image.ImageId
                              join imageSize in _context.ImageSizes on image.ImageSizeId equals imageSize.ImageSizeId
                              join lang in _context.Languages on image.LanguageId equals lang.LanguageId
                              where cardIds.Contains(cardImage.CardId) && !cardImage.Deleted && !image.Deleted
                              && !imageSize.Deleted && !lang.Deleted
                              select GetImageModel(cardImage, image, imageSize, lang)).ToList();
            return cardImages;
        }

        public static CardImageModel GetImageModel(CardImage cardImage, Image image, ImageSize imageSize, Language language)
        {
            return new CardImageModel()
            {
                CardImageId = cardImage.CardImageId,
                CardId = cardImage.CardId,
                Image = new ImageModel()
                {
                    ImageId = image.ImageId,
                    Language = LanguageService.GetLanguageModel(language),
                    Url = image.Url,
                    Size = ImageSizeService.GetImageSizeType(imageSize),
                    CreatedById = image.CreatedById,
                    CreatedDate = image.CreatedDate,
                    UpdatedById = image.UpdatedById,
                    UpdatedDate = image.UpdatedDate,
                    Deleted = image.Deleted,
                },
                CreatedById = cardImage.CreatedById,
                CreatedDate = cardImage.CreatedDate,
                UpdatedById = cardImage.UpdatedById,
                UpdatedDate = cardImage.UpdatedDate,
                Deleted = cardImage.Deleted,
            };
        }
    }
}
