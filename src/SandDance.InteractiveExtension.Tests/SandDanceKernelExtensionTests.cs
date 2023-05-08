using System.Diagnostics;
using Assent;
using FluentAssertions;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting;
using Microsoft.DotNet.Interactive.Formatting.Csv;
using Microsoft.DotNet.Interactive.Formatting.TabularData;
using TestUtilities;
using Xunit;

namespace SandDance.InteractiveExtension.Tests;

public class SandDanceKernelExtensionTests : IDisposable
{
    private readonly Configuration _configuration;

    public SandDanceKernelExtensionTests()
    {
        _configuration = new Configuration()
            .SetInteractive(Debugger.IsAttached)
            .UsingExtension("json");
    }

    [Fact]
    public void it_configures_preferred_mimeTypes()
    {
        using var kernel = new CompositeKernel();

        KernelExtension.Load(kernel);

        var mimetypes = Formatter.GetPreferredMimeTypesFor(typeof(SandDanceDataExplorer)).Distinct();

        mimetypes
            .Should().BeEquivalentTo(HtmlFormatter.MimeType, CsvFormatter.MimeType);
    }

    [Fact]
    public void it_registers_html_formatter()
    {
        using var kernel = new CompositeKernel();

        KernelExtension.Load(kernel);

        var data = new[]
        {
            new {Type="orange", Price=1.2},
            new {Type="apple" , Price=1.3},
            new {Type="grape" , Price=1.4}
        };

        var formatted = data.ExploreWithSandDance().ToDisplayString(HtmlFormatter.MimeType);

        formatted.Should().Contain("(['sandDanceUri'], (sandDance) => {");
    }

    [Fact]
    public void it_registers_TabularDataResourceFormatter()
    {
        using var kernel = new CompositeKernel();

        KernelExtension.Load(kernel);

        var data = new[]
        {
            new {Type="orange", Price=1.2},
            new {Type="apple" , Price=1.3},
            new {Type="grape" , Price=1.4}
        };


        var formattedValue = data.ExploreWithSandDance().ToDisplayString(TabularDataResourceFormatter.MimeType);
        formattedValue.Should().Contain("\"profile\": \"tabular-data-resource\"");
    }

    [Fact]
    public void it_is_formatted_as_multiple_mimeTypes()
    {
        using var kernel = new CompositeKernel();

        KernelExtension.Load(kernel);

        var data = new[]
        {
            new {Type="orange", Price=1.2},
            new {Type="apple" , Price=1.3},
            new {Type="grape" , Price=1.4}
        };

        var formattedValues = FormattedValue.CreateManyFromObject(data.ExploreWithSandDance());
        formattedValues.Select(fv => fv.MimeType)
            .Should()
            .BeEquivalentTo(HtmlFormatter.MimeType, CsvFormatter.MimeType);
    }

    [Fact]
    public void widget_code_generation_is_not_broken()
    {
        using var kernel = new CompositeKernel();

        KernelExtension.Load(kernel);

        var data = new[]
        {
            new {Type="orange", Price=1.2},
            new {Type="apple" , Price=1.3},
            new {Type="grape" , Price=1.4}
        };


        var html = data.ExploreWithSandDance().ToDisplayString(HtmlFormatter.MimeType);

        this.Assent(html.FixedGuid().FixedCacheBuster());
    }

    [Fact]
    public void it_can_load_script_from_the_extension()
    {
        using var kernel = new CompositeKernel();

        KernelExtension.Load(kernel);

        kernel.UseSandDanceExplorer();

        var data = new[]
        {
            new {Type="orange", Price=1.2},
            new {Type="apple" , Price=1.3},
            new {Type="grape" , Price=1.4}
        };


        var formatted = data.ExploreWithSandDance().ToDisplayString(HtmlFormatter.MimeType);

        formatted.Should().Contain("configureRequireFromExtension");
    }

