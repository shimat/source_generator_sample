using System;

namespace SourceGeneratorSample.Generators;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PropertyCountAttribute : Attribute
{
    public int Count { get; }

    public PropertyCountAttribute(int count)
    {
        Count = count;
    }
}