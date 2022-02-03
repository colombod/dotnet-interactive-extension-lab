using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.DotNet.Interactive.Formatting;
using Microsoft.DotNet.Interactive.Formatting.TabularData;
using Microsoft.ML;
using nteract.InteractiveExtension;


// ReSharper disable once CheckNamespace
namespace System;
public static class ExploreExtensionsFornteract
{
    public static NteractDataExplorer ExploreWithNteract(this TabularDataResource source)
    {
        var explorer = new NteractDataExplorer(source);
        return explorer;
    }

    public static NteractDataExplorer ExploreWithSandDance(this JsonDocument source)
    {
        return source.ToTabularDataResource().ExploreWithNteract();
    }


    public static NteractDataExplorer ExploreWithNteract(this JsonElement source)
    {
        return source.ToTabularDataResource().ExploreWithNteract();
    }

    public static NteractDataExplorer ExploreWithNteract<T>(this IEnumerable<T> source)
    {
        return source.ToTabularDataResource().ExploreWithNteract();
    }

    public static NteractDataExplorer ExploreWithNteract(this IDataView source)
    {
        return source.ToTabularDataResource().ExploreWithNteract();
    }

    private static T? GetValue<T>(ValueGetter<T>? valueGetter)
    {
        T? value = default;
        if (valueGetter is not null) valueGetter(ref value!);
        return value;
    }

    private static TabularDataResource ToTabularDataResource(this IDataView source)
    {
        var fields = source.Schema.ToDictionary(column => column.Name, column => column.Type.RawType);
        var data = new List<Dictionary<string, object?>>();

        var cursor = source.GetRowCursor(source.Schema);

        while (cursor.MoveNext())
        {
            var rowObj = new Dictionary<string, object?>();

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

                rowObj.Add(column.Name, value);
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