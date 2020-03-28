using HpTcgCardBrowser.Business.Models.LessonModels;
using HpTcgCardBrowser.Data;
using System.Collections.Generic;
using System.Linq;

namespace HpTcgCardBrowser.Business.Services.LessonServices
{
    public class LessonService
    {
        private HpTcgContext _context { get; set; }

        public LessonService(HpTcgContext context)
        {
            _context = context;
        }

        public List<LessonTypeModel> GetLessonTypes()
        {
            var lessonTypeModels = (from lessonType in _context.LessonType
                                    where !lessonType.Deleted
                                    select GetLessonTypeModel(lessonType)).ToList();

            return lessonTypeModels;
        }

        public static LessonTypeModel GetLessonTypeModel(LessonType lessonType)
        {
            return new LessonTypeModel()
            {
                LessonTypeId = lessonType.LessonTypeId,
                Name = lessonType.Name,
                ImageName = lessonType.ImageName,
                CreatedById = lessonType.CreatedById,
                CreatedDate = lessonType.CreatedDate,
                UpdatedById = lessonType.UpdatedById,
                UpdatedDate = lessonType.UpdatedDate,
                Deleted = lessonType.Deleted,
            };
        }
    }
}
