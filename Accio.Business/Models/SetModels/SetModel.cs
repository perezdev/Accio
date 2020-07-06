using Accio.Business.Models.LanguageModels;
using System;
using System.Collections.Generic;

namespace Accio.Business.Models.SetModels
{
    public class SetModel
    {
        public Guid SetId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string IconFileName { get; set; }
        public int? Order { get; set; }
        public string ReleaseDate { get; set; }
        public int? TotalCards { get; set; }
        public List<LanguageModel> Languages { get; set; } = new List<LanguageModel>();
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
