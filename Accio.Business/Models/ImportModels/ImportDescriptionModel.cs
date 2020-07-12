using Newtonsoft.Json;

namespace Accio.Business.Models.ImportModels
{
    public class ImportDescriptionModel
    {
        public string Text { get; set; }
        [JsonProperty("effect")]
        public string Effect { get; set; }
        [JsonProperty("toSolve")]
        public string ToSolve { get; set; }
        [JsonProperty("reward")]
        public string Reward { get; set; }
        [JsonProperty("toWin")]
        public string ToWin { get; set; }
        [JsonProperty("prize")]
        public string Prize { get; set; }
    }
}
