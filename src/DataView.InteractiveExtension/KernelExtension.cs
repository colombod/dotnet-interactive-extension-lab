using System.Threading.Tasks;

using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;

namespace DataView.InteractiveExtension;

public class KernelExtension : IKernelExtension
{
    /// <inheritdoc/>
    public Task OnLoadAsync(Kernel kernel)
    {

        return kernel.SendAsync(
            new DisplayValue(new FormattedValue(
                "text/markdown",
                $"Added support IDataView to kernel {kernel.Name}.")));
    }

}