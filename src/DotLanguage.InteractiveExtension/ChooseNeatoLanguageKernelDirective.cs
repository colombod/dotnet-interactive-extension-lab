using System.CommandLine;
using Microsoft.DotNet.Interactive;

namespace DotLanguage.InteractiveExtension;

internal class ChooseNeatoLanguageKernelDirective : ChooseKernelDirective
{
    public ChooseNeatoLanguageKernelDirective(NeatoLanguageKernel kernel) : base(kernel, $"Render network graphs using dot language.")
    {
        Add(HeightOption);
        Add(WidthOption);
    }

    public Option<string> WidthOption { get; } = new(
        "--display-width",
        description: "Specify width for the display.",
        getDefaultValue: () => "100%");

    public Option<string> HeightOption { get; } = new(
        "--display-height",
        description: "Specify height for the display.",
        getDefaultValue: () => "600px");

}