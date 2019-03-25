using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using AzureBlobStorageWorkshop.Services.Interfaces;

namespace AzureBlobStorageWorkshop.Services
{
    public class StorageService : IStorageService
    {
        private readonly string _containerName = "imagescontainer";
        private readonly string _storageConnectionString;        
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient; 

        public StorageService(string storageConnectionString)
        {
            _storageConnectionString = storageConnectionString ?? throw new ArgumentNullException($"The {nameof(storageConnectionString)} can not be null.");

            _storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _blobClient.DefaultRequestOptions = new BlobRequestOptions() { ParallelOperationThreadCount = 4 };
        }

        public async Task<Stream> DownloadDataAsync(string id)
        {
            var blobContainer = _blobClient.GetContainerReference(_containerName);

            var blockBlob = blobContainer.GetBlockBlobReference(id);

            if (await blockBlob.ExistsAsync().ConfigureAwait(false))
            {
                Stream targetStream = new MemoryStream();

                await blockBlob.DownloadToStreamAsync(targetStream).ConfigureAwait(false);

                return targetStream;
            }

            return null;
        }

        public async Task<string> UploadDataAsync(Stream dataSource, string fileName) 
        {
            var blobContainer = _blobClient.GetContainerReference(_containerName);

            await blobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null).ConfigureAwait(false);

            var blobId = Guid.NewGuid().ToString();

            var blockBlob = blobContainer.GetBlockBlobReference(blobId);

            using (dataSource)
            {
                await blockBlob.UploadFromStreamAsync(dataSource).ConfigureAwait(false);
            }

            return blobId;
        }

        public async Task<IEnumerable<string>> GetBlobNameListAsync()
        {
            var blobContainer = _blobClient.GetContainerReference(_containerName);

            List<string> allBlockBlobs = new List<string>();

            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var resultSegment = await blobContainer.ListBlobsSegmentedAsync(blobContinuationToken).ConfigureAwait(false);

                blobContinuationToken = resultSegment.ContinuationToken;

                foreach (IListBlobItem item in resultSegment.Results)
                {                
                    if (item is CloudBlockBlob blockBlob)
                    {
                        allBlockBlobs.Add(blockBlob.Name);
                    }
                }

            } while (blobContinuationToken != null);

            return allBlockBlobs;
        }
    }
}
