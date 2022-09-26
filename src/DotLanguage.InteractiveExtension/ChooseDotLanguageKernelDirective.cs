using System;
using System.CommandLine;
using Microsoft.DotNet.Interactive;

namespace DotLanguage.InteractiveExtension;

internal class ChooseDotLanguageKernelDirective : ChooseKernelDirective
{
    public ChooseDotLanguageKernelDirective(DotLanguageKernel kernel) : base(kernel, "Render network graphs using dot language.")
    {
        Add(LayoutEngineOption);
        Add(HeightOption);
        Add(WidthOption);
    }

    public Option<LayoutEngine> LayoutEngineOption { get; } = new(
        "--layout-engine",
        description: "Specify the layout engine.",
        getDefaultValue: () => LayoutEngine.dot);

    public Option<string> WidthOption { get; } = new(
        "--display-width",
        description: "Specify width for the display.",
        getDefaultValue: () => "100%");


    public Option<string> HeightOption { get; } = new(
        "--display-height",
        description: "Specify height for the display.",
        getDefaultValue: () => "600px");

}

internal enum LayoutEngine
{
    circo,
    dot,
    fdp,
    sfdp,
    neato,
    osage,
    patchwork,
    twopi,
}