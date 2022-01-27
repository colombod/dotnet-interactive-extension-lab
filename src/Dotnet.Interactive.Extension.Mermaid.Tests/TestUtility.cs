using FluentAssertions;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive.Formatting.TabularData;

namespace Dotnet.Interactive.Extension.Mermaid.Tests;

internal static class TestUtility
{
   
    internal static TabularDataResource ShouldDisplayTabularDataResourceWhich(
        this SubscribedList<KernelEvent> events)
    {
        events.Should().NotContainErrors();

        return events
            .Should()
            .ContainSingle<DisplayedValueProduced>(e => e.Value is DataExplorer<TabularDataResource>)
            .Which
            .Value
            .As<DataExplorer<TabularDataResource>>()
            .Data;
    }
}