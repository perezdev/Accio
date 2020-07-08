using Accio.Business.Models.CardModels;
using Accio.Business.Models.LanguageModels;
using Accio.Data;

namespace Accio.Business.Services.CardServices
{
    public class CardDetailService
    {
        private AccioContext _context { get; set; }

        public CardDetailService(AccioContext context)
        {
            _context = context;
        }

        public static CardDetailModel GetCardDetailModel(CardDetail cardDetail, Language language)
        {
            return new CardDetailModel()
            {
                CardDetailId = cardDetail.CardDetailId,
                CardId = cardDetail.CardId,
                Language = new LanguageModel()
                {
                    LanguageId = language.LanguageId,
                    Name = language.Name,
                    Code = language.Code,
                    FlagImagePath = language.FlagImagePath,
                    CreatedById = language.CreatedById,
                    CreatedDate = language.CreatedDate,
                    UpdatedById = language.UpdatedById,
                    UpdatedDate = language.UpdatedDate,
                    Deleted = language.Deleted,
                },
                Name = cardDetail.Name,
                Text = cardDetail.Text,
                Effect = cardDetail.Effect,
                Reward = cardDetail.Reward,
                ToSolve = cardDetail.ToSolve,
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
