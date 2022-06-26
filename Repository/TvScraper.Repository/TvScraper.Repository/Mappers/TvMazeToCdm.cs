using TvScraper.Repository.Models.TvMazeResponseModels;
using TvScraper.Repository.MongoDb;

namespace TvScraper.Repository.Mappers
{
    public static class TvMazeToCdm
    {
        public static TvShowDb MapShowToCdm(this KeyValuePair<int, string> show, IEnumerable<Person> cast)
        {
            return new TvShowDb()
            {
                ID = show.Key.ToString(),
                Name = show.Value,
                Cast = cast.MapCastToCDM()
            };
        }

        private static List<MongoDb.Actor> MapCastToCDM(this IEnumerable<Person> cast)
        {
            List<MongoDb.Actor> Actors = new List<MongoDb.Actor>();
            foreach(var actor in cast)
            {
                MongoDb.Actor cdmActor = new MongoDb.Actor()
                {
                    ID = actor.Id,
                    Name = actor.Name ?? "",
                    Birthday = actor.Birthday != null ? DateTime.Parse(actor.Birthday) : null,
                };
                Actors.Add(cdmActor);
            }
            return Actors;
        }
    }
}
