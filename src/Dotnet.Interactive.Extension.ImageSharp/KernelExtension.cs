using System;
using System.Threading.Tasks;

using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;

namespace Dotnet.Interactive.Extension.ImageSharp
{
    /// <summary>
    /// A <see cref="IKernelExtension"/> implementation adding support for ImageSharp images.
    /// </summary>
    public class KernelExtension : IKernelExtension
    {
        /// <inheritdoc/>
        public Task OnLoadAsync(Kernel kernel)
        {
            RegisterFormatters();

            return kernel.SendAsync(
                new DisplayValue(new FormattedValue(
                    "text/markdown",
                    $"Added support for SixLabors.ImageSharp to kernel {kernel.Name}.")));
        }

        /// <summary>
        /// Registers the formatters.
        /// </summary>
        public static void RegisterFormatters()
        {
            Formatter.Register<Image>(
                (image, writer) =>
                {
                    var id = Guid.NewGuid().ToString("N");
                    var imgTag = CreateImgTag(image, id, image.Height, image.Width);
                    writer.Write(imgTag);
                }, HtmlFormatter.MimeType);


            Formatter.Register<Color>((color, writer) =>

            {
                var img = new Image<Rgba32>(36, 24, color);
                img.Mutate(c =>
                {
                    c.DrawPolygon(Color.Black, 2.0f, new[]
                    {
                        new PointF(0f, 0f),
                        new PointF(img.Width, 0f),
                        new PointF(img.Width, img.Height),
                        new PointF(0f, img.Height),
                    });
                });

                var id = Guid.NewGuid().ToString("N");
                var imgTag = CreateImgTag(img, id, img.Height, img.Width);

                PocketView colorSwatch = div(table(tr(
                    td(imgTag),
                    td(color.ToString())))
                );
                writer.Write(colorSwatch.ToDisplayString());

            }, HtmlFormatter.MimeType);
        }

        private static PocketView CreateImgTag(Image image, string id, int height, int width)
        {
            var format = image.Frames.Count > 1
                ? (IImageFormat)GifFormat.Instance
                : PngFormat.Instance;

            var imageSource = image.ToBase64String(format);

            return (PocketView)img[id: id, src: imageSource, height: height, width: width]();
        }
    }
}
