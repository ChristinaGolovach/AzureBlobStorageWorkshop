﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobStorageWorkshop.Services.Interfaces
{
    public interface IQueueStorageService
    {
        Task<string> EnqueueMessageAsync(string message);
    }
}
