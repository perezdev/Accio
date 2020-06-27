using System;

namespace Accio.Business.Models.LessonModels
{
    public class LessonTypeModel
    {
        public Guid LessonTypeId { get; set; }
        public string Name { get; set; }
        public string ImageName { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
