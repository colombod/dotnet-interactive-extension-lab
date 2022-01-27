using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;

namespace Dotnet.Interactive.Extension.DotLanguage;

public class KernelExtension : IKernelExtension
{
    public Task OnLoadAsync(Kernel kernel)
    {
        if (kernel is CompositeKernel cs)
        {
            cs.Add(new DotLanguageKernel());
            KernelInvocationContext.Current?.Display(
                new HtmlString(@"<details><summary>Draw networks using dot language.</summary>
        <p>This extension adds support for dot language. Try this code:</p>
<pre>
    <code>
#!dot
dinetwork {node[shape=circle]; 1 -> 1 -> 2; 2 -> 3; 2 -- 4; 2 -> 1 [style=dotted, width=3] }
    </code>
</pre>
<p>Display size can be changed. Try this code:</p>
<pre>
    <code>
#!dot --display-height 300px
dinetwork {node[shape=circle]; 1 -> 1 -> 2; 2 -> 3; 2 -- 4; 2 -> 1 [style=dotted, width=3] }
    </code>
</pre>
        </details>"),
                "text/html");
        }
        return Task.CompletedTask;
    }
}