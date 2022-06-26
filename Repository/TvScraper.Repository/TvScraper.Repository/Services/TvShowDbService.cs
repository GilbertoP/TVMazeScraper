using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using TvScraper.Repository.MongoDb;

namespace TvScraper.Repository.Services
{
    public interface ITvShowDbService
    {
        Task<List<TvShowDb>> GetAsync();
        Task<TvShowDb?> GetAsync(string id);
        Task CreateAsync(TvShowDb newTvShow);
        Task CreateManyAsync(List<TvShowDb> newTvShows);
        Task UpdateAsync(string id, TvShowDb updatedTvShow);
        Task RemoveAsync(string id);
        Task<long> CountAsync();
        Task<List<TvShowDb>> GetItemsByFilter(FilterDefinition<TvShowDb> filter);
    }

    public class TvShowDbService: ITvShowDbService
    {
        private readonly IMongoCollection<TvShowDb> _mongoCollection;

        public TvShowDbService(IOptions<TvShowDbSettings> tvShowDbSettings)
        {
            var mongoClient = new MongoClient(
                tvShowDbSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                tvShowDbSettings.Value.DatabaseName);

            _mongoCollection = mongoDatabase.GetCollection<TvShowDb>(
                tvShowDbSettings.Value.CollectionName);
        }

        public async Task<List<TvShowDb>> GetAsync() =>
            await _mongoCollection.Find(_ => true).ToListAsync();

        public async Task<TvShowDb?> GetAsync(string id) =>
            await _mongoCollection.Find(x => x.ID == id).FirstOrDefaultAsync();

        public async Task<List<TvShowDb>> GetItemsByFilter(FilterDefinition<TvShowDb> filter) =>
            await _mongoCollection.Find(filter).ToListAsync();

        public async Task<long> CountAsync() =>
                    await _mongoCollection.CountAsync(new BsonDocument());

        public async Task CreateAsync(TvShowDb newTvShow) =>
            await _mongoCollection.InsertOneAsync(newTvShow);

        public async Task CreateManyAsync(List<TvShowDb> newTvShows) =>
            await _mongoCollection.InsertManyAsync(newTvShows);

        public async Task UpdateAsync(string id, TvShowDb updatedTvShow) =>
            await _mongoCollection.ReplaceOneAsync(x => x.ID == id, updatedTvShow);

        public async Task RemoveAsync(string id) =>
            await _mongoCollection.DeleteOneAsync(x => x.ID == id);
    }
}
