using Microsoft.DotNet.Interactive.Commands;

namespace DuckDB.InteractiveExtension;

public class ConnectDuckDBKernel : ConnectKernelCommand
{
    public ConnectDuckDBKernel(string connectedKernelName) : base(connectedKernelName)
    {
    }

    public string ConnectionString { get; set; }
}