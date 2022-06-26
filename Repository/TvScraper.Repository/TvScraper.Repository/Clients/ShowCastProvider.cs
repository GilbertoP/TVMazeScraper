namespace TvScraper.Repository.Clients
{
    public interface IShowCastProvider
    {
        Task<HttpResponseMessage> GetTvShowCast(int showID);
    }

    /// <summary>
    /// Gets the cast per show from TvMaze
    /// </summary>
    public class ShowCastProvider : IShowCastProvider
    {
        public readonly HttpClient _client;
        private const string TV_MAZE_SHOW_CAST = "https://api.tvmaze.com/shows";
        public ShowCastProvider(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> GetTvShowCast(int showID)
        {

            var response = await _client.GetAsync($"{TV_MAZE_SHOW_CAST}/{showID}/cast");
            if (response.IsSuccessStatusCode) return new HttpResponseMessage { StatusCode = response.StatusCode, Content = response.Content };

            return new HttpResponseMessage { StatusCode = response.StatusCode , Content = response.Content};
        }
    }
}
