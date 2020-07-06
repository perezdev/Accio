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
        private HpTcgContext _context { get; set; }
        private static List<SetModel> SetsCache { get; set; } = new List<SetModel>();

        public SetService(HpTcgContext context)
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
    }
}
