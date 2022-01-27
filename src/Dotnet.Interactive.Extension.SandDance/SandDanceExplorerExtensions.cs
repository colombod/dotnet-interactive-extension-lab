using System;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting.TabularData;

namespace Dotnet.Interactive.Extension.SandDance;

public static class SandDanceExplorerExtensions
{
    public static T UseSandDanceExplorer<T>(this T kernel, Uri? libraryUri = null, string? libraryVersion = null,
        string? cacheBuster = null) where T : Kernel
    {
        SandDanceDataExplorer.RegisterFormatters();
        SandDanceDataExplorer.ConfigureDefaults(libraryUri, libraryVersion,
            cacheBuster);

        DataExplorer.Register<TabularDataResource, SandDanceDataExplorer>();
        return kernel;
    }
}