using System;
using System.Threading.Tasks;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;
using Microsoft.DotNet.Interactive.Formatting.Csv;

namespace lobe.InteractiveExtension;

public class KernelExtension : IKernelExtension
{
    /// <inheritdoc/>
    public Task OnLoadAsync(Kernel kernel)
    {
        //Formatter.Register<Classification>((Classification classification, FormatContext context) =>
        //{
        //    throw new NotImplementedException();
        //    return true;
        //}, HtmlFormatter.MimeType);

        //Formatter.Register<ClassificationResults>((ClassificationResults classificationResults, FormatContext context) =>
        //{
        //    throw new NotImplementedException();
        //    return true;
        //}, HtmlFormatter.MimeType);

        //Formatter.Register<ClassificationResults>((ClassificationResults classificationResults, FormatContext context) =>
        //{
        //    throw new NotImplementedException();
        //    return true;
        //}, CsvFormatter.MimeType);

        Formatter.SetPreferredMimeTypesFor(typeof(ClassificationResults), HtmlFormatter.MimeType, CsvFormatter.MimeType);

        return kernel.SendAsync(
            new DisplayValue(new FormattedValue(
                "text/markdown",
                $"Added support for lobe {kernel.Name}.")));
    }

}