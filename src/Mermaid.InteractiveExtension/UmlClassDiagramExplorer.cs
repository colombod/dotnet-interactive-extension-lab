using System;

namespace Mermaid.InteractiveExtension;

public class UmlClassDiagramExplorer
{
    private readonly Type _type;
    private ClassDiagramConfiguration? _classDiagramConfiguration;

    public UmlClassDiagramExplorer(Type type, ClassDiagramConfiguration? classDiagramConfiguration)
    {
        _type = type;
        _classDiagramConfiguration = classDiagramConfiguration;
    }

    public MermaidMarkdown ToMarkdown()
    {
        return _type.ToClassDiagram(_classDiagramConfiguration);
    }

    public UmlClassDiagramExplorer WithGraphDepth(int graphDepth)
    {
        if (_classDiagramConfiguration is null)
        {
            _classDiagramConfiguration = new ClassDiagramConfiguration();
        }
        _classDiagramConfiguration = _classDiagramConfiguration  with { GraphDepth = graphDepth };
        return this;
    }
}