using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace SourceGeneratorSample.Generators;

public static class SemanticHelper
{
    /// <summary>
    /// Returns the full namespace for a symbol.
    /// </summary>
    /// <param name="symbol">The symbol being examined.</param>
    /// <returns></returns>
    public static string FullNamespace(this ISymbol symbol)
    {
        var parts = new Stack<string>();
        INamespaceSymbol? iterator = (symbol as INamespaceSymbol) ?? symbol.ContainingNamespace;
        while (iterator != null)
        {
            if (!string.IsNullOrEmpty(iterator.Name))
                parts.Push(iterator.Name);
            iterator = iterator.ContainingNamespace;
        }
        return string.Join(".", parts);
    }

    /// <summary>
    /// This will return a full name of a type, including the namespace and type arguments.
    /// </summary>
    /// <param name="symbol">The symbol being examined.</param>
    /// <returns></returns>
    //[return: NotNullIfNotNull("symbol")]
    public static string? FullName(this INamedTypeSymbol? symbol)
    {
        if (symbol == null)
            return null;

        var prefix = FullNamespace(symbol);
        var suffix = "";
        if (symbol.Arity > 0)
        {
            suffix = CollectTypeArguments(symbol.TypeArguments);
        }

        if (prefix != "")
            return prefix + "." + symbol.Name + suffix + symbol.NullableToken();
        else
            return symbol.Name + suffix + symbol.NullableToken();
    }

    static string CollectTypeArguments(IReadOnlyList<ITypeSymbol> typeArguments)
    {
        var output = new List<string>();
        for (var i = 0; i < typeArguments.Count; i++)
        {
            switch (typeArguments[i])
            {
                case INamedTypeSymbol nts:
                    output.Add(FullName(nts));
                    break;
                case ITypeParameterSymbol tps:
                    output.Add(tps.Name + tps.NullableToken());
                    break;
                default:
                    throw new NotSupportedException($"Cannot generate type name from type argument {typeArguments[i].GetType().FullName}");
            }
        }
        
        return "<" + string.Join(", ", typeArguments) + ">";
    }

    /// <summary>
    /// This is used to append a `?` to type name.
    /// </summary>
    /// <param name="symbol">The symbol being examined.</param>
    /// <returns></returns>
    static string NullableToken(this ITypeSymbol symbol)
    {
        if (symbol.IsValueType || symbol.NullableAnnotation != NullableAnnotation.Annotated)
            return "";
        return "?";
    }
}