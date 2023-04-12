using System.CommandLine;
using Microsoft.DotNet.Interactive;

namespace DuckDB.InteractiveExtension;

public class ChooseDuckDBKernelDirective : ChooseKernelDirective
{
    public ChooseDuckDBKernelDirective(Kernel kernel) : base(kernel, $"Run a DuckDB query using the \"{kernel.Name}\" connection.")
    {
        Add(NameOption);
    }

    public Option<string> NameOption { get; } = new(
        "--name",
        description: "Specify the value name to store the results.",
        getDefaultValue: () => "lastResults");

}