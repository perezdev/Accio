using Accio.Business.Models.SetModels;
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
            SetsCache = sets;

            return SetsCache;
        }
        public SetModel GetSet(Guid setId)
        {
            if (SetsCache.Count > 0)
                return SetsCache.Single(x => x.SetId == setId);

            var set = (from s in _context.Set
                        where !s.Deleted where s.SetId == setId
                        select GetSetModel(s)).Single();

            return set;
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
