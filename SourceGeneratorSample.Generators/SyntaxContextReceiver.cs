using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGeneratorSample.Generators;

internal class SyntaxContextReceiver : ISyntaxContextReceiver
{
    public List<WorkItem> WorkItems { get; } = new();

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

        WorkItems.Add(new WorkItem(targetRecord, (int)value));
    }
}

internal class WorkItem
{
    public INamedTypeSymbol TargetRecord { get; }
    public int PropertyCount { get; }

    public WorkItem(INamedTypeSymbol targetRecord, int propertyCount)
    {
        TargetRecord = targetRecord ?? throw new ArgumentNullException(nameof(targetRecord));
        PropertyCount = propertyCount;
    }
}