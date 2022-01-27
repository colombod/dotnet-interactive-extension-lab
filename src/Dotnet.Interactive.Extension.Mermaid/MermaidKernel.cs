using System.Threading.Tasks;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;

namespace Dotnet.Interactive.Extension.Mermaid;

public class MermaidKernel : Kernel
    , IKernelCommandHandler<SubmitCode>

{
    public MermaidKernel() : base("mermaid")
    {
    }

    public Task HandleAsync(SubmitCode command, KernelInvocationContext context)
    {
        var markdown = new MermaidMarkdown(command.Code);
        context.Display(markdown);
        return Task.CompletedTask;
    }
}