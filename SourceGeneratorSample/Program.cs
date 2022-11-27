using SourceGeneratorSample.Generators;

namespace SourceGeneratorSample;

class Program
{
    static void Main()
    {
        IRunner runner = 
            new RunnerStj();
            //new RunnerStjCustom();
            //new RunnerJsonNet();
        runner.Run();

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
            0,1,2,3,4,5,6,7,8,9);
        //*/

        //var json = $"{{ {string.Join(", ", Enumerable.Range(0, 64).Select(i => $"\"V{i}\": {i}"))} }}";
        //var json = $"{{ {string.Join(", ", Enumerable.Range(0, 2480).Select(i => $"\"V{i}\": {i}"))} }}";
        //var json = $"{{ {string.Join(", ", Enumerable.Range(0, 502).Select(i => $"\"V{i}\": {i}"))} }}";
        //var foo = JsonSerializer.Deserialize<Foo>(json);
        //var foo = Newtonsoft.Json.JsonConvert.DeserializeObject<Foo>(json);
        /*var foo = JsonSerializer.Deserialize<Foo>(json, new JsonSerializerOptions
        {
            Converters = { new FooConverterFactory() }
        });*/
        //Console.WriteLine(foo);

        /*
        var json = $"{{ {string.Join(", ", Enumerable.Range(0, 65).Select(i => $"\"V{i}\": {i}"))} }}";
        var bar = JsonSerializer.Deserialize<Bar>(json);
        Console.WriteLine(bar);
        */
    }
}
