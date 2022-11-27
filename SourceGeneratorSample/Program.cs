using System.Text.Json;
using SourceGeneratorSample.Generators;

namespace SourceGeneratorSample;

class Program
{
    static void Main(string[] args)
    {
        /*
        var foo = new Foo(
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,

            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9,
            0,1,2,3,4,5,6,7,8,9);*/

        //var json = $"{{ {string.Join(", ", Enumerable.Range(0, 2480).Select(i => $"\"V{i}\": {i}"))} }}";
        //var json = $"{{ {string.Join(", ", Enumerable.Range(0, 502).Select(i => $"\"V{i}\": {i}"))} }}";
        //var foo = JsonSerializer.Deserialize<Foo>(json);
        //var foo = Newtonsoft.Json.JsonConvert.DeserializeObject<Foo>(json);
        /*var foo = JsonSerializer.Deserialize<Foo>(json, new JsonSerializerOptions
        {
            Converters = { new FooConverterFactory() }
        });*/
        var foo = new Foo(1, 2);
        Console.WriteLine(foo);

        /*
        var json = $"{{ {string.Join(", ", Enumerable.Range(0, 65).Select(i => $"\"V{i}\": {i}"))} }}";
        var bar = JsonSerializer.Deserialize<Bar>(json);
        Console.WriteLine(bar);
        */
    }
}

/*
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PropertyCountAttribute : Attribute
{
    public int Count { get; }
    public PropertyCountAttribute(int count)
    {
        Count = count;
    }
} */

[PropertyCount(2)]
public partial record Foo;
public partial record Bar;