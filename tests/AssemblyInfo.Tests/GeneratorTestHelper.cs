using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SlusserLabs.AssemblyInfo.Tests;

internal static class GeneratorTestHelper
{
    public static async ValueTask VerifyAsync(string source, CancellationToken cancellationToken = default)
    {
        var compilation = CreateCompilation(source, cancellationToken);
        var generator = new AssemblyInfoGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _, cancellationToken);

        var compilationErrors = outputCompilation.GetDiagnostics(cancellationToken).Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        await Assert.That(compilationErrors).IsEmpty();

        await Verifier.Verify(driver).UseDirectory("Snapshots");
    }

    internal static CSharpCompilation CreateCompilation(string source, CancellationToken cancellationToken = default)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, cancellationToken: cancellationToken);
        var trustedPlatformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ??
            throw new InvalidOperationException("Missing platform assemblies.");
        var references = trustedPlatformAssemblies.Split(Path.PathSeparator).Select(static path => MetadataReference.CreateFromFile(path)).ToList();
        references.Add(MetadataReference.CreateFromFile(typeof(GeneratedAssemblyInfoAttribute).Assembly.Location));

        return CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
    }
}
