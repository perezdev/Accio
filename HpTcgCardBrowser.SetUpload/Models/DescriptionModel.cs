using Newtonsoft.Json;

namespace HpTcgCardBrowser.SetUpload.Models
{
    public class DescriptionModel
    {
        public string Text { get; set; }
        [JsonProperty("effect")]
        public string Effect { get; set; }
        [JsonProperty("toSolve")]
        public string ToSolve { get; set; }
        [JsonProperty("reward")]
        public string Reward { get; set; }
    }
}
