using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Data.Analysis;
using Microsoft.ML.Data;

// ReSharper disable CheckNamespace
namespace Microsoft.DotNet.Interactive.Formatting.TabularData;

public static  class TabularDataExtensionsForTabularData
{
    public static DataFrame ToDataFrame(this TabularDataResource tabularDataResource)
    {
        var dataFrame = new DataFrame();
        if (tabularDataResource == null)
        {
            throw new ArgumentNullException(nameof(tabularDataResource));
        }

        foreach (var fieldDescriptor in tabularDataResource.Schema.Fields)
        {
            switch (fieldDescriptor.Type)
            {
                case TableSchemaFieldType.Any:
                    break;
                case TableSchemaFieldType.Object:
                    break;
                case TableSchemaFieldType.Null:
                    break;
                case TableSchemaFieldType.Number:
                    dataFrame.Columns.Add(new DoubleDataFrameColumn(fieldDescriptor.Name, tabularDataResource.Data.Select(d => Convert.ToDouble( d.First(v => v.Key ==fieldDescriptor.Name).Value))));
                    break;
                case TableSchemaFieldType.Integer:
                    dataFrame.Columns.Add(new Int64DataFrameColumn(fieldDescriptor.Name, tabularDataResource.Data.Select(d => Convert.ToInt64( d.First(v => v.Key == fieldDescriptor.Name).Value))));
                    break;
                case TableSchemaFieldType.Boolean:
                    dataFrame.Columns.Add(new BooleanDataFrameColumn(fieldDescriptor.Name, tabularDataResource.Data.Select(d => Convert.ToBoolean( d.First(v => v.Key == fieldDescriptor.Name).Value))));
                    break;
                case TableSchemaFieldType.String:
                    dataFrame.Columns.Add(new StringDataFrameColumn(fieldDescriptor.Name, tabularDataResource.Data.Select(d =>Convert.ToString( d.First(v => v.Key == fieldDescriptor.Name).Value))));
                    break;
                case TableSchemaFieldType.Array:
                    var column = GenerateVBufferColumn(fieldDescriptor, tabularDataResource);
                    dataFrame.Columns.Add(column);
                    break;
                case TableSchemaFieldType.DateTime:
                    break;
                case TableSchemaFieldType.GeoPoint:
                    break;
                case TableSchemaFieldType.GeoJson:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return dataFrame;
    }

    private static DataFrameColumn GenerateVBufferColumn(TableSchemaFieldDescriptor fieldDescriptor,
        TabularDataResource tabularDataResource)
    {
        var sourceData = tabularDataResource.Data.Select(d => d.First(v => v.Key == fieldDescriptor.Name).Value);

        var f = sourceData.First(d => d is not null);
        var elementType = f!.GetType().GetElementType();

        if (elementType == typeof(int))
        {
            var intSource = sourceData.Cast<IEnumerable<int>>();
            var vb = intSource.Select(d =>
            {
                var data = d.ToArray();
                return new VBuffer<int>(data.Length, data);
            });
            return new VBufferDataFrameColumn<int>(fieldDescriptor.Name, vb);
        }

        if (elementType == typeof(double))
        {
            var doubleSource = sourceData.Cast<IEnumerable<double>>();
            var vb = doubleSource.Select(d =>
            {
                var data = d.ToArray();
                return new VBuffer<double>(data.Length, data);
            });
            return new VBufferDataFrameColumn<double>(fieldDescriptor.Name, vb);
        }

        throw new ArgumentException($"type {elementType} not supported ");
    }
}