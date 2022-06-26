using MongoDB.Bson;
using MongoDB.Driver;
using System.Text;
using TvScraper.Repository.Models.DTO;
using TvScraper.Repository.MongoDb;

namespace TvScraper.Repository.Services
{
    public interface ITvShowService
    {
        Task PopulateDb();
        Task<TvShowDb?> GetShowFromDb(int showId);
        Task<ShowsDTO> GetShowsFromDb(int page = 1);
    }
    /// <summary>
    /// Service involving the population of the Db and reading the Db, this is the moneyshot
    /// </summary>
    public class TvShowService: ITvShowService
    {
        private const long ITEMS_ON_PAGE = 10;
        private readonly ITvShowDbService _tvShowDbService;
        private readonly IShowCastCollector _showCastCollector;
        private readonly ILogger<TvShowService> _logger;
        public TvShowService(ITvShowDbService tvShowDbService,
                             IShowCastCollector showCastCollector,
                             ILogger<TvShowService> logger)
        {
            _tvShowDbService = tvShowDbService;
            _showCastCollector = showCastCollector;
            _logger = logger;   
        }

        public async Task PopulateDb()
        {
            try
            {
                _logger.LogInformation("Starting with update of Db");
                // get shows complete with casts, expensive call => refactor if time, best solution compare db with TvMaze for new updates
                var shows = await _showCastCollector.GetTvShowInformation();

                if (!shows.Any() || shows == null) return;

                // check if tv show exists in MongoDb
                var dbShows = await _tvShowDbService.GetAsync();

                if (dbShows?.Count > 0)
                {
                    //prune results, only should contain new records
                    var newShows = shows.Where(x => !dbShows.Any(y => y.ID == x.ID)).ToList();

                    if(newShows.Count > 1)
                    {
                        await _tvShowDbService.CreateManyAsync(newShows);
                    }
                    else if(newShows.Count == 1)
                    {
                        await _tvShowDbService.CreateAsync(newShows.FirstOrDefault());
                    }
                }

            }
            catch(Exception ex)
            {
                throw new Exception($"Something went wrong in the process of populating the db with errormessage : {ex.Message.ToString()}");
            }
            finally
            {
                _logger.LogInformation($"Finished {nameof(PopulateDb)} at {DateTime.Now}");
            }
            
        }

        public async Task<TvShowDb?> GetShowFromDb(int showId)
        {
            var tvShow = await _tvShowDbService.GetAsync(showId.ToString());
            if (tvShow == null) {
                _logger.LogInformation("Not found any Tv_Shows in the database");
                return null;
            } 
            return tvShow;
        }

        public async Task<ShowsDTO> GetShowsFromDb(int page)
        {
            try
            {
                // In this scenario _id in the document equals the number of shows incrementally in the collection.
                // In real life scenarios this could be a dangerous and often a false assumption
                var totalShowsinDb = await _tvShowDbService.CountAsync();
                var totalPages = (double)totalShowsinDb / ITEMS_ON_PAGE;

                //Functional decision, null as response to the  api will be handled as not found
                if (1 > page || page > totalPages) return null;

                var query = ConstructMongoQuery(page, totalPages, totalShowsinDb);

                if (string.IsNullOrEmpty(query)) return null;

                var showSelection = await _tvShowDbService.GetItemsByFilter(query);

                if (!showSelection.Any())
                {
                    _logger.LogInformation("Not found any Tv_Shows in the database");
                    return null;
                }

                //todo fix ordering
                var orderdedShow = new List<TvShowDb>();
                foreach(var show in showSelection)
                {
                    show.Cast = show.Cast.OrderByDescending(x => x.Birthday).ToList();
                    orderdedShow.Add(show);
                }

                return new ShowsDTO()
                {
                    TotalShows = totalShowsinDb,
                    TotalPages = totalShowsinDb / ITEMS_ON_PAGE,
                    CurrentPage = page,
                    TvShows = orderdedShow
                };
            }
            catch(Exception ex)
            {
                throw new Exception($"Something went wrong in the process of getting a selection of tv shows from the db with errormessage : {ex.Message.ToString()}");
            }
            finally
            {
                _logger.LogInformation($"Finished {nameof(GetShowsFromDb)} at {DateTime.Now}");
            }

        }

        /// <summary>
        /// not my brightest helper
        /// </summary>
        /// <param name="pageNumber">number of pages to be served in the api response</param>
        /// <param name="totalPages">total amount of pages to be displayed in the pagination</param>
        /// <param name="totalShows">fucntions as max value</param>
        /// <returns></returns>
        private static string ConstructMongoQuery(int pageNumber, double totalPages, long totalShows)
        {
            List<int> ids;
            string allContents;
            string query;
            int firstDocument;
            // set id's to list according to pagenumber and totalpages
            if (pageNumber > 0 && pageNumber < (totalShows/ITEMS_ON_PAGE))
            {
                int lastDocument = (pageNumber * 10); 
                firstDocument = (lastDocument - (int)ITEMS_ON_PAGE) +1;        //how arrays work
                ids = Enumerable.Range(firstDocument , (int)ITEMS_ON_PAGE).ToList();
                allContents = string.Join("', '", ids);
                query = "{ _id " + ": { $in : ['" + allContents + "'] }}";
                return query;
            }else if(pageNumber == (totalShows / ITEMS_ON_PAGE))
            {
                firstDocument = (pageNumber * 10);
                ids = Enumerable.Range(pageNumber*10+1, ((int)totalShows - firstDocument)).ToList();
                allContents = string.Join("', '", ids);
                query = "{ _id " + ": { $in : ['" + allContents + "'] }}";
                return query;
            }
            return "";
        }
    }
}
