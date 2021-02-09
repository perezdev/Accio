using Accio.Business.Models.RulingRestrictionModels;
using Accio.Data;
using System;

namespace Accio.Business.Services.RulingRestrictionServices
{
    public class RulingRestrictionService
    {
        private static Guid BannedRulingRestrictionId { get; set; } = Guid.Parse("07F98A53-3867-45DC-B7F9-A99F332ACB38");
        private static Guid LegalRulingRestrictionId { get; set; } = Guid.Parse("3A73CD59-F46D-4700-A45A-DF818C607FE2");
        private static Guid RestrictionRulingRestrictionId { get; set; } = Guid.Parse("32D64370-1BE8-4E7B-A2B5-A8C8BBBA7C4E");
        private static Guid NotLegalRulingRestrictionId { get; set; } = Guid.Parse("F6E1928B-C51C-4E46-91CE-4C3A64AC1113");

        public static RulingRestrictionModel GetRulingRestrictionModel(RulingRestriction rulingRestriction)
        {
            var restrictionType = GetRulingRestrictionTypeById(rulingRestriction.RulingRestrictionId);
            return new RulingRestrictionModel() 
            {
                RulingRestrictionId = rulingRestriction.RulingRestrictionId,
                Name = rulingRestriction.Name,
                ShortName = rulingRestriction.ShortName,
                Type = restrictionType,
                CreatedById = rulingRestriction.CreatedById,
                CreatedDate = rulingRestriction.CreatedDate,
                UpdatedById = rulingRestriction.UpdatedById,
                UpdatedDate = rulingRestriction.UpdatedDate,
                Deleted = rulingRestriction.Deleted,
            };
        }
        public static RulingRestrictionType GetRulingRestrictionTypeById(Guid rulingRestrictionId)
        {
            if (rulingRestrictionId == BannedRulingRestrictionId)
            {
                return RulingRestrictionType.Banned;
            }
            else if (rulingRestrictionId == LegalRulingRestrictionId)
            {
                return RulingRestrictionType.Legal;
            }
            else if (rulingRestrictionId == RestrictionRulingRestrictionId)
            {
                return RulingRestrictionType.Restricted;
            }
            else if (rulingRestrictionId == NotLegalRulingRestrictionId)
            {
                return RulingRestrictionType.NotLegal;
            }
            else
            {
                throw new Exception($"Ruling restriction ID {rulingRestrictionId} is not valid.");
            }
        }
    }
}
