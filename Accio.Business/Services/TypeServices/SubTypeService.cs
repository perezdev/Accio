using Accio.Business.Models.TypeModels;
using Accio.Data;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.TypeServices
{
    public class SubTypeService
    {
        public AccioContext _context { get; set; }

        public SubTypeService(AccioContext context)
        {
            _context = context;
        }

        public List<SubTypeModel> GetAllSubTypes()
        {
            var subTypes = (from subType in _context.SubType
                            where !subType.Deleted
                            select GetSubTypeModel(subType)).ToList();
            return subTypes;
        }

        public static SubTypeModel GetSubTypeModel(SubType subType)
        {
            return new SubTypeModel()
            {
                SubTypeId = subType.SubTypeId,
                Name = subType.Name,
                CreatedById = subType.CreatedById,
                CreatedDate = subType.CreatedDate,
                UpdatedById = subType.UpdatedById,
                UpdatedDate = subType.UpdatedDate,
                Deleted = subType.Deleted,
            };
        }
    }
}
