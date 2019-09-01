using Infrastructure.Configuration;
using MongoDB.Driver;
using Serilog;

namespace Infrastructure.Persistence
{
    public class ImageContext : IImageContext
    {
        private readonly IMongoDatabase _db;

        public ImageContext(MongoDBConfig config, ILogger logger)
        {
            var client = new MongoClient(config.ConnectionString);
            logger.Information($"Connecting to {config.ConnectionString}");
            _db = client.GetDatabase(config.Database);
        }

        public IMongoCollection<ImageDetails> Images => _db.GetCollection<ImageDetails>("Images");
    }
}