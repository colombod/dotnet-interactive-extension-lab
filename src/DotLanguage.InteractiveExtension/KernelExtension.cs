using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;

namespace DotLanguage.InteractiveExtension;

public class KernelExtension : IKernelExtension
{
    public Task OnLoadAsync(Kernel kernel)
    {
        if (kernel is CompositeKernel cs)
        {
            cs.Add(new DotLanguageKernel());
            KernelInvocationContext.Current?.Display(
                new HtmlString(@"<details><summary>Draw networks using dot language.</summary>
        <p>This extension adds support for <a href=""https://www.graphviz.org/doc/info/lang.html"">dot </a> languages via <a href=""https://github.com/hpcc-systems/hpcc-js-wasm"">hpcc-js/wasm</a>. Try this code:</p>
<pre>
    <code>
#!dot
graph ethane {
    C_0 -- H_0 [type=s];
    C_0 -- H_1 [type=s];
    C_0 -- H_2 [type=s];
    C_0 -- C_1 [type=s];
    C_1 -- H_3 [type=s];
    C_1 -- H_4 [type=s];
    C_1 -- H_5 [type=s];
}
    </code>
</pre>

Change layout engine like this:

</pre>
#!dot --layout-engine neato
graph G {
    run -- intr;
    intr -- runbl;
    runbl -- run;
    run -- kernel;
    kernel -- zombie;
    kernel -- sleep;
    kernel -- runmem;
    sleep -- swap;
    swap -- runswap;
    runswap -- new;
    runswap -- runmem;
    new -- runmem;
    sleep -- runmem;
}
    </code>
</pre>


        </details>"),
                "text/html");
        }
        return Task.CompletedTask;
    }
}