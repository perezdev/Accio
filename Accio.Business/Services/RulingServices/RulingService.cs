using Accio.Business.Models.RulingModels;
using Accio.Business.Services.CardServices;
using Accio.Data;

namespace Accio.Business.Services.RulingServices
{
    public class RulingService
    {
        public static RulingModel GetRulingModel(Ruling ruling, RulingSource rulingSource, RulingType rulingType, CardType cardType)
        {
            return new RulingModel() 
            {
                RulingId = ruling.RulingId,
                RulingType = RulingTypeService.GetRulingSourceModel(rulingType),
                RulingSource = RulingSourceService.GetRulingSourceModel(rulingSource),
                CardType = cardType == null ? null : TypeService.GetCardTypeModel(cardType),
                Question = ruling.Question,
                Answer = ruling.Answer,
                Ruling = ruling.Ruling1,
                GeneralInfo = ruling.GeneralInfo,
                RulingDate = ruling.RulingDate,
                LanguageId = ruling.LanguageId,
                CreatedById = ruling.CreatedById,
                CreatedDate = ruling.CreatedDate,
                UpdatedById = ruling.UpdatedById,
                UpdatedDate = ruling.UpdatedDate
            };
        }
    }
}
