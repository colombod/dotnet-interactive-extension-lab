using System.Data;
using System.Threading.Tasks;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;

namespace Data.Analysis.InteractiveExtension;

public static class KernelExtension
{
    public static Task LoadAsync(Kernel kernel)
    {

        return kernel.SendAsync(
            new DisplayValue(new FormattedValue(
                "text/markdown",
                $"Added support for {nameof(DataView)} to kernel {kernel.Name}.")));
    }

}