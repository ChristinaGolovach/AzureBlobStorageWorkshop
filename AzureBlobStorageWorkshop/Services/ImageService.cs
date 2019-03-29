using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.Extensions.AzureTableStorage;
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
        private readonly Logger _logger;

        public ImageService(IBlobStorageService blobStorageService, IQueueStorageService queueStorageService)
        {
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException($"The {nameof(blobStorageService)} can not be null.");
            _queueStorageService = queueStorageService ?? throw new ArgumentNullException($"The {nameof(queueStorageService)} can not be null.");

            //var azureStorageTarget = (AzureTableStorageTarget)LogManager.Configuration.FindTargetByName("AzureTableStorage");
            //azureStorageTarget.ConnectionString = connection;
            //LogManager.ReconfigExistingLoggers();           

            _logger = LogManager.GetCurrentClassLogger();     

        }

        public async Task<IEnumerable<string>> GetAllImagesNameAsync()
        {
            _logger.Info($"Loading all images name from storage.");

            var result =  await _blobStorageService.GetBlobNameListAsync();

            return result;
        }

        public async Task<Stream> GetImageAsync(string imageName)
        {
            _logger.Info($"Loading image {nameof(imageName)} from storage.");

            var dataStream = await _blobStorageService.DownloadDataAsync(imageName).ConfigureAwait(false);

            if (dataStream == null)
            {
                _logger.Error($"Image {nameof(imageName)} not found.");
            }  

           _logger.Info($"Image {nameof(imageName)} is loaded from storage.");

            return dataStream;
        }

        public async Task AddImageAsync(Stream file, string imageName)
        {
            _logger.Info($"Start uploading image {nameof(imageName)} to storage."); 

            await _blobStorageService.UploadDataAsync(file, imageName).ConfigureAwait(false);

            string messageId = await _queueStorageService.EnqueueMessageAsync(imageName).ConfigureAwait(false);

            _logger.Info($"Image {nameof(imageName)} is uploaded to storage.");
        }
    }
}
