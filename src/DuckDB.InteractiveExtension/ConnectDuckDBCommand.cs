using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Connection;

namespace DuckDB.InteractiveExtension;

public class ConnectDuckDBCommand : ConnectKernelCommand
{
    public ConnectDuckDBCommand()
        : base("duckdb", "Connects to a DuckDB database")
    {
        Add(ConnectionStringArgument);
    }

    public Argument<string> ConnectionStringArgument { get; } =
        new("connectionString", "The connection string used to connect to the database");

    public override Task<Kernel> ConnectKernelAsync(
        KernelInvocationContext context,
        InvocationContext commandLineContext)
    {
        var connectionString = commandLineContext.ParseResult.GetValueForArgument(ConnectionStringArgument);
        var connector = new DuckDBKernelConnector(connectionString);
        var localName = commandLineContext.ParseResult.GetValueForOption(KernelNameOption);
        return connector.CreateKernelAsync(localName);
    }
}