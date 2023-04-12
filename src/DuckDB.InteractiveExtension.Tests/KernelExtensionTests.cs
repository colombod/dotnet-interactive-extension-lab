using FluentAssertions;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting;
using TestUtilities;
using Xunit;
using DuckDB.NET.Data;

namespace DuckDB.InteractiveExtension.Tests;

public class DuckDBConnectionTests
{

    [Fact]
    public async Task It_can_connect_and_query_data()
    {
        using var kernel = new CompositeKernel
        {
            new CSharpKernel().UseNugetDirective(),
            new KeyValueStoreKernel()
        };

        kernel.AddKernelConnector(new ConnectDuckDBCommand());

        using var _ = CreateInMemorySQLiteDb(out var connectionString);

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

    internal static IDisposable CreateInMemorySQLiteDb(out string connectionString)
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
    deliciousness INTEGER
);
            ";
        createCommand.ExecuteNonQuery();

        using var connection = new DuckDBConnection(connectionString);

        connection.Open();

        var updateCommand = connection.CreateCommand();
        updateCommand.CommandText =
            @"INSERT INTO fruit VALUES ('apple', 'green', 10), ('banana', 'yellow', 11), ('banana', 'green', 11)";
        updateCommand.ExecuteNonQuery();



        return topLevelConnection;
    }
}