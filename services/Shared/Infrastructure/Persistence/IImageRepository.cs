using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public interface IImageRepository
    {
        Task<IEnumerable<ImageDetails>> GetAllImages();

        Task<ImageDetails> GetImage(string id);
        Task Create(ImageDetails img);
        Task<bool> Update(ImageDetails img);
        Task<bool> Delete(string id);
    }
}