using Accio.Business.Models.SourceModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accio.Business.Services.SourceServices
{
    public class SourceService
    {
        private Guid AndroidSourceId { get; set; } = Guid.Parse("25747CB2-14C9-473C-A484-0F6295CF0B13");
        private Guid ApiSourceId { get; set; } = Guid.Parse("AA5C9818-C371-497B-94DC-1951208B1451");
        private Guid IosSourceId { get; set; } = Guid.Parse("5B794B7A-20F0-4C29-8861-62EB4DEFF74F");
        private Guid WebsiteSourceId { get; set; } = Guid.Parse("D78A2C62-0DBC-41A3-8BB2-CE4CBE9D7F75");
        private Guid UntapSourceId { get; set; } = Guid.Parse("0B1729C3-68D0-4035-ACCB-94BF2DE39E89");

        private string AndroidName { get; set; } = "Android";
        private string ApiName { get; set; } = "ApiName";
        private string IosName { get; set; } = "iOS";
        private string WebsiteName { get; set; } = "Website";
        private string UntapName { get; set; } = "Untap";

        public SourceModel GetSource(SourceType sourceType)
        {
            switch (sourceType)
            {
                case SourceType.Android:
                    return GetSourceModel(AndroidSourceId, AndroidName);
                case SourceType.API:
                    return GetSourceModel(ApiSourceId, ApiName);
                case SourceType.iOS:
                    return GetSourceModel(IosSourceId, IosName);
                case SourceType.Website:
                    return GetSourceModel(WebsiteSourceId, WebsiteName);
                case SourceType.Untap:
                    return GetSourceModel(UntapSourceId, UntapName);
                default:
                    throw new Exception("No valid source.");
            }
        }

        private SourceModel GetSourceModel(Guid id, string name)
        {
            return new SourceModel() 
            {
                SourceId = id,
                Name = name,
            };
        }
    }
}
