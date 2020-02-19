using HpTcgCardBrowser.SetUpload.CustomJsonConverters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HpTcgCardBrowser.SetUpload.Models
{
    public class CardModel
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
        [JsonProperty("description")]
        [JsonConverter(typeof(DescriptionConverter))]
        public DescriptionModel Description { get; set; } = new DescriptionModel();
        [JsonProperty("flavorText")]
        public string FlavorText { get; set; }
        [JsonProperty("rarity")]
        public string Rarity { get; set; }
        [JsonProperty("artist")]
        [JsonConverter(typeof(ArtistConverter))]
        public List<string> Artists { get; set; }
        [JsonProperty("cost")]
        public string Cost { get; set; }

    }
}
