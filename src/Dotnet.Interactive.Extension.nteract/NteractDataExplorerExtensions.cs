using System;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting.TabularData;

namespace Dotnet.Interactive.Extension.nteract;

public static class NteractDataExplorerExtensions
{
    public static T UseNteractDataExplorer<T>(this T kernel, Uri libraryUri = null, string libraryVersion = null, string cacheBuster = null) where T : Kernel
    {
        NteractDataExplorer.RegisterFormatters();
        NteractDataExplorer.SetDefaultConfiguration(libraryUri, libraryVersion, cacheBuster);
        DataExplorer.Register<TabularDataResource, NteractDataExplorer>();
        return kernel;
    }
}