using System.Collections.Generic;
using System.Text.Json;
using Microsoft.DotNet.Interactive.Formatting;
using Microsoft.DotNet.Interactive.Formatting.TabularData;
using SandDance.InteractiveExtension;

// ReSharper disable once CheckNamespace
namespace System;

public static class ExploreExtensionsForSandDance
{
    public static SandDanceDataExplorer ExploreWithSandDance(this TabularDataResource source)
    {
        var explorer = new SandDanceDataExplorer(source);
        return explorer;
    }

    public static SandDanceDataExplorer ExploreWithSandDance(this JsonDocument source)
    {
        return source.ToTabularDataResource().ExploreWithSandDance();
    }
    

    public static SandDanceDataExplorer ExploreWithSandDance(this JsonElement source)
    {
        return source.ToTabularDataResource().ExploreWithSandDance();
    }

    public static SandDanceDataExplorer ExploreWithSandDance<T>(this IEnumerable<T> source)
    {
        return source.ToTabularDataResource().ExploreWithSandDance();
    }
}