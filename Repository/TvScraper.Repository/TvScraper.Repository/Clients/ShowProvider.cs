namespace TvScraper.Repository.Clients
{
    public interface IShowProvider
    {
        Task<HttpResponseMessage> GetTvShows();
    }

    /// <summary>
    /// Gets all the shows known in TvMaze
    /// </summary>
    public class ShowProvider : IShowProvider
    {
        public readonly HttpClient _client;
        private const string TV_MAZE_SHOWS = "https://api.tvmaze.com/shows";
        public ShowProvider(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> GetTvShows()
        {
            var response = await _client.GetAsync(TV_MAZE_SHOWS);
            if (response.IsSuccessStatusCode) return new HttpResponseMessage { StatusCode = response.StatusCode, Content = response.Content };

            return new HttpResponseMessage { StatusCode = response.StatusCode, Content = response.Content };
        }
    }
}
