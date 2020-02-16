using HpTcgCardBrowser.Business.Models.CardModels;
using HpTcgCardBrowser.Data;

namespace HpTcgCardBrowser.Business.Services.CardServices
{
    public class CardDetailService
    {
        private HpTcgContext _context { get; set; }

        public CardDetailService(HpTcgContext context)
        {
            _context = context;
        }

        public static CardDetailModel GetCardDetailModel(CardDetail cardDetail)
        {
            return new CardDetailModel()
            {
                CardDetailId = cardDetail.CardDetailId,
                Name = cardDetail.Name,
                Text = cardDetail.Text,
                TextHtml = cardDetail.TextHtml,
                Url = cardDetail.Url,
                FlavorText = cardDetail.FlavorText,
                Illustrator = cardDetail.Illustrator,
                Copyright = cardDetail.Copyright,
                CreatedById = cardDetail.CreatedById,
                CreatedDate = cardDetail.CreatedDate,
                UpdatedById = cardDetail.UpdatedById,
                UpdatedDate = cardDetail.UpdatedDate,
                Deleted = cardDetail.Deleted,
            };
        }
    }
}
