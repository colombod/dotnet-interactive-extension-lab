using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;
using SixLabors.ImageSharp;
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
                var data = GetImageBytes(image);
                var imgTag = CreateImgTag(data, id, image.Height, image.Width);
                writer.Write(imgTag);
            }, HtmlFormatter.MimeType);

            await kernel.SendAsync(
                new DisplayValue(new FormattedValue(
                    "text/markdown",
                    $"Added support for SixLabors.ImageSharp to kernel {kernel.Name}.")));
        }

        private static byte[] GetImageBytes(Image image)
        {
            using var stream = new MemoryStream();
            image.Save(stream, new PngEncoder());
            stream.Flush();
            var data = stream.ToArray();
            return data;
        }

        private static PocketView CreateImgTag(byte[] data, string id, int height, int width)
        {
            var imageSource = $"data:image/png;base64, {Convert.ToBase64String(data)}";
            PocketView imgTag = img[id: id, src: imageSource, height: height, width: width]();
            return imgTag;
        }
    }
}
