namespace SlusserLabs.AssemblyInfo;

internal readonly record struct GeneratorDiagnostic
{
    internal GeneratorDiagnostic(string id, string argument)
    {
        Id = id;
        Argument = argument;
    }

    internal string Id { get; }

    internal string Argument { get; }
}
