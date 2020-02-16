using HpTcgCardBrowser.Business.Models.LanguageModels;
using HpTcgCardBrowser.Data;
using System.Collections.Generic;
using System.Linq;

namespace HpTcgCardBrowser.Business.Services.LanguageServices
{
    public class LanguageService
    {
        private HpTcgContext _context { get; set; }

        public LanguageService(HpTcgContext context)
        {
            _context = context;
        }

        public List<LanguageModel> GetLanguages()
        {
            var languageModels = (from language in _context.Language
                                  where !language.Deleted
                                  select GetLanguageModel(language)).ToList();

            return languageModels;
        }

        public static LanguageModel GetLanguageModel(Language language)
        {
            return new LanguageModel()
            {
                LanguageId = language.LanguageId,
                Name = language.Name,
                CreatedById = language.CreatedById,
                CreatedDate = language.CreatedDate,
                UpdatedById = language.UpdatedById,
                UpdatedDate = language.UpdatedDate,
                Deleted = language.Deleted,
            };
        }
    }
}
