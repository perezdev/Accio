using Accio.Business.Models.FormatModels;
using System;

namespace Accio.Business.Services.FormatServices
{
    public class FormatService
    {
        private Guid ClassicFormatId { get; set; } = Guid.Parse("ABB82666-73CF-45F7-B333-FFC2114C823D");
        private Guid RevivalFormatId { get; set; } = Guid.Parse("28D5D2CF-BCC7-4AAC-8732-DC727959DC62");

        public FormatType GetFormatTypeById(Guid formatId)
        {
            if (formatId == ClassicFormatId)
            {
                return FormatType.Classic;
            }
            else if (formatId == RevivalFormatId)
            {
                return FormatType.Revival;
            }
            else
            {
                throw new Exception($"{formatId.ToString().ToUpper()} is not a valid format ID.");
            }
        }
    }
}
