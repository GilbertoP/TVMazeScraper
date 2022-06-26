using Newtonsoft.Json;
using TvScraper.Repository.MongoDb;

namespace TvScraper.Repository.Models.DTO
{
    public class ShowsDTO
    {
        [JsonProperty("totalShows")]
        public long TotalShows { get; set; }
        [JsonProperty("totalPages")]
        public long TotalPages { get; set; }
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("tvShows")]
        public List<TvShowDb> TvShows { get; set; }

    }
}
