using Accio.Business.Models.RulingModels;
using Accio.Data;

namespace Accio.Business.Services.RulingServices
{
    public class RulingTypeService
    {
        public static RulingTypeModel GetRulingSourceModel(RulingType rulingType)
        {
            return new RulingTypeModel()
            {
                RulingTypeId = rulingType.RulingTypeId,
                Name = rulingType.Name,
                CreatedById = rulingType.CreatedById,
                UpdatedById = rulingType.UpdatedById,
                CreatedDate = rulingType.CreatedDate,
                UpdatedDate = rulingType.UpdatedDate,
                Deleted = rulingType.Deleted,
            };
        }
    }
}
