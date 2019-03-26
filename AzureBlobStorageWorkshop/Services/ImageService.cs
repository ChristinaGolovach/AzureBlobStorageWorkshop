using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AzureBlobStorageWorkshop.Services.Interfaces;
using System.IO;

namespace AzureBlobStorageWorkshop.Services
{
    public class ImageService : IImageService
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IQueueStorageService _queueStorageService;
        private readonly ILogger _logger;

        public ImageService(IBlobStorageService blobStorageService, IQueueStorageService queueStorageService, ILoggerFactory loggerFactory)
        {
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException($"The {nameof(blobStorageService)} van not be null.");
            _queueStorageService = queueStorageService ?? throw new ArgumentNullException($"The {nameof(queueStorageService)} van not be null.");

            _logger = loggerFactory.CreateLogger<ImageService>();
        }

        public async Task<IEnumerable<string>> GetAllImagesNameAsync()
        {
            _logger.LogInformation($"Loading all images name from storage.");

            var result =  await _blobStorageService.GetBlobNameListAsync();

            return result;
        }

        public async Task<Stream> GetImageAsync(string imageName)
        {
            _logger.LogInformation($"Loading image {nameof(imageName)} from storage.");

            var dataStream = await _blobStorageService.DownloadDataAsync(imageName).ConfigureAwait(false);

            if (dataStream == null)
            {
                _logger.LogError($"Image {nameof(imageName)} not found.");
            }  

           _logger.LogInformation($"Image {nameof(imageName)} is loaded from storage.");

            return dataStream;
        }

        public async Task AddImageAsync(Stream file, string imageName)
        {
            _logger.LogInformation($"Start uploading image {nameof(imageName)} to storage."); 

            await _blobStorageService.UploadDataAsync(file, imageName).ConfigureAwait(false);

            string messageId = await _queueStorageService.EnqueueMessageAsync(imageName).ConfigureAwait(false);

            _logger.LogInformation($"Image {nameof(imageName)} is uploaded to storage.");
        }
    }
}
