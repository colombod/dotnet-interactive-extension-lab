using System.Threading.Tasks;

using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;

namespace DataView.InteractiveExtension;

public static class KernelExtension 
{
    public static Task LoadAsync(Kernel kernel)
    {

        return kernel.SendAsync(
            new DisplayValue(new FormattedValue(
                "text/markdown",
                $"Added support IDataView to kernel {kernel.Name}.")));
    }

}