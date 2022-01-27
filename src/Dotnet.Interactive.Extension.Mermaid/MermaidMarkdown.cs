using System;

namespace Dotnet.Interactive.Extension.Mermaid;

public class MermaidMarkdown
{
    public override string ToString()
    {
        return _value;
    }

    private readonly string _value;

    public MermaidMarkdown(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }
}