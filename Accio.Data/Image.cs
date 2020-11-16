using System;
using System.Collections.Generic;

#nullable disable

namespace Accio.Data
{
    public partial class Image
    {
        public Guid ImageId { get; set; }
        public Guid LanguageId { get; set; }
        public string Url { get; set; }
        public Guid ImageSizeId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
