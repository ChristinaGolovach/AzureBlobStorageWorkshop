using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobStorageWorkshop.Services.Interfaces
{
    public interface IStorageService
    {
        Task<IEnumerable<string>> GetBlobNameListAsync();
        Task<Stream> DownloadDataAsync(string id);
        Task UploadDataAsync(Stream dataSource, string fileName);        
    }
}
