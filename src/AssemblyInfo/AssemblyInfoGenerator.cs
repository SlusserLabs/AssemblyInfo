using System.Globalization;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SlusserLabs.AssemblyInfo.Infrastructure;

namespace SlusserLabs.AssemblyInfo;

/// <summary>
/// Generates compile-time accessors for assembly information.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AssemblyInfoGenerator : IIncrementalGenerator
{
    private const string _markerAttributeMetadataName = "SlusserLabs.AssemblyInfo.GenerateAssemblyInfoAttribute";
    private const int _allOptions = (int)GenerateAssemblyInfoOptions.All;

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var targets = context.SyntaxProvider.ForAttributeWithMetadataName(
            _markerAttributeMetadataName,
            predicate: static (node, _) => IsTargetDeclaration(node),
            transform: static (attributeContext, _) => GetTarget(attributeContext));

        var assemblyInfo = context.CompilationProvider.Select(static (compilation, _) => AssemblyInfoData.Create(compilation));
        context.RegisterSourceOutput(targets.Combine(assemblyInfo), static (sourceContext, input) => Generate(sourceContext, input.Left, input.Right));
    }

    private static bool IsTargetDeclaration(SyntaxNode node)
    {
        return node switch
        {
            ClassDeclarationSyntax => true,
            RecordDeclarationSyntax recordDeclaration => !recordDeclaration.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword),
            _ => false,
        };
    }

    private static AssemblyInfoTarget GetTarget(GeneratorAttributeSyntaxContext context)
    {
        var targetDeclaration = (TypeDeclarationSyntax)context.TargetNode;
        var targetSymbol = (INamedTypeSymbol)context.TargetSymbol;
        var options = GetOptions(context.Attributes[0]);
        var declarations = new List<string>();
        var diagnostics = new List<StringPair>();
        // Rebuild the containing type chain so nested targets can be reopened in generated code
        var syntaxDeclarations = targetDeclaration.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().Reverse().ToArray();

        for (var index = 0; index < syntaxDeclarations.Length; index++)
        {
            var declaration = syntaxDeclarations[index];
            var symbol = index == syntaxDeclarations.Length - 1 ? targetSymbol : context.SemanticModel.GetDeclaredSymbol(declaration) as INamedTypeSymbol;

            if (symbol is null || !IsSupportedDeclaration(declaration))
            {
                diagnostics.Add(new StringPair(DiagnosticDescriptors.UnsupportedTargetId, declaration.Identifier.ValueText));
                continue;
            }

            if (declaration.Modifiers.Any(SyntaxKind.FileKeyword))
            {
                diagnostics.Add(new StringPair(DiagnosticDescriptors.UnsupportedTargetId, symbol.Name));
            }

            if (!declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                var id = index == syntaxDeclarations.Length - 1
                    ? DiagnosticDescriptors.TargetMustBePartialId
                    : DiagnosticDescriptors.ContainingTypeMustBePartialId;
                diagnostics.Add(new StringPair(id, symbol.Name));
            }

            declarations.Add(CreateTypeDeclaration(symbol));
        }

        var undefinedOptions = (int)options & ~_allOptions;

        if (undefinedOptions != 0)
        {
            diagnostics.Add(new StringPair(DiagnosticDescriptors.UndefinedOptionsId, undefinedOptions.ToString("X", CultureInfo.InvariantCulture)));
        }

        var namespaceName = targetSymbol.ContainingNamespace.IsGlobalNamespace ? default : targetSymbol.ContainingNamespace.ToDisplayString();
        var typeName = targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return new AssemblyInfoTarget
        {
            NamespaceName = namespaceName,
            TypeDeclarations = new EquatableArray<string>(declarations.ToArray()),
            DisplayName = targetSymbol.ToDisplayString(),
            HintName = CreateHintName(typeName),
            Options = options,
            Diagnostics = new EquatableArray<StringPair>(diagnostics.ToArray())
        };
    }

    private static GenerateAssemblyInfoOptions GetOptions(AttributeData attribute)
    {
        if (attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is int value)
        {
            return (GenerateAssemblyInfoOptions)value;
        }

        return GenerateAssemblyInfoOptions.All;
    }

    private static bool IsSupportedDeclaration(TypeDeclarationSyntax declaration)
    {
        return declaration is ClassDeclarationSyntax ||
            declaration is RecordDeclarationSyntax record && !record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword);
    }

    private static string CreateTypeDeclaration(INamedTypeSymbol symbol)
    {
        var declaration = new StringBuilder();
        declaration.Append(GetAccessibility(symbol.DeclaredAccessibility));
        declaration.Append(' ');

        if (symbol.IsStatic)
        {
            declaration.Append("static ");
        }

        declaration.Append("partial ");
        declaration.Append(symbol.IsRecord ? "record class " : "class ");
        declaration.Append(EscapeIdentifier(symbol.Name));

        if (symbol.TypeParameters.Length > 0)
        {
            declaration.Append('<');

            for (var index = 0; index < symbol.TypeParameters.Length; index++)
            {
                if (index > 0)
                {
                    declaration.Append(", ");
                }

                declaration.Append(EscapeIdentifier(symbol.TypeParameters[index].Name));
            }

            declaration.Append('>');
        }

        return declaration.ToString();
    }

    private static string GetAccessibility(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.Private => "private",
            _ => "internal",
        };
    }

    private static string EscapeIdentifier(string identifier)
    {
        return SyntaxFacts.GetKeywordKind(identifier) == SyntaxKind.None ? identifier : "@" + identifier;
    }

    private static string CreateHintName(string typeName)
    {
        var name = new StringBuilder(typeName.Length);
        // The stable hash keeps otherwise similar sanitized type names from sharing a hint name
        uint hash = 2166136261;

        foreach (var character in typeName)
        {
            name.Append(SyntaxFacts.IsIdentifierPartCharacter(character) ? character : '_');
            hash = (hash ^ character) * 16777619;
        }

        name.Append('_');
        name.Append(hash.ToString("X8", CultureInfo.InvariantCulture));
        name.Append(".g.cs");

        return name.ToString();
    }

    private static void Generate(SourceProductionContext context, AssemblyInfoTarget target, AssemblyInfoData assemblyInfo)
    {
        var hasErrors = false;

        for (var index = 0; index < target.Diagnostics.Count; index++)
        {
            var diagnostic = target.Diagnostics[index];
            var descriptor = GetDescriptor(diagnostic.Key);
            object[] arguments;

            if (diagnostic.Key == DiagnosticDescriptors.UndefinedOptionsId)
            {
                arguments = [target.DisplayName, diagnostic.Value];
            }
            else
            {
                arguments = [diagnostic.Value];
            }

            context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None, arguments));
            hasErrors |= descriptor.DefaultSeverity == DiagnosticSeverity.Error;
        }

        // Metadata lives beside the standard constants, so every standard name stays reserved
        var metadataNames = new HashSet<string>(EmbeddedSources.StandardMemberNames, StringComparer.Ordinal);

        if ((target.Options & GenerateAssemblyInfoOptions.AssemblyMetadata) != 0)
        {
            for (var index = 0; index < assemblyInfo.Metadata.Count; index++)
            {
                var metadata = assemblyInfo.Metadata[index];
                var identifier = EmbeddedSources.CreateIdentifier(metadata.Key);

                if (!metadataNames.Add(identifier))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MetadataNameCollision, Location.None, metadata.Key, identifier));
                    hasErrors = true;
                }
            }
        }

        if (hasErrors)
        {
            return;
        }

        context.AddSource(target.HintName, SourceText.From(EmbeddedSources.Generate(target, assemblyInfo), Encoding.UTF8));
    }

    private static DiagnosticDescriptor GetDescriptor(string id)
    {
        return id switch
        {
            DiagnosticDescriptors.TargetMustBePartialId => DiagnosticDescriptors.TargetMustBePartial,
            DiagnosticDescriptors.ContainingTypeMustBePartialId => DiagnosticDescriptors.ContainingTypeMustBePartial,
            DiagnosticDescriptors.UnsupportedTargetId => DiagnosticDescriptors.UnsupportedTarget,
            _ => DiagnosticDescriptors.UndefinedOptions,
        };
    }
}