    [Fact]
    public void it_checks_and_load_require()
    {
        using var kernel = new CompositeKernel();

        KernelExtension.Load(kernel);

        var data = new[]
        {
            new {Type="orange", Price=1.2},
            new {Type="apple" , Price=1.3},
            new {Type="grape" , Price=1.4}
        };

        var explorer = data.ExploreWithSandDance();
        explorer.LibraryUri = new Uri("https://a.cdn.url/script.js");
        var formatted = explorer.ToDisplayString(HtmlFormatter.MimeType);

        formatted.Should()
            .Contain("if ((typeof(require) !==  typeof(Function)) || (typeof(require.config) !== typeof(Function)))")
            .And
            .Contain("require_script.onload = function()");
    }

    [Fact]
    public void it_can_loads_script_from_uri()
    {
        using var kernel = new CompositeKernel();

        KernelExtension.Load(kernel);

        var data = new[]
        {
            new {Type="orange", Price=1.2},
            new {Type="apple" , Price=1.3},
            new {Type="grape" , Price=1.4}
        };

        var explorer = data.ExploreWithSandDance();
        explorer.LibraryUri = new Uri("https://a.cdn.url/script.js");
        var formatted = explorer.ToDisplayString(HtmlFormatter.MimeType);

        formatted.Should().Contain("require.config(");
    }

    [Fact]
    public void it_can_loads_script_from_uri_and_specify_context()
    {
        using var kernel = new CompositeKernel();

        KernelExtension.Load(kernel);

        var data = new[]
        {
            new {Type="orange", Price=1.2},
            new {Type="apple" , Price=1.3},
            new {Type="grape" , Price=1.4}
        };

        var explorer = data.ExploreWithSandDance();
        explorer.LibraryUri = new Uri("https://a.cdn.url/script.js");
        explorer.LibraryVersion = "2.2.2";
        var formatted = explorer.ToDisplayString(HtmlFormatter.MimeType);

        formatted.Should().Contain("'context': '2.2.2'");
    }

    [Fact]
    public void uri_is_quoted()
    {
        using var kernel = new CompositeKernel();

        KernelExtension.Load(kernel);

        var data = new[]
        {
            new {Type="orange", Price=1.2},
            new {Type="apple" , Price=1.3},
            new {Type="grape" , Price=1.4}
        };

        var explorer = data.ExploreWithSandDance();
        explorer.LibraryUri = new Uri("https://a.cdn.url/script.js");
        var formatted = explorer.ToDisplayString(HtmlFormatter.MimeType);

        formatted.Should().Contain("'https://a.cdn.url/script'");
    }

    [Fact]
    public void uri_extension_is_removed()
    {
        using var kernel = new CompositeKernel();

        KernelExtension.Load(kernel);

        var data = new[]
        {
            new {Type="orange", Price=1.2},
            new {Type="apple" , Price=1.3},
            new {Type="grape" , Price=1.4}
        };

        var explorer = data.ExploreWithSandDance();
        explorer.LibraryUri = new Uri("https://a.cdn.url/script.js");
        var formatted = explorer.ToDisplayString(HtmlFormatter.MimeType);

        formatted.Should()
            .Contain("'https://a.cdn.url/script'")
            .And
            .NotContain("'https://a.cdn.url/script.js'");
    }

    [Fact]
    public void can_specify_cacheBuster()
    {
        using var kernel = new CompositeKernel();

        KernelExtension.Load(kernel);

        var data = new[]
        {
            new {Type="orange", Price=1.2},
            new {Type="apple" , Price=1.3},
            new {Type="grape" , Price=1.4}
        };

        var explorer = data.ExploreWithSandDance();
        explorer.LibraryUri = new Uri("https://a.cdn.url/script.js");
        explorer.CacheBuster = "XYZ";
        var formatted = explorer.ToDisplayString(HtmlFormatter.MimeType);

        formatted.Should().Contain("'urlArgs': 'cacheBuster=XYZ'");
    }

    public void Dispose()
    {
        Formatter.ResetToDefault();
        SandDanceDataExplorer.ConfigureDefaults();
    }
}