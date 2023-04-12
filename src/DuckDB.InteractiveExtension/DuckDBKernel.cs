using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DuckDB.NET.Data;

using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting.TabularData;

namespace DuckDB.InteractiveExtension;

public class DuckDBKernel : Kernel, 
    IKernelCommandHandler<SubmitCode>

{
    private IEnumerable<IEnumerable<IEnumerable<(string name, object value)>>>? _tables;
    private readonly DuckDBConnection connection;


    public DuckDBKernel(string name, string connectionString) : base(name)
    {
        KernelInfo.LanguageName = "SQL";
        connection = new DuckDBConnection(connectionString);
        RegisterForDisposal(connection);
    }

    public async Task HandleAsync(SubmitCode submitCode, KernelInvocationContext context)
    {
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }
        await using var dbCommand = connection.CreateCommand();

        dbCommand.CommandText = submitCode.Code;

        _tables = Execute(dbCommand);

        foreach (var table in _tables)
        {
            var tabularDataResource = table.ToTabularDataResource();

            var explorer = DataExplorer.CreateDefault(tabularDataResource);
            context.Display(explorer);
        }
    }

    private IEnumerable<IEnumerable<IEnumerable<(string name, object value)>>> Execute(IDbCommand command)
    {
        using var reader = command.ExecuteReader();

        do
        {
            var values = new object[reader.FieldCount];
            var names = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToArray();

            AliasDuplicateColumnNames(names);

            // holds the result of a single statement within the query
            var table = new List<(string, object)[]>();

            while (reader.Read())
            {
                reader.GetValues(values);
                var row = new (string, object)[values.Length];
                for (var i = 0; i < values.Length; i++)
                {
                    row[i] = (names[i], values[i]);
                }

                table.Add(row);
            }

            yield return table;
        } while (reader.NextResult());
    }

    private static void AliasDuplicateColumnNames(IList<string> columnNames)
    {
        var nameCounts = new Dictionary<string, int>(capacity: columnNames.Count);
        for (var i = 0; i < columnNames.Count; i++)
        {
            var columnName = columnNames[i];
            if (nameCounts.TryGetValue(columnName, out var count))
            {
                nameCounts[columnName] = ++count;
                columnNames[i] = columnName + $" ({count})";
            }
            else
            {
                nameCounts[columnName] = 1;
            }
        }
    }
}