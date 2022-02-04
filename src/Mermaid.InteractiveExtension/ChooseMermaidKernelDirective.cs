using System.CommandLine;
using Microsoft.DotNet.Interactive;

namespace Mermaid.InteractiveExtension;

internal class ChooseMermaidKernelDirective : ChooseKernelDirective
{
    public ChooseMermaidKernelDirective(MermaidKernel kernel) : base(kernel, "Render mermaid markdown.")
    {
        Add(HeightOption);
        Add(WidthOption);
        Add(BackgroundOption);
    }

    public Option<string> WidthOption { get; } = new(
        "--display-width",
        description: "Specify width for the display.",
        getDefaultValue: () => "");

    public Option<string> HeightOption { get; } = new(
        "--display-height",
        description: "Specify height for the display.",
        getDefaultValue: () => "");

    public Option<string> BackgroundOption { get; } = new(
        "--display-background-color",
        description: "Specify background color for the display.",
        getDefaultValue: () => "white");

}