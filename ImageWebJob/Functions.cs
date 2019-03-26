using System.IO;
using Microsoft.Azure.WebJobs;
using System.Drawing;
using System.Drawing.Imaging;
using StackBlur.Extensions;

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
            using (var bitmap = new Bitmap(inputImageBlob))
            {
                int radius = 100;

                bitmap.StackBlur(radius);

                ImageFormat imgFormat = bitmap.RawFormat;

                bitmap.Save(outputImageBlob, imgFormat);
            }
        }
    }
}
