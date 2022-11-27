using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace SourceGeneratorSample.Generators;

[Generator]
public class ManyPropertiesSourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        Make1(context);
        //Make2(context);
    }

    private static void Make1(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxContextReceiver receiver)
            return;

        var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);
        var namespaceName = mainMethod?.ContainingNamespace.ToDisplayString();

        foreach (var w in receiver.WorkItems)
        {
            var typeName = w.TargetRecord.Name;
            var parameterString = string.Join(", ", Enumerable.Range(0, w.PropertyCount).Select(i => $"int V{i}"));

            var source = $@"// auto-generated
namespace {namespaceName};

public partial record {typeName}({parameterString});
";

            context.AddSource($"{typeName}.generated.cs", source);
        }
    }
    
    private static void Make2(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxContextReceiver receiver)
            return;

        var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);
        var namespaceName = mainMethod?.ContainingNamespace.ToDisplayString();

        foreach (var w in receiver.WorkItems)
        {
            var typeName = w.TargetRecord.Name;

            var source = new StringBuilder()
                .AppendLine($@"// auto-generated")
                .AppendLine($"namespace {namespaceName};")
                .AppendLine()
                .AppendLine($"public partial record {typeName}")
                .AppendLine("{");
            for (var i = 0; i < w.PropertyCount; i++)
            {
                source.AppendLine($"    public required int V{i} {{ get; init; }}");
            }

            source.AppendLine("}");

            context.AddSource($"{typeName}.generated.cs", source.ToString());
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxContextReceiver());
    }
}