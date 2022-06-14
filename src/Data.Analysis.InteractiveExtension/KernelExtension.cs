using System.Data;
using System.Threading.Tasks;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;

namespace Data.Analysis.InteractiveExtension;

public class KernelExtension : IKernelExtension
{
    /// <inheritdoc/>
    public Task OnLoadAsync(Kernel kernel)
    {

        return kernel.SendAsync(
            new DisplayValue(new FormattedValue(
                "text/markdown",
                $"Added support for {nameof(DataView)} to kernel {kernel.Name}.")));
    }

}