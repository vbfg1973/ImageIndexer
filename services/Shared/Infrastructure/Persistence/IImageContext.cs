using MongoDB.Driver;

namespace Infrastructure.Persistence
{
    public interface IImageContext
    {
        IMongoCollection<ImageDetails> Images { get; }
    }
}