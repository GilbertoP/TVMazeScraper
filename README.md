# TVMazeScraper
## Assesment for RTL


> Code fullfills the next requeriments
> * Scrapes the [TvMaze API](http://www.tvmaze.com/api) for shows and actors of those shows.
> * Joins the shows with corresponding actors in a new model.
> * Populates a MongoDb database on local machine.
> * Returns scraped transformed data, from the database,  as Json response to any consuming client.
>   * Data can be retrieved with a Get-operation :  **/api/v1/TvScraper/GetTvShowsFromDb/page/{page}** 
>   * This response is paginated and contains the actors list per show sorted by descendening order per birthday
>   * Contains two extra endpoints:
>     * One to scrape TvMaze and populate the database, Get-operation **/api/v1/TvScraper/UpdateTvShowDb**
>     * Second endpoint used for testing and retrieves data directly from TvMaze, Get-operation **/api/v1/TvScraper/GetConcenatedShows**

For more information check the code or consult the samples in the repository, or contact me

