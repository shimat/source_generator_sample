using SourceGeneratorSample.Generators;
using System.Text.Json;

namespace SourceGeneratorSample;

public class RunnerStj : IRunner
{
    //public const int PropertyCount = 64; // OK
    public const int PropertyCount = 65; // System.NotSupportedException

    public void Run()
    {
        var json = $"{{ {string.Join(", ", Enumerable.Range(0, PropertyCount).Select(i => $"\"V{i}\": {i}"))} }}";
        var obj = JsonSerializer.Deserialize<FooStj>(json);
        Console.WriteLine(obj);
    }
}

[PropertyCount(RunnerStj.PropertyCount)]
public partial record FooStj;

/*
 * System.NotSupportedException: 'The deserialization constructor on type 'SourceGeneratorSample.FooStj' may not have more than 64 parameters for deserialization. Path: $ | LineNumber: 0 | BytePositionInLine: 697.'
 */