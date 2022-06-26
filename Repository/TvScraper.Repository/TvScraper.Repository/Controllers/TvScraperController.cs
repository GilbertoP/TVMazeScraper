using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TvScraper.Repository.Models.DTO;
using TvScraper.Repository.MongoDb;
using TvScraper.Repository.Services;

namespace TvScraper.Repository.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class TvScraperController : ControllerBase
    {
        private readonly ILogger<TvScraperController> _logger;
        private readonly ITvShowService _tvShowService;
        private readonly IShowCastCollector _showCastCollector;
        public TvScraperController(ILogger<TvScraperController> logger,
                                   IShowCastCollector showCastCollector,
                                   ITvShowService tvShowService)
        {
            _logger = logger;
            _showCastCollector = showCastCollector;
            _tvShowService = tvShowService; 
        }

        /// <summary>
        /// Gets all TvShows entries from MongoDB, and provides consuming Client with show information 
        /// This is the moneyshot!
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ShowsDTO), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("page/{page:int?}", Name = "GetTvShowsFromDb")]
        public async Task<IActionResult> GetTvShowsFromDb(int page = 1)
        {
            _logger.LogInformation("Starting to read local Db for shows");
            var result = await _tvShowService.GetShowsFromDb(page);
            if (result == null) return NotFound();
            return Ok(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// Get method used to start scraping Tv Maze, and fill the db
        /// A scheduled function, timerTrigger set to one week, is a better sollution to keep Db up to date
        /// Assignment is to build a REST api
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(List<TvShowDb>), 200)]
        [HttpGet(Name = "UpdateTvShowDb")]
        public async Task UpdateTvShowDb()
        {
            _logger.LogInformation("Starting to scrape TV Maze for shows");
            await _tvShowService.PopulateDb();
        }

        /// <summary>
        /// Get method used to test
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(List<TvShowDb>), 200)]
        [HttpGet(Name = "GetConcenatedShows")]
        public async Task<IActionResult> GetConcenatedShows()
        {
            _logger.LogInformation("Starting to scrape TV Maze for shows");
            var result = await _showCastCollector.GetTvShowInformation();
            return Ok(JsonConvert.SerializeObject(result));
        }
    }
}
