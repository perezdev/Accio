using System;
using System.Collections.Generic;

namespace Accio.Data
{
    public partial class CardProvidesLesson
    {
        public Guid CardProvidesLessonId { get; set; }
        public Guid CardId { get; set; }
        public Guid LessonId { get; set; }
        public int Provides { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
