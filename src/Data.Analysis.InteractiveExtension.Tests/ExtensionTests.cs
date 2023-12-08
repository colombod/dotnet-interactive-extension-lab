using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.DotNet.Interactive.Formatting.TabularData;
using Xunit;

namespace Data.Analysis.InteractiveExtension.Tests;

public class ExtensionTests 
{
    [Fact]
    public void can_generate_dataframe_from_tabular_data_resource()
    {
        var schema = new TableSchema();
        schema.Fields.Add(new TableSchemaFieldDescriptor("A", TableSchemaFieldType.Integer));
        schema.Fields.Add(new TableSchemaFieldDescriptor("B", TableSchemaFieldType.String));
        schema.Fields.Add(new TableSchemaFieldDescriptor("C", TableSchemaFieldType.Array));
        schema.Fields.Add(new TableSchemaFieldDescriptor("D", TableSchemaFieldType.Array));
        var tabularDataResource = new TabularDataResource(
            schema,
            new List<List<KeyValuePair<string, object?>>>
            {
                new() {new("A", 1), new("B", "2"), new KeyValuePair<string, object?>("C",new [] { 1, 3, 5, 7 }), new KeyValuePair<string, object?>("D",new [] { 1.0, 3.0, 5.0, 7.0 }) },
                new() {new("A", 3), new("B", "4"), new KeyValuePair<string, object?>("C", new[] { 2, 4, 6, 8 }), new KeyValuePair<string, object?>("D", new[] { 2.0, 4.0, 6.0, 8.0 }) }
            });

        var dataFrame = tabularDataResource.ToDataFrame();

        using var _ = new AssertionScope();
        dataFrame.Columns.Count.Should().Be(4);
        dataFrame.Columns[0].Name.Should().Be("A");
        dataFrame.Columns[1].Name.Should().Be("B");
        dataFrame.Columns[2].Name.Should().Be("C");
        dataFrame.Columns[3].Name.Should().Be("D");

        dataFrame.Rows[0].Should().BeEquivalentTo(new object[]{ 1,"2", new[] { 1, 3, 5, 7 }, new[] { 1.0, 3.0, 5.0, 7.0 } });
    }
}