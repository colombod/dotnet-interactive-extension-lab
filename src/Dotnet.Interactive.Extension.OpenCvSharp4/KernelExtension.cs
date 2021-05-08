using System;
using System.Threading.Tasks;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;
using OpenCvSharp;

using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;

namespace Dotnet.Interactive.Extension.OpenCvSharp4
{
    public class KernelExtension : IKernelExtension
    {
        public async Task OnLoadAsync(Kernel kernel)
        {
            Formatter.Register<Mat>((openCvImage, writer) =>
            {
                var id = Guid.NewGuid().ToString("N");
                var data = openCvImage.ImEncode(".png");
                var imgTag = CreateImgTag(data, id, openCvImage.Height, openCvImage.Width);
                writer.Write(imgTag);
            }, HtmlFormatter.MimeType);

            await kernel.SendAsync(
                new DisplayValue(new FormattedValue(
                    "text/markdown",
                    $"Added support for OpenCvSharp4 to kernel {kernel.Name}.")));
        }

        private static PocketView CreateImgTag(byte[] data, string id, int height, int width)
        {
            var imageSource = $"data:image/png;base64, {Convert.ToBase64String(data)}";
            PocketView imgTag = img[id: id, src: imageSource, height: height, width: width]();
            return imgTag;
        }
    }
}
