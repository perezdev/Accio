using Accio.Business.Models.RulingModels;
using Accio.Data;

namespace Accio.Business.Services.RulingServices
{
    public class RulingSourceService
    {
        public static RulingSourceModel GetRulingSourceModel(RulingSource rulingSource)
        {
            return new RulingSourceModel() 
            {
                RulingSourceId = rulingSource.RulingSourceId,
                Name = rulingSource.Name,
                CreatedById = rulingSource.CreatedById,
                UpdatedById = rulingSource.UpdatedById,
                CreatedDate = rulingSource.CreatedDate,
                UpdatedDate = rulingSource.UpdatedDate,
                Deleted = rulingSource.Deleted,
            };
        }
    }
}
