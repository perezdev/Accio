using System;

namespace Accio.Business.Models.CardModels
{
    public class SingleCardParameters
    {
        public string SetShortName { get; set; }
        public string CardNumber { get; set; }
        public Guid LanguageId { get; set; }
        public Guid CardId { get; set; }
        public Guid SourceId { get; set; }
        public Guid? PerformedByUserId { get; set; }
    }
}
