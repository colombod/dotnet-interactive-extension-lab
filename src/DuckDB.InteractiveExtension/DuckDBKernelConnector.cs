using System.Threading.Tasks;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Connection;

namespace DuckDB.InteractiveExtension;

public class DuckDBKernelConnector : IKernelConnector
{
    public DuckDBKernelConnector(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; }

    public Task<Kernel> CreateKernelAsync(string kernelName)
    {
        var kernel = new DuckDBKernel(
            $"{kernelName}",
            ConnectionString);

        return Task.FromResult<Kernel>(kernel);
    }
}