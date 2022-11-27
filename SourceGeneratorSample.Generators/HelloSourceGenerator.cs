using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGeneratorSample.Generators
{
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
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif 
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver)) 
                return;
            
            var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);
            var namespaceName = mainMethod.ContainingNamespace.ToDisplayString();

            var attributeText = $@"
namespace {namespaceName};

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PropertyCountAttribute : System.Attribute
{{
    public int Count {{ get; }}
    public PropertyCountAttribute(int count)
    {{
        Count = count;
    }}
}}";
            var options = (context.Compilation as CSharpCompilation).SyntaxTrees[0].Options as CSharpParseOptions;
            var compilation = context.Compilation.AddSyntaxTrees(
                CSharpSyntaxTree.ParseText(SourceText.From(attributeText, Encoding.UTF8), options));
            var attributeSymbol = compilation.GetTypeByMetadataName($"{namespaceName}.PropertyCountAttribute");
            
            
            foreach (var candidate in receiver.CandidateTypes)
            {
                var model = compilation.GetSemanticModel(candidate.SyntaxTree);
                var typeSymbol = ModelExtensions.GetDeclaredSymbol(model, candidate);
                if (typeSymbol.GetAttributes().Any(ad =>
                        ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default)))
                {
                    var classSource = ProcessType(typeSymbol, attributeSymbol);
                    context.AddSource(
                        $"{typeSymbol.ContainingNamespace.ToDisplayString()}_{typeSymbol.Name}_value_object",
                        SourceText.From(classSource, Encoding.UTF8));
                }
            }

            /*
            var classWithAttributes  = context.Compilation.SyntaxTrees
                .Where(st => st.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>()
                    .Any(p => p.DescendantNodes().OfType<AttributeSyntax>().Any()))
                .ToArray();

            foreach (SyntaxTree tree in classWithAttributes)
            {
                var semanticModel = context.Compilation.GetSemanticModel(tree);
                var declaredClasses = tree
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<RecordDeclarationSyntax>()
                    .Where(cd => cd.DescendantNodes().OfType<AttributeSyntax>().Any())
                    .ToArray();
                foreach (var declaredClass in declaredClasses)
                {
                    var nodes = declaredClass
                        .DescendantNodes()
                        .OfType<AttributeSyntax>()
                        .FirstOrDefault(a => a.DescendantTokens().Any(dt =>
                        {
                            var x = semanticModel.GetTypeInfo(dt.Parent).Type;
                            return dt.IsKind(SyntaxKind.IdentifierToken) &&
                                   semanticModel.GetTypeInfo(dt.Parent).Type.Name == "PropertyCountAttribute";
                        }))
                        ?.DescendantTokens()
                        .Where(dt => dt.IsKind(SyntaxKind.IdentifierToken))
                        .ToArray();
                    if(nodes is null)
                        continue;
                    
                    var relatedClass = semanticModel.GetTypeInfo(nodes.Last().Parent);
                    GC.KeepAlive(relatedClass);
                }

                GC.KeepAlive(declaredClasses);
            }*/

            
            //context.AddSource($"{typeName}.generated.cs", source);
            throw new NotImplementedException();
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
            // No initialization required for this one
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }
    }
    
    public class SyntaxReceiver : ISyntaxReceiver
    {
        public IList<RecordDeclarationSyntax> CandidateTypes { get; } =
            new List<RecordDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is RecordDeclarationSyntax rds && rds.AttributeLists.Count > 0)
            {
                var syntaxAttributes = rds.AttributeLists.SelectMany(e => e.Attributes)
                    .Where(e => e.Name.NormalizeWhitespace().ToFullString() == "PropertyCount");
                if (syntaxAttributes.Any())
                {
                    CandidateTypes.Add(rds);

                }
            }
        }
    }
}