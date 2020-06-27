using Newtonsoft.Json;
using System.Collections.Generic;

namespace Accio.Business.Models.ImportModels
{
    public class ImportSetModel
    {
        [JsonProperty("setName")]
        public string Name { get; set; }
        [JsonProperty("releaseDate")]
        public string ReleaseDate { get; set; }
        [JsonProperty("totalCards")]
        public int TotalCards { get; set; }
        [JsonProperty("cards")]
        public List<ImportCardModel> Cards { get; set; } = new List<ImportCardModel>();
    }
}
