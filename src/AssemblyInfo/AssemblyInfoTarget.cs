namespace SlusserLabs.AssemblyInfo;

internal readonly record struct AssemblyInfoTarget
{
    internal AssemblyInfoTarget(
        string? namespaceName,
        EquatableArray<string> typeDeclarations,
        string displayName,
        string hintName,
        AssemblyInfoOptions options,
        EquatableArray<GeneratorDiagnostic> diagnostics)
    {
        NamespaceName = namespaceName;
        TypeDeclarations = typeDeclarations;
        DisplayName = displayName;
        HintName = hintName;
        Options = options;
        Diagnostics = diagnostics;
    }

    internal string? NamespaceName { get; }

    internal EquatableArray<string> TypeDeclarations { get; }

    internal string DisplayName { get; }

    internal string HintName { get; }

    internal AssemblyInfoOptions Options { get; }

    internal EquatableArray<GeneratorDiagnostic> Diagnostics { get; }
}
