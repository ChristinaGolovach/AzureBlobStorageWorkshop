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
        private IStorageService _storageService;

        public ImagesController(IStorageService storageService)
        {
            _storageService = storageService;
        }
        
        [HttpGet]
        public async Task<ActionResult> GetAllFile()
        {
            var result =  await _storageService.GetBlobNameListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetFile(string id)
        {
            var dataStream = await _storageService.DownloadDataAsync(id);           

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
            if (file != null)
            {
                using (var stream = file.OpenReadStream())
                {
                    await _storageService.UploadDataAsync(stream, file.FileName);
                }
            }

            return Ok();
        }
    }
}