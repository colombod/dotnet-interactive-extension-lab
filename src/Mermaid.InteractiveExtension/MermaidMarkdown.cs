using System;

namespace Mermaid.InteractiveExtension;

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