using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AzureBlobStorageWorkshop.Services.Interfaces;
using NLog.Extensions.AzureTableStorage;
using Microsoft.Extensions.Logging;

namespace AzureBlobStorageWorkshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImagesController(IImageService imageService)
        {
            _imageService = imageService ?? throw new ArgumentNullException($"The {nameof(imageService)} van not be null.");
        }

        [HttpGet]
        public async Task<ActionResult> GetAllFileList()
        {
            var result = await _imageService.GetAllImagesNameAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetFile(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var dataStream = await _imageService.GetImageAsync(id);         

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
                await _imageService.AddImageAsync(stream, file.FileName);

                return Ok(file.FileName);
            }           
        }
    }
}