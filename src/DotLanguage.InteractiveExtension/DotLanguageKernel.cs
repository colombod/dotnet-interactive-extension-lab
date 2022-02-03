using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Http;

namespace DotLanguage.InteractiveExtension;

internal class DotLanguageKernel : Kernel,
    IKernelCommandHandler<SubmitCode>
{
    private readonly string _cacheBuster;

    private ChooseDotLanguageKernelDirective? _chooseKernelDirective;

    public DotLanguageKernel() : base("dot")
    {
        _cacheBuster = Guid.NewGuid().ToString("N");
    }

    public Task HandleAsync(SubmitCode command, KernelInvocationContext context)
    {
        var width = "100%";
        var height = "600px";
        if (ChooseKernelDirective is ChooseDotLanguageKernelDirective chooser)
        {
            width = command.KernelChooserParseResult.ValueForOption(chooser.WidthOption);
            height = command.KernelChooserParseResult.ValueForOption(chooser.HeightOption);
        }

        var code = GenerateHtml(command.Code, new Uri("https://visjs.github.io/vis-network/standalone/umd/vis-network.min.js", UriKind.Absolute), null, _cacheBuster, width, height);
        context.Display(code);
        return Task.CompletedTask;

    }

    public override ChooseKernelDirective ChooseKernelDirective => _chooseKernelDirective ??= new ChooseDotLanguageKernelDirective(this);

    private IHtmlContent GenerateHtml(string commandCode, Uri libraryUri, string? libraryVersion, string cacheBuster, string? width, string? height)
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

        code.AppendLine($"<div id=\"{divId}\" style=\"height:{height}; width:{width}\"></div>");
        code.AppendLine("</div>");

        var html = new HtmlString(code.ToString());
        return html;
    }

    private static void AppendJsCode(StringBuilder stringBuilder,
        string divId, string functionName, Uri libraryUri, string? libraryVersion, string cacheBuster, string code)
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