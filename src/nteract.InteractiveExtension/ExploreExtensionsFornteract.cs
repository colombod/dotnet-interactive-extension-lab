using System.Collections.Generic;
using System.Text.Json;
using Microsoft.DotNet.Interactive.Formatting;
using Microsoft.DotNet.Interactive.Formatting.TabularData;

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
}