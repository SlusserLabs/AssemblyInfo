using SlusserLabs.AssemblyInfo.Infrastructure;

namespace SlusserLabs.AssemblyInfo;

internal record struct AssemblyInfoTarget
{
    public string? NamespaceName { get; set; }

    public EquatableArray<string> TypeDeclarations { get; set; }

    public string DisplayName { get; set; }

    public string HintName { get; set; }

    public GenerateAssemblyInfoOptions Options { get; set; }

    public EquatableArray<StringPair> Diagnostics { get; set; }
}
