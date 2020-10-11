using Accio.Business.Services.ImportServices;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Accio.Business.Models.ImportModels
{
    public class ImportCardModel
    {
        [JsonProperty("number")]
        public string Number { get; set; } //Numbers can be alphanumeric
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("lesson")]
        public string LessonType { get; set; }
        [JsonProperty("subTypes")]
        public string[] SubTypes { get; set; }
        [JsonProperty("dmgEachTurn")]
        public int? Damage { get; set; }
        [JsonProperty("health")]
        public int? Health { get; set; }
        [JsonProperty("description")]
        [JsonConverter(typeof(DescriptionConverter))]
        public ImportDescriptionModel Description { get; set; } = new ImportDescriptionModel();
        [JsonProperty("flavorText")]
        public string FlavorText { get; set; }
        [JsonProperty("rarity")]
        public string Rarity { get; set; }
        [JsonProperty("artist")]
        [JsonConverter(typeof(ArtistConverter))]
        public List<string> Artists { get; set; }
        [JsonProperty("cost")]
        public string Cost { get; set; }
        [JsonProperty("provides")]
        public string[] Provides { get; set; }
    }
}
