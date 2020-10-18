using Accio.Business.Models.LanguageModels;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.LanguageServices
{
    public class LanguageService
    {
        private AccioContext _context { get; set; }

        public LanguageService(AccioContext context)
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
        public Guid? GetLanguageIdByCode(string languageCode)
        {
            var language = (from lang in _context.Language
                            where !lang.Deleted && lang.Code == languageCode
                            select lang).SingleOrDefault();
            return language?.LanguageId;
        }
        public Guid GetLanguageId(TypeOfLanguage typeOfLanguage)
        {
            var englishLanguageId = Guid.Parse("4F5CC98D-4315-4410-809F-E2CC428E0C9B");

            switch (typeOfLanguage)
            {
                case TypeOfLanguage.English:
                    return englishLanguageId;
                default:
                    throw new Exception("Language type is not valid.");
            }
        }

        public static LanguageModel GetLanguageModel(Language language)
        {
            return new LanguageModel()
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
            };
        }
    }
}
