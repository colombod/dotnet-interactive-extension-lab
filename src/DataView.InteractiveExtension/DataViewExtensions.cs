using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.DotNet.Interactive.Formatting.TabularData;

// ReSharper disable CheckNamespace
namespace Microsoft.ML;

public static class DataViewExtensions
{
    private static T? GetValue<T>(ValueGetter<T>? valueGetter)
    {
        T? value = default;
        if (valueGetter is not null) valueGetter(ref value!);
        return value;
    }

    public static TabularDataResource ToTabularDataResource(this IDataView source)
    {
        var fields = source.Schema.ToDictionary(column => column.Name, column => column.Type.RawType);
        var data = new List<List<KeyValuePair<string, object?>>>();

        var cursor = source.GetRowCursor(source.Schema);

        while (cursor.MoveNext())
        {
            var rowObj = new List<KeyValuePair<string, object?>>();

            foreach (var column in source.Schema)
            {
                var type = column.Type.RawType;
                var getGetterMethod = cursor.GetType()
                    .GetMethod(nameof(cursor.GetGetter))
                    ?.MakeGenericMethod(type);

                var valueGetter = getGetterMethod?.Invoke(cursor, new object[] { column });

                object? value = GetValue((dynamic)valueGetter!);

                if (value is ReadOnlyMemory<char>)
                {
                    value = value.ToString();
                }

                rowObj.Add(new KeyValuePair<string, object?>( column.Name, value));
            }

            data.Add(rowObj);
        }

        var schema = new TableSchema();

        foreach (var (fieldName, fieldValue) in fields)
        {
            schema.Fields.Add(new TableSchemaFieldDescriptor(fieldName, fieldValue.ToTableSchemaFieldType()));
        }

        return new TabularDataResource(schema, data);
    }
}