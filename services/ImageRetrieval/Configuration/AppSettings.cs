using Infrastructure.Configuration;

namespace ImageRetrieval.Configuration
{
    public class AppSettings
    {
        public RabbitSettings RabbitSettings { get; set; }
        public MongoDBConfig MongoDbConfig { get; set; }
    }
}