using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGeneratorSample.Generators;

internal class SyntaxContextReceiver : ISyntaxContextReceiver
{
    private List<(INamedTypeSymbol TargetRecord, int PropertyCount)> workItems = new();
    public IReadOnlyList<(INamedTypeSymbol TargetRecord, int PropertyCount)> WorkItems => workItems;

    /// <summary>
    /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
    /// </summary>
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is not RecordDeclarationSyntax rds) 
            return;
        var targetRecord = (INamedTypeSymbol?)context.SemanticModel.GetDeclaredSymbol(context.Node);
        if (targetRecord is null)
            return;
        var attributes = targetRecord.GetAttributes();
        var targetAttribute = attributes.FirstOrDefault(att => 
            att.AttributeClass.FullName() == "SourceGeneratorSample.Generators.PropertyCountAttribute");
        if (targetAttribute is null) 
            return;
        if (targetAttribute.ConstructorArguments.Length != 1)
            return;
        var value = targetAttribute.ConstructorArguments[0].Value;
        if (value is null) 
            return;

        workItems.Add((targetRecord, (int)value));
    }
}
