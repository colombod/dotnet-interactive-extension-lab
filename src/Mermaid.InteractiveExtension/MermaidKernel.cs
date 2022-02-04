using System.Threading.Tasks;

using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;

namespace Mermaid.InteractiveExtension;

public class MermaidKernel : Kernel
    , IKernelCommandHandler<SubmitCode>

{

    private ChooseMermaidKernelDirective? _chooseKernelDirective;

    public MermaidKernel() : base("mermaid")
    {
    }

    public Task HandleAsync(SubmitCode command, KernelInvocationContext context)
    {
        string? width = null;
        string? height = null;
        string? background = null;
        if (ChooseKernelDirective is ChooseMermaidKernelDirective chooser)
        {
            width = command.KernelChooserParseResult.ValueForOption(chooser.WidthOption);
            height = command.KernelChooserParseResult.ValueForOption(chooser.HeightOption);
            background = command.KernelChooserParseResult.ValueForOption(chooser.BackgroundOption);
        }

        var markdown = new MermaidMarkdown(command.Code)
        {
            Width = width ?? string.Empty,
            Height = height ?? string.Empty,
            Background = string.IsNullOrWhiteSpace(background) ? "white" : background
        };
        context.Display(markdown);
        return Task.CompletedTask;
    }

    public override ChooseKernelDirective ChooseKernelDirective => _chooseKernelDirective ??= new ChooseMermaidKernelDirective(this);
}