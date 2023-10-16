using FluentAssertions;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting;
using TestUtilities;
using Xunit;
using DuckDB.NET.Data;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting.TabularData;

namespace DuckDB.InteractiveExtension.Tests;

public class DuckDBConnectionTests
{

    [Fact]
    public async Task It_can_connect_and_query_data()
    {
        using var kernel = new CompositeKernel
        {
            new CSharpKernel(),
            new KeyValueStoreKernel()
        };

        kernel.AddKernelConnector(new ConnectDuckDBCommand());

        using var _ = CreateInMemoryDuckDB(out var connectionString);

        var result = await kernel.SubmitCodeAsync(
            $"#!connect duckdb --kernel-name mydb  \"{connectionString}\"");

        result.Events
            .Should()
            .NotContainErrors();

        result = await kernel.SubmitCodeAsync(@"
#!mydb
SELECT SUM(deliciousness) FROM fruit GROUP BY color
");

        result.Events.Should().NotContainErrors();

        result.Events.Should()
            .ContainSingle<DisplayedValueProduced>()
            .Which
            .FormattedValues
            .Should()
            .ContainSingle(f => f.MimeType == HtmlFormatter.MimeType);
    }

    [Fact]
    public async Task It_can_connect_and_query_data_with_list_type()
    {
        using var kernel = new CompositeKernel
        {
            new CSharpKernel(),
            new KeyValueStoreKernel()
        };

        kernel.AddKernelConnector(new ConnectDuckDBCommand());

        using var _ = CreateInMemoryDuckDB(out var connectionString);

        var result = await kernel.SubmitCodeAsync(
            $"#!connect duckdb --kernel-name mydb  \"{connectionString}\"");

        result.Events
            .Should()
            .NotContainErrors();

        result = await kernel.SubmitCodeAsync(@"
#!mydb
SELECT numbers FROM fruit 
");

        result.Events.Should().NotContainErrors();

        result.Events.Should()
            .ContainSingle<DisplayedValueProduced>()
            .Which
            .FormattedValues
            .Should()
            .ContainSingle(f => f.MimeType == HtmlFormatter.MimeType);
    }

    [Fact]
    public async Task It_can_handle_multiple_statements()
    {
        using var kernel = new CompositeKernel
        {
            new CSharpKernel(),
            new KeyValueStoreKernel()
        };

        kernel.AddKernelConnector(new ConnectDuckDBCommand());

        using var _ = CreateInMemoryDuckDB(out var connectionString);

        var result = await kernel.SubmitCodeAsync(
            $"#!connect duckdb --kernel-name mydb  \"{connectionString}\"");

        result.Events
            .Should()
            .NotContainErrors();

        result = await kernel.SubmitCodeAsync(@"
#!mydb
SELECT SUM(deliciousness) FROM fruit GROUP BY color;

SELECT SUM(deliciousness) FROM fruit GROUP BY name;
");

        result.Events.Should().NotContainErrors();

        result.Events.OfType<DisplayedValueProduced>().Should().HaveCount(2);
    }

    [Fact]
    public async Task It_can_store_result_set_with_a_name()
    {
        using var kernel = new CompositeKernel
        {
            new CSharpKernel(),
            new KeyValueStoreKernel()
        };

        kernel.AddKernelConnector(new ConnectDuckDBCommand());

        using var _ = CreateInMemoryDuckDB(out var connectionString);

        var result = await kernel.SubmitCodeAsync(
            $"#!connect duckdb --kernel-name mydb  \"{connectionString}\"");

        result.Events
            .Should()
            .NotContainErrors();

        result = await kernel.SubmitCodeAsync(@"
#!mydb --name my_data_result
SELECT SUM(deliciousness) FROM fruit GROUP BY color
");

        var duckDbKernel = kernel.FindKernelByName("mydb");

        result = await duckDbKernel.SendAsync(new RequestValue("my_data_result"));

        result.Events.Should().ContainSingle<ValueProduced>()
            .Which.Value.Should().BeAssignableTo<IEnumerable<TabularDataResource>>();
    }

    internal static IDisposable CreateInMemoryDuckDB(out string connectionString)
    {
        var dbFileName = Path.GetTempFileName();
        connectionString = $"Data Source={dbFileName}.db";
        var topLevelConnection = new DuckDBConnection(connectionString);
        topLevelConnection.Open();
        
        var createCommand = topLevelConnection.CreateCommand();
        createCommand.CommandText =
            @"
CREATE TABLE fruit (
    name TEXT,
    color TEXT,
    deliciousness INTEGER,
    numbers FLOAT[]
);
            ";
        createCommand.ExecuteNonQuery();

        using var connection = new DuckDBConnection(connectionString);

        connection.Open();

        var updateCommand = connection.CreateCommand();
        updateCommand.CommandText =
            @"INSERT INTO fruit VALUES ('apple', 'green', 10, [1,2.3]), ('banana', 'yellow', 11, [1,2.3]), ('banana', 'green', 11, [1,2.3])";
        updateCommand.ExecuteNonQuery();



        return topLevelConnection;
    }
}