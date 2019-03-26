using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AzureBlobStorageWorkshop.Services.Interfaces;

namespace AzureBlobStorageWorkshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private IBlobStorageService _blobStorageService;
        private IQueueStorageService _queueStorageService;

        public ImagesController(IBlobStorageService blobStorageService, IQueueStorageService queueStorageService)
        {
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException($"The {nameof(blobStorageService)} van not be null.");
            _queueStorageService = queueStorageService ?? throw new ArgumentNullException($"The {nameof(queueStorageService)} van not be null.");
        }

        [HttpGet]
        public async Task<ActionResult> GetAllFileList()
        {
            var result =  await _blobStorageService.GetBlobNameListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetFile(string id)
        {
            var dataStream = await _blobStorageService.DownloadDataAsync(id);           

            if (dataStream == null)
            {
                return NotFound();
            }

            dataStream.Position = 0;

            //TODO image/jpeg - вынести как-то и получать в зависимости от конфиг. storage service
            return File(dataStream, "image/jpeg");
        } 

        [HttpPost]
        public async Task<ActionResult> PostFile(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest();               
            }

            using (var stream = file.OpenReadStream())
            {
                await _blobStorageService.UploadDataAsync(stream, file.FileName);

                string messageId =  await _queueStorageService.EnqueueMessageAsync(file.FileName);

                return Ok(); //blobId
            }           
        }
    }
}