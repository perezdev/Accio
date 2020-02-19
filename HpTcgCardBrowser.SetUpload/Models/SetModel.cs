using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HpTcgCardBrowser.SetUpload.Models
{
    public class SetModel
    {
        [JsonProperty("setName")]
        public string SetName { get; set; }
        [JsonProperty("releaseDate")]
        public string ReleaseDate { get; set; }
        [JsonProperty("totalCards")]
        public int TotalCards { get; set; }
        [JsonProperty("cards")]
        public List<CardModel> Cards { get; set; } = new List<CardModel>();
    }
}
