using Accio.Business.Models.LessonModels;
using System;

namespace Accio.Business.Models.CardModels
{
    public class CardProvidesLessonModel
    {
        public Guid CardProvidesLessonId { get; set; }
        public Guid CardId { get; set; }
        public LessonTypeModel Lesson { get; set; } = new LessonTypeModel();
        public int ProvidesCount { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
        
    }
}
