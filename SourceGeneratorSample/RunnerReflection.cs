using System.Reflection;
using SourceGeneratorSample.Generators;

namespace SourceGeneratorSample;

public class RunnerReflection : IRunner
{
    //public const int PropertyCount = 100;
    public const int PropertyCount = 2081; 

    public void Run()
    {
        var obj = (FooReflection?)Activator.CreateInstance(
            type: typeof(FooReflection),
            bindingAttr: BindingFlags.CreateInstance,
            binder: null,
            args: Enumerable.Range(0, PropertyCount).Select(x => (object)x).ToArray(),
            culture: null);
            
        Console.WriteLine(obj);
    }
}

[PropertyCount(RunnerReflection.PropertyCount)]
public partial record FooReflection;
