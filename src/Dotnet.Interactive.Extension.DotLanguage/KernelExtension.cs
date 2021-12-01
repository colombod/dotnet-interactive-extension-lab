using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Http;

namespace Dotnet.Interactive.Extension.DotLanguage
{
    public class KernelExtension : IKernelExtension
    {
        public Task OnLoadAsync(Kernel kernel)
        {
            if (kernel is CompositeKernel cs)
            {
                cs.Add(new DotLanguageKernel());
                KernelInvocationContext.Current?.Display(
                    new HtmlString(@"<details><summary>Draw networks using dto language.</summary>
        <p>This extension adds support for dot language. Try this code:</p>
<pre>
    <code>
#!dot
dinetwork {node[shape=circle]; 1 -> 1 -> 2; 2 -> 3; 2 -- 4; 2 -> 1 [style=dotted, width=3] }
    </code>
</pre>
        </details>"),
                    "text/html");
            }
            return Task.CompletedTask;
        }
    }

    public class DotLanguageKernel : Kernel,
        IKernelCommandHandler<SubmitCode>
    {
        private readonly string _cacheBuster;

        public DotLanguageKernel() : base("dot")
        {
            _cacheBuster = Guid.NewGuid().ToString("N");
        }

        public Task HandleAsync(SubmitCode command, KernelInvocationContext context)
        {
            var code = GenerateHtml(command.Code, new Uri("https://visjs.github.io/vis-network/standalone/umd/vis-network.min.js", UriKind.Absolute), null, _cacheBuster);
            context.Display(code);
            return Task.CompletedTask;

        }

        private IHtmlContent GenerateHtml(string commandCode, Uri libraryUri, string libraryVersion, string cacheBuster)
        {
            var requireUri = new Uri("https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
            var divId = Guid.NewGuid().ToString("N");
            var code = new StringBuilder();
            var functionName = $"loadVisjs_{divId}";
            code.AppendLine("<div >");

            code.AppendLine(@"<script type=""text/javascript"">");
            AppendJsCode(code, divId, functionName, libraryUri, libraryVersion, cacheBuster,commandCode);
            code.AppendLine(JavascriptUtilities.GetCodeForEnsureRequireJs(requireUri, functionName));
            code.AppendLine("</script>");

            code.AppendLine($"<div id=\"{divId}\" style=\"height:600px;\"></div>");
            code.AppendLine("</div>");

            var html = new HtmlString(code.ToString());
            return html;
        }

        private static void AppendJsCode(StringBuilder stringBuilder,
            string divId, string functionName, Uri libraryUri, string libraryVersion, string cacheBuster, string code)
        {
            libraryVersion ??= "1.0.0";
            stringBuilder.AppendLine($@"
{functionName} = () => {{");
       
                var libraryAbsoluteUri = libraryUri.AbsoluteUri.Replace(".js", string.Empty);
                cacheBuster ??= Guid.NewGuid().ToString("N");
                stringBuilder.AppendLine($@" 
        (require.config({{ 'paths': {{ 'context': '{libraryVersion}', 'visjs' : '{libraryAbsoluteUri}', 'urlArgs': 'cacheBuster={cacheBuster}' }}}}) || require)(['visjs'], (visjs) => {{");
            

            stringBuilder.AppendLine($@"
            let container = document.getElementById('{divId}');
            let dot = `{code}`;
            let data = visjs.parseDOTNetwork(dot);
            let network = new visjs.Network(container, data); 
        }},
        (error) => {{
            console.log(error);
        }});
}}");
        }
    }
}

