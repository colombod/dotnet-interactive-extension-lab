using System.CommandLine;
using Microsoft.DotNet.Interactive;

namespace Dotnet.Interactive.Extension.DotLanguage;

internal class ChooseDotLanguageKernelDirective : ChooseKernelDirective
{
    public ChooseDotLanguageKernelDirective(DotLanguageKernel kernel) : base(kernel, $"Render network graphs using dot language.")
    {
        Add(HeightOption);
        Add(WidthOption);
    }

    public Option<string> WidthOption { get; } = new(
        "--width",
        description: "Specify width for the output.",
        getDefaultValue: () => "100%");

    public Option<string> HeightOption { get; } = new(
        "--height",
        description: "Specify height for the output.",
        getDefaultValue: () => "600px");

}