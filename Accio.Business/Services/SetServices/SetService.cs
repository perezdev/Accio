using Accio.Business.Models.SetModels;
using Accio.Business.Services.LanguageServices;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.CardServices
{
    public class SetService
    {
        private static List<SetModel> SetsCache { get; set; } = new List<SetModel>();

        private Guid AdventuresAtHogwartsSetId { get; set; } = Guid.Parse("C14CCE67-D0F8-4FC3-9BFA-1915CE4B1C86");
        private Guid BaseSetId { get; set; } = Guid.Parse("34ECEC73-F224-420B-927A-BE7CE8B28124");
        private Guid ChamberOfSecretsSetId { get; set; } = Guid.Parse("EFB9A41B-E84A-4E28-B07C-B30969BCD083");
        private Guid DiagonAlleySetId { get; set; } = Guid.Parse("5468F246-A070-4EFD-9B23-0CF5D26A5ED7");
        private Guid HeirOfSlytherinSetId { get; set; } = Guid.Parse("33B77285-FBB2-4712-BECF-65A0B26C32C2");
        private Guid QuidditchCupSetId { get; set; } = Guid.Parse("F05F91CD-939C-438E-866F-ADDEBEECC6F2");

        private string AdventuresAtHogwartsSetName { get; set; } = "Adventures at Hogwarts";
        private string BaseSetName { get; set; } = "Base";
        private string ChamberOfSecretsSetName { get; set; } = "Chamber of Secrets";
        private string DiagonAlleySetName { get; set; } = "Diagon Alley";
        private string HeirOfSlytherinName { get; set; } = "Heir of Slytherin";
        private string QuidditchCupName { get; set; } = "Quidditch Cup";

        private AccioContext _context { get; set; }

        public SetService(AccioContext context)
        {
            _context = context;
        }

        public List<SetModel> GetSets()
        {
            if (SetsCache.Count > 0)
                return SetsCache;

            var sets = (from set in _context.Set
                        where !set.Deleted
                        orderby set.Order
                        select GetSetModel(set)).ToList();

            var setLanguages = (from setLang in _context.SetLanguage
                                join lang in _context.Language on setLang.LanguageId equals lang.LanguageId
                                where !setLang.Deleted && !lang.Deleted && sets.Select(x => x.SetId).ToList().Contains(setLang.SetId)
                                select new { setLang, lang }).ToList();

            foreach (var set in sets)
            {
                var sls = setLanguages.Where(x => x.setLang.SetId == set.SetId);
                if (sls == null)
                    continue;

                foreach (var sl in sls)
                {
                    var language = LanguageService.GetLanguageModel(sl.lang);
                    language.Enabled = sl.setLang.Enabled;
                    set.Languages.Add(language);

                    //Order the languages so they appear as the enabled languages first and then ordered by name
                    set.Languages = set.Languages.OrderByDescending(x => x.Enabled).ThenBy(x => x.Code).ToList();
                }
            }

            SetsCache = sets;

            return SetsCache;
        }
        public SetModel GetSet(Guid setId)
        {
            if (SetsCache.Count > 0)
                return SetsCache.Single(x => x.SetId == setId);

            var setModel = (from set in _context.Set
                            where !set.Deleted && set.SetId == setId
                            orderby set.Order
                            select GetSetModel(set)).Single();

            return setModel;
        }

        public SetModel GetSetByShortName(string setShortName)
        {
            if (SetsCache.Count > 0)
                return SetsCache.FirstOrDefault(set => set.ShortName == setShortName);

            return (from set in _context.Set
                    where !set.Deleted && set.ShortName == setShortName
                    select GetSetModel(set)).FirstOrDefault();
        }

        public SetModel GetSet(TypeOfSet type)
        {
            switch (type)
            {
                case TypeOfSet.AdventuresAtHogwarts:
                    return GetSetModel(AdventuresAtHogwartsSetId, AdventuresAtHogwartsSetName);
                case TypeOfSet.Base:
                    return GetSetModel(BaseSetId, BaseSetName);
                case TypeOfSet.ChamberOfSecrets:
                    return GetSetModel(ChamberOfSecretsSetId, ChamberOfSecretsSetName);
                case TypeOfSet.DiagonAlley:
                    return GetSetModel(DiagonAlleySetId, DiagonAlleySetName);
                case TypeOfSet.HeirOfSlytherin:
                    return GetSetModel(HeirOfSlytherinSetId, HeirOfSlytherinName);
                case TypeOfSet.QuidditchCup:
                    return GetSetModel(QuidditchCupSetId, QuidditchCupName);
                default:
                    throw new Exception("Could not identify set type.");
            }
        }

        public static SetModel GetSetModel(Set set)
        {
            return new SetModel()
            {
                SetId = set.SetId,
                Name = set.Name,
                ShortName = set.ShortName,
                Description = set.Description,
                IconFileName = set.IconFileName,
                Order = set.Order,
                ReleaseDate = set.ReleaseDate,
                TotalCards = set.TotalCards,
                CreatedById = set.CreatedById,
                CreatedDate = set.CreatedDate,
                UpdatedById = set.UpdatedById,
                UpdatedDate = set.UpdatedDate,
                Deleted = set.Deleted,
            };
        }
        public static SetModel GetSetModel(Guid setId, string name)
        {
            return new SetModel()
            {
                SetId = setId,
                Name = name,
                Deleted = false,
            };
        }
    }
}
