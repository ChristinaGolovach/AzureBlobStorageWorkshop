using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
//using SixLabors.ImageSharp.Processing.Processors.


namespace ImageWebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void AddBlurEffect
            ([QueueTrigger("imagequeue")] string imageId,
            [Blob("imagescontainer/{queueTrigger}", FileAccess.Read)] Stream inputImageBlob, 
            [Blob("imagescontainer/{queueTrigger}", FileAccess.Write)] Stream outputImageBlob)
        {
            using (var image = Image.Load(inputImageBlob))
            {
                image.Mutate(i => i.GaussianBlur());

                image.SaveAsJpeg(outputImageBlob);
            }
        }
    }
}
