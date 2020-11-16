using Accio.Business.Models.LessonModels;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accio.Business.Services.LessonServices
{
    public class LessonService
    {
        private Guid ComcLessonId { get; set; } = Guid.Parse("9E9216EF-4ADA-4CCA-B83B-2390965089CC");
        private Guid CharmsLessonId { get; set; } = Guid.Parse("C50F3FB9-0363-4802-AE31-0D4ADA96C4B2");
        private Guid PotionsLessonId { get; set; } = Guid.Parse("B36C558C-110D-4EB4-94C4-2B9FDBAC8539");
        private Guid QuidditchLessonId { get; set; } = Guid.Parse("4576FE6B-0337-4219-B622-5D172905866F");
        private Guid TransfigurationLessonId { get; set; } = Guid.Parse("6D4611CF-8044-4ABF-83F2-B334B557DA20");
        
        private string ComcLessonName { get; set; } = "Care of Magical Creatures";
        private string CharmsLessonName { get; set; } = "Charms";
        private string PotionsLessonName { get; set; } = "Potions";
        private string QuidditchLessonName { get; set; } = "Quidditch";
        private string TransfigurationLessonName { get; set; } = "Transfiguration";

        private string ComcLessonCssClassName { get; set; } = "lesson-color-comc";
        private string CharmsLessonCssClassName { get; set; } = "lesson-color-charms";
        private string PotionsLessonCssClassName { get; set; } = "lesson-color-potions";
        private string QuidditchLessonCssClassName { get; set; } = "lesson-color-quidditch";
        private string TransfigurationLessonCssClassName { get; set; } = "lesson-color-transfig";

        private AccioContext _context { get; set; }

        public LessonService(AccioContext context)
        {
            _context = context;
        }

        public List<LessonTypeModel> GetLessonTypes()
        {
            var lessonTypeModels = (from lessonType in _context.LessonTypes
                                    where !lessonType.Deleted
                                    select GetLessonTypeModel(lessonType)).ToList();

            return lessonTypeModels;
        }

        public LessonTypeModel GetLessonType(TypeOfLesson type)
        {
            switch (type)
            {
                case TypeOfLesson.CareOfMagicalCreatures:
                    return GetLessonTypeModel(ComcLessonId, ComcLessonName, ComcLessonCssClassName);
                case TypeOfLesson.Charms:
                    return GetLessonTypeModel(CharmsLessonId, CharmsLessonName, CharmsLessonCssClassName);
                case TypeOfLesson.Potions:
                    return GetLessonTypeModel(PotionsLessonId, PotionsLessonName, PotionsLessonCssClassName);
                case TypeOfLesson.Quidditch:
                    return GetLessonTypeModel(QuidditchLessonId, QuidditchLessonName, QuidditchLessonCssClassName);
                case TypeOfLesson.Transfiguration:
                    return GetLessonTypeModel(TransfigurationLessonId, TransfigurationLessonName, TransfigurationLessonCssClassName);
                default:
                    throw new Exception("Lesson type not recognized.");
            }
        }

        public static LessonTypeModel GetLessonTypeModel(LessonType lessonType)
        {
            return new LessonTypeModel()
            {
                LessonTypeId = lessonType.LessonTypeId,
                Name = lessonType.Name,
                ImageName = lessonType.ImageName,
                CssClassName = lessonType.CssClassName,
                CreatedById = lessonType.CreatedById,
                CreatedDate = lessonType.CreatedDate,
                UpdatedById = lessonType.UpdatedById,
                UpdatedDate = lessonType.UpdatedDate,
                Deleted = lessonType.Deleted,
            };
        }
        public static LessonTypeModel GetLessonTypeModel(Guid lessonTypeId, string name, string cssClassName)
        {
            return new LessonTypeModel()
            {
                LessonTypeId = lessonTypeId,
                Name = name,
                CssClassName = cssClassName,
                Deleted = false,
            };
        }
    }
}
