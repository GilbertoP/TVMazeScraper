using Newtonsoft.Json;

namespace TvScraper.Repository.Models.TvMazeResponseModels
{
    public class TvMazeActors
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("country")]
        public Country Country { get; set; }

        [JsonProperty("birthday")]
        public string Birthday { get; set; }

        [JsonProperty("deathday")]
        public string Deathday { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("updated")]
        public int Updated { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
}
