using System;
using System.Linq;

using Microsoft.Data.Analysis;

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
                    dataFrame.Columns.Add(new DoubleDataFrameColumn(fieldDescriptor.Name, tabularDataResource.Data.Select(d => (double)d[fieldDescriptor.Name])));
                    break;
                case TableSchemaFieldType.Integer:
                    dataFrame.Columns.Add(new Int64DataFrameColumn(fieldDescriptor.Name, tabularDataResource.Data.Select(d => (long)d[fieldDescriptor.Name])));
                    break;
                case TableSchemaFieldType.Boolean:
                    dataFrame.Columns.Add(new BooleanDataFrameColumn(fieldDescriptor.Name, tabularDataResource.Data.Select(d => (bool)d[fieldDescriptor.Name])));
                    break;
                case TableSchemaFieldType.String:
                    dataFrame.Columns.Add(new StringDataFrameColumn(fieldDescriptor.Name, tabularDataResource.Data.Select(d => (string)d[fieldDescriptor.Name])));
                    break;
                case TableSchemaFieldType.Array:
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
}