using System;
using System.Threading.Tasks;

using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;

using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;

namespace Dotnet.Interactive.Extension.ImageSharp
{
    public class KernelExtension : IKernelExtension
    {
        public async Task OnLoadAsync(Kernel kernel)
        {
            Formatter.Register<Image>((image, writer) =>
            {
                var id = Guid.NewGuid().ToString("N");
     
                var imgTag = CreateImgTag(image, id, image.Height, image.Width);
                writer.Write(imgTag);
            }, HtmlFormatter.MimeType);

            await kernel.SendAsync(
                new DisplayValue(new FormattedValue(
                    "text/markdown",
                    $"Added support for SixLabors.ImageSharp to kernel {kernel.Name}.")));
        }

        private static PocketView CreateImgTag(Image image, string id, int height, int width)
        {
            var format = image.Frames.Count > 1 ? (IImageFormat)GifFormat.Instance : PngFormat.Instance;
            var imageSource = image.ToBase64String(format);
            PocketView imgTag = img[id: id, src: imageSource, height: height, width: width]();
            return imgTag;
        }
    }
}
