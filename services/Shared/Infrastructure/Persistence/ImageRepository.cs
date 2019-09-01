using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Persistence
{
    public class ImageRepository : IImageRepository
    {
        private readonly IImageContext _context;

        public ImageRepository(IImageContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ImageDetails>> GetAllImages()
        {
            return await _context
                .Images
                .Find(_ => true)
                .ToListAsync();
        }

        public Task<ImageDetails> GetImage(string id)
        {
            FilterDefinition<ImageDetails> filter =
                Builders<ImageDetails>.Filter.Eq(m => m.Id, id);

            return _context
                .Images
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task Create(ImageDetails Image)
        {
            await _context.Images.InsertOneAsync(Image);
        }

        public async Task<bool> Update(ImageDetails Image)
        {
            ReplaceOneResult updateResult =
                await _context
                    .Images
                    .ReplaceOneAsync(
                        filter: g => g.Id == Image.Id,
                        replacement: Image);

            return updateResult.IsAcknowledged
                   && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> Delete(string id)
        {
            FilterDefinition<ImageDetails> filter = Builders<ImageDetails>.Filter.Eq(m => m.Id, id);
            DeleteResult deleteResult = await _context.Images.DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }
    }
}