using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace TvScraper.Repository.MongoDb
{
    public class TvShowDb
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        public string Name { get; set; } = null!;

        public List<Actor> Cast { get; set; } = null!;
    }

    public class Actor
    {

        public int ID { get; set; }
        public string Name { get; set; } = null!;

        public DateTime? Birthday { get; set; } = null!;
    }
}
