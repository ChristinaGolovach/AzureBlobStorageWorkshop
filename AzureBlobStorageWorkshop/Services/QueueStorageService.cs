﻿using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using AzureBlobStorageWorkshop.Services.Interfaces;

namespace AzureBlobStorageWorkshop.Services
{
    public class QueueStorageService : IQueueStorageService
    {
        //TODO move _queueName outside
        private readonly string _queueName = "imagequeue";
        private CloudStorageAccount _storageAccount;
        private CloudQueueClient _queueClient;

        public QueueStorageService(string storageConnectionString)
        {
            storageConnectionString = storageConnectionString ?? throw new ArgumentNullException($"The {nameof(storageConnectionString)} can not be null.");

            _storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _queueClient = _storageAccount.CreateCloudQueueClient();
        } 
        
        public async Task<string> EnqueueMessageAsync(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException($"The {nameof(message)} can not be null or empty.");
            }

            var cloudQueue = _queueClient.GetQueueReference(_queueName);

            await cloudQueue.CreateIfNotExistsAsync().ConfigureAwait(false);

            var queueMessage = new CloudQueueMessage(message);

            await cloudQueue.AddMessageAsync(queueMessage).ConfigureAwait(false);

            return queueMessage.Id;
        }
    }
}
