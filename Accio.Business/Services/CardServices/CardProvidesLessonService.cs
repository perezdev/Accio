using Accio.Business.Models.CardModels;
using Accio.Business.Services.LessonServices;
using Accio.Data;
using System;

namespace Accio.Business.Services.CardServices
{
    public class CardProvidesLessonService
    {
        public AccioContext _context { get; set; }

        public CardProvidesLessonService(AccioContext context)
        {
            _context = context;
        }

        public static CardProvidesLessonModel GetCardProvidesLessonModel(CardProvidesLesson cardProvidesLesson, LessonType lessonType)
        {
            return new CardProvidesLessonModel() 
            {
                CardProvidesLessonId = cardProvidesLesson.CardProvidesLessonId,
                CardId = cardProvidesLesson.CardId,
                Lesson = LessonService.GetLessonTypeModel(lessonType),
                ProvidesCount = cardProvidesLesson.Provides,
                CreatedById = cardProvidesLesson.CreatedById,
                CreatedDate = cardProvidesLesson.CreatedDate,
                UpdatedById = cardProvidesLesson.UpdatedById,
                UpdatedDate = cardProvidesLesson.UpdatedDate,
                Deleted = cardProvidesLesson.Deleted,
            };
        }

        public void PersistCardProvidesLesson(Guid cardId, Guid lessonId, int provides)
        {
            var now = DateTime.UtcNow;
            var cardProvidesLesson = new CardProvidesLesson()
            {
                CardProvidesLessonId = Guid.NewGuid(),
                CardId = cardId,
                Provides = provides,
                LessonId = lessonId,
                CreatedById = Guid.Empty,
                CreatedDate = now,
                UpdatedById = Guid.Empty,
                UpdatedDate = now,
                Deleted = false,
            };
            _context.CardProvidesLesson.Add(cardProvidesLesson);
            _context.SaveChanges();
        }
    }
}
