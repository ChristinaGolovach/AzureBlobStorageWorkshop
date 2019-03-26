using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureBlobStorageWorkshop.Services.Interfaces
{
    public interface IImageService
    {
        Task<IEnumerable<string>> GetAllImagesNameAsync();
        Task<Stream> GetImageAsync(string imageName);
        Task AddImageAsync(Stream file, string imageName);
    }
}
