using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;

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
            width = command.KernelChooserParseResult.GetValueForOption(chooser.WidthOption);
            height = command.KernelChooserParseResult.GetValueForOption(chooser.HeightOption);
        }

        var code = GenerateHtml(command.Code, new Uri("https://cdn.jsdelivr.net/npm/@hpcc-js/wasm@1.14.1/dist/index.min.js", UriKind.Absolute),  width, height);
        context.Display(code);
        return Task.CompletedTask;

    }

    public override ChooseKernelDirective ChooseKernelDirective => _chooseKernelDirective ??= new ChooseDotLanguageKernelDirective(this);

    private IHtmlContent GenerateHtml(string commandCode, Uri libraryUri,  string? width, string? height)
    {
       
        var divId = Guid.NewGuid().ToString("N");
        var code = new StringBuilder();
        var functionName = $"loadHpccWasm_{divId}";
        code.AppendLine("<div >");

       

        code.AppendLine($"<div id=\"{divId}\" style=\"height:{height}; width:{width}\"></div>");
        code.AppendLine("</div>");

        code.AppendLine(@"<script type=""text/javascript"" defer>");
        AppendJsCode(code, divId, functionName, libraryUri, commandCode);
        code.AppendLine("</script>");        
        
        var html = new HtmlString(code.ToString());
        return html;
    }

    private static void AppendJsCode(StringBuilder stringBuilder,
        string divId, string functionName, Uri libraryUri, string code)
    {
  
        stringBuilder.AppendLine($@"
{functionName} = () => {{
    let container = document.getElementById('{divId}');
    const dot = `{code}`;
            
    let hpccWasm = window['@hpcc-js/wasm'];
    hpccWasm.graphviz.layout(dot, ""svg"", ""dot"").then(svg => {{ container.innerHTML = svg; }});
}}

if (window[""@hpcc-js/wasm""]) {{
    {functionName}();
    }} else {{
    let hpccWasm_script = document.createElement('script');
    hpccWasm_script.setAttribute('src', '{libraryUri.AbsoluteUri}');
    hpccWasm_script.setAttribute('type', 'text/javascript');
    hpccWasm_script.setAttribute('defer', 'true');
    hpccWasm_script.onload = () => {{ {functionName}(); }};
    document.getElementsByTagName('head')[0].appendChild(hpccWasm_script);
}}");
    }
}