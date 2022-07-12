using System;
using System.Text;
using System.Text.RegularExpressions;
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
            width = command.KernelChooserParseResult?.GetValueForOption(chooser.WidthOption);
            height = command.KernelChooserParseResult?.GetValueForOption(chooser.HeightOption);
        }
        
        var code = GenerateHtml(command.Code, new Uri("https://cdn.jsdelivr.net/npm/@hpcc-js/wasm@1.15.0/dist/index.min.js", UriKind.Absolute), new Uri("https://cdn.jsdelivr.net/npm/@hpcc-js/wasm@1.15.0/dist", UriKind.Absolute), "1.15.0", _cacheBuster, width, height);
        context.Display(code);
        return Task.CompletedTask;

    }

    public override ChooseKernelDirective ChooseKernelDirective => _chooseKernelDirective ??= new ChooseDotLanguageKernelDirective(this);

    private IHtmlContent GenerateHtml(string commandCode, Uri libraryUri, Uri wasmFolder, string? libraryVersion, string cacheBuster, string? width, string? height)
    {
        var requireUri = new Uri("https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
        var divId = $"hppc_{Guid.NewGuid().ToString("N")}";
        var code = new StringBuilder();
        var renderingFunctionName = $"loadHpcc_{divId}";
        code.AppendLine("<div>");
        // the svg part
        code.AppendLine($"<div id=\"container_{divId}\">");
        code.AppendLine($"<div id=\"{divId}\" style=\"height:{height}; width:{width}\"></div>");
        code.AppendLine("</div>");

        // the script part
        code.AppendLine(@"<script type=""text/javascript"" defer>");
        AppendJsCode(code, divId, renderingFunctionName, libraryUri, wasmFolder, libraryVersion, cacheBuster, commandCode);
        code.AppendLine(JavascriptUtilities.GetCodeForEnsureRequireJs(requireUri, renderingFunctionName));
        code.AppendLine("</script>");
        code.AppendLine("</div>");

        var html = new HtmlString(code.ToString());
        return html;
    }

    private static void AppendJsCode(StringBuilder stringBuilder,
        string divId, string functionName, Uri libraryUri, Uri wasmFolder, string? libraryVersion, string cacheBuster, string code)
    {
        libraryVersion ??= "1.15.0";
        stringBuilder.AppendLine($@"
{functionName} = () => {{");

        var libraryAbsoluteUri = Regex.Replace(libraryUri.AbsoluteUri, @"(\.js)$", string.Empty);
        cacheBuster ??= Guid.NewGuid().ToString("N");
        stringBuilder.AppendLine($@" 
        (require.config({{ 'paths': {{ 'context': '{libraryVersion}' , 'd3': 'https://cdn.jsdelivr.net/npm/d3@7.4.4/dist/d3.min', 'hpcc' : '{libraryAbsoluteUri}', 'urlArgs': 'cacheBuster={cacheBuster}' }}}}) || require)(['d3','hpcc'], (d3, hpcc) => {{");


        stringBuilder.AppendLine($@"
            let container = document.getElementById('{divId}');
            let dot = `{code}`;
            hpcc.wasmFolder(`{wasmFolder.AbsoluteUri}`);
            hpcc.graphviz.layout(dot, ""svg"", ""dot"").then(svg => {{ 
                d3.select('#container_{divId}').html(svg);
                svg = d3.select('#container_{divId}').select('svg');
                let g = svg.select('g');
                let [x, y, width, height] = svg.attr('viewBox').split(' ');
                let zoom = d3.zoom();
                svg.call(zoom
                .extent([
                    [0, 0],
                    [width, height]
                ])
                .scaleExtent([0.1, 8])
                .on(""zoom"", ({{
                    transform
                }}) => g.attr('transform', transform))
            );
            svg.call(zoom.translateTo, width / 2, -height / 2);
            container.innerHTML = svg; 
            }});
        }},
        (error) => {{
            console.log(error);
        }});
}}");
    }
}