using Newtonsoft.Json;
using TvScraper.Repository.Clients;
using TvScraper.Repository.Mappers;
using TvScraper.Repository.Models.TvMazeResponseModels;
using TvScraper.Repository.MongoDb;

namespace TvScraper.Repository.Services
{
    public interface IShowCastCollector
    {
        Task<List<TvShowDb>> GetTvShowInformation();
    }

    /// <summary>
    /// Combines the show information to match cast from TvMaze
    /// Maps it to CDM, will be stored in MongoDb
    /// </summary>
    public class ShowCastCollector : IShowCastCollector
    {
        private readonly IShowProvider _showProvider;
        private readonly IShowCastProvider _castProvider;
        private readonly ILogger<ShowCastCollector> _logger;
        public ShowCastCollector(IShowProvider showProvider,
                                 IShowCastProvider castProvider,
                                 ILogger<ShowCastCollector> logger)
        {
            _showProvider = showProvider;
            _castProvider = castProvider;
            _logger = logger;
        }

        public async Task<List<TvShowDb>> GetTvShowInformation()
        {
            try
            {
                var result = await _showProvider.GetTvShows();

                var showList = new List<TvShowDb>();

                if (!result.IsSuccessStatusCode) return showList;

                var shows = JsonConvert.DeserializeObject<List<TvMazeShow>>(result.Content.ReadAsStringAsync().Result);

                if (shows == null ||!shows.Any()) return showList;

                foreach (var show in shows)
                {
                    var resultCastInformation = await _castProvider.GetTvShowCast(show.Id);

                    if (!resultCastInformation.IsSuccessStatusCode)
                    {
                        _logger.LogWarning($"Couldnt get the information for show : {show.Name} id: {show.Id}: Reason, statusCode {resultCastInformation.StatusCode} and message {resultCastInformation.Content.ToString()}.");
                        continue;   
                    }
                    var castInformation = JsonConvert.DeserializeObject<List<TvMazeShowCast>>(resultCastInformation.Content.ReadAsStringAsync().Result);
                    
                    if (castInformation == null ||!castInformation.Any() || string.IsNullOrEmpty(show.Name)) continue;

                    //prune the Data
                    var persons = castInformation.Select(x => x.Person);
                    KeyValuePair<int, string> showInfo = new(show.Id, show.Name);

                    var newShowInformation = ConstructShowInformation(persons, showInfo);
                    showList.Add(newShowInformation);
                }

                return showList;
            }
            catch (Exception ex)
            {
                throw new Exception($"Something went wrong in the process of getting the TvShowInformation from TvMaze errormessage : {ex.Message.ToString()}");

            }
            finally
            {
                _logger.LogInformation($"{nameof(GetTvShowInformation)} has run at {DateTime.Now}");
            }

        }

        //Overengineering?? like to keep the methods small in this example 
        private static TvShowDb ConstructShowInformation(IEnumerable<Person> cast, KeyValuePair<int, string> showInfo)
        {
            return showInfo.MapShowToCdm(cast);
        }
    }
}
