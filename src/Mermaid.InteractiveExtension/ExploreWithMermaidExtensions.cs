using System;
using Mermaid.InteractiveExtension;

// ReSharper disable once CheckNamespace
namespace System;

public static class ExploreWithMermaidExtensions
{
    public static UmlClassDiagramExplorer ExploreWithUmlClassDiagram(this Type type)
    {
        return new UmlClassDiagramExplorer(type, new ClassDiagramConfiguration(1));
    }
}