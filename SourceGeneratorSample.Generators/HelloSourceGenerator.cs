using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGeneratorSample.Generators;

[Generator]
public class HelloSourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        MakeFoo(context);
        MakeBar(context);
    }

    private static void MakeFoo(GeneratorExecutionContext context)
    {
        /*
#if DEBUG
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
#endif
        //*/
        //if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
        //    return;
        if (!(context.SyntaxContextReceiver is SyntaxContextReceiver receiver))
            return;

        var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);
        var namespaceName = mainMethod.ContainingNamespace.ToDisplayString();

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

    private static string ProcessType(ISymbol typeSymbol, ISymbol attributeSymbol)
    {
        if (!typeSymbol.ContainingSymbol.Equals(typeSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            return null;
            
        var namespaceName = typeSymbol.ContainingNamespace.ToDisplayString();
        var attributeData = typeSymbol.GetAttributes().Single(ad =>
            ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default));
        var x = attributeData.NamedArguments;
        var overridenNameOpt =
            attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "Count").Value;
        var propertyName = overridenNameOpt.IsNull ? "Hoge" : overridenNameOpt.Value.ToString();

        var parameterCount = 1;
        var parameterString = string.Join(", ", Enumerable.Range(0, parameterCount).Select(i => $"int V{i}"));
        var source = $@"
namespace {namespaceName};

public sealed partial record {typeSymbol.Name}({parameterString});
";
        return source;
    }
        
    private static void MakeFoo_(GeneratorExecutionContext context)
    {
        const string typeName = "Foo";
        const int parameterCount = 2480;

        var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);
            
        var parameterString = string.Join(", ", Enumerable.Range(0, parameterCount).Select(i => $"int V{i}"));
        var source = $@"// auto-generated
namespace {mainMethod.ContainingNamespace.ToDisplayString()};

public partial record {typeName}({parameterString});
";
            
        context.AddSource($"{typeName}.generated.cs", source);
    }

    private static void MakeBar(GeneratorExecutionContext context)
    {
        const string typeName = "Bar";
        const int fieldCount = 65;

        var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);
            
        var source = new StringBuilder()
            .AppendLine($@"// auto-generated")
            .AppendLine($"namespace {mainMethod.ContainingNamespace.ToDisplayString()};")
            .AppendLine()
            .AppendLine($"public partial record {typeName}")
            .AppendLine("{");
        for (int i = 0; i < fieldCount; i++)
        {
            source.AppendLine($"    public required int V{i} {{ get; init; }}");
        }
        source.AppendLine("}");

        context.AddSource($"{typeName}.generated.cs", source.ToString());
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        //context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        context.RegisterForSyntaxNotifications(() => new SyntaxContextReceiver());
    }
}