using Accio.Business.Models.ApiModels.UntapModels;
using Accio.Business.Models.CardModels;
using Accio.Business.Models.ImageModels;
using Accio.Business.Models.SourceModels;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.SourceServices;
using Accio.Data;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.ApiServices.UntapServices
{
    public class UntapService
    {
        private AccioContext _context { get; set; }
        private CardService _cardService { get; set; }
        private SourceService _sourceService { get; set; }

        public UntapService(AccioContext context, CardService cardService, SourceService sourceService)
        {
            _context = context;
            _cardService = cardService;
            _sourceService = sourceService;
        }

        /// <summary>
        /// Returns a list of cards formatted for Untap
        /// </summary>
        public List<UntapCardModel> GetAllCards()
        {
            var source = _sourceService.GetSource(SourceType.Untap);
            var cards = _cardService.GetAllCards(source);

            return cards.Select(x => GetUntapCardModelFromCardModel(x)).ToList();
        }

        public UntapCardModel GetUntapCardModelFromCardModel(CardModel cardModel)
        {
            //Untap orientation is "right side down", which is coded as rsd. Since HP cards only rotate to the right,
            //we set null if it's vertical to force portrait. It also supports left side down, but that's not relevant to HP
            var orientation = cardModel.Orientation == "Horizontal" ? "rsd" : null;

            var largeImage = cardModel.Images.SingleOrDefault(x => x.Size == ImageSizeType.Large);
            var smallImage = cardModel.Images.SingleOrDefault(x => x.Size == ImageSizeType.Small);

            return new UntapCardModel() {
                Name = cardModel.Detail.Name,
                ImageUrl = largeImage != null ? largeImage.Url : smallImage.Url,
                Orientation = orientation,
                Rarity = cardModel.Rarity.Name,
                SetCode = cardModel.CardSet.ShortName, //Untap uses the same set codes
            };
        }
    }
}
