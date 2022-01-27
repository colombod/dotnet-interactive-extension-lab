using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;

using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;

namespace Dotnet.Interactive.Extension.System.Drawing;

public class KernelExtension : IKernelExtension
{
    public async Task OnLoadAsync(Kernel kernel)
    {
        Formatter.Register<Image>((image, writer) =>
        {
            var imgTag = CreatePocketView(image);
            writer.Write(imgTag);
        }, HtmlFormatter.MimeType);

        Formatter.Register<Bitmap>((image, writer) =>
        {
            var imgTag = CreatePocketView(image);
            writer.Write(imgTag);
        }, HtmlFormatter.MimeType);

        await kernel.SendAsync(
            new DisplayValue(new FormattedValue(
                "text/markdown",
                $"Added support for System.Drawing to kernel {kernel.Name}.")));
    }

    private static PocketView CreatePocketView(Image image)
    {
        var id = Guid.NewGuid().ToString("N");
        using var stream = new MemoryStream();
        image.Save(stream, ImageFormat.Png);
        stream.Flush();
        var data = stream.ToArray();
        var imgTag = CreateImgTag(data, id, image.Height, image.Width);
        return imgTag;
    }

    private static PocketView CreateImgTag(byte[] data, string id, int height, int width)
    {
        var imageSource = $"data:image/png;base64, {Convert.ToBase64String(data)}";
        PocketView imgTag = img[id: id, src: imageSource, height: height, width: width]();
        return imgTag;
    }
}