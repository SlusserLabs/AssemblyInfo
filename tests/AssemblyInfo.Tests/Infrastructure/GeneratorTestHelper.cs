using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SlusserLabs.AssemblyInfo.Tests.Infrastructure;

internal static class GeneratorTestHelper
{
    public static async ValueTask VerifyAsync(string source, CancellationToken cancellationToken = default)
    {
        // Compile the supplied source and run the generator against it
        var compilation = CreateCompilation(source, cancellationToken);
        var generator = new AssemblyInfoGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _, cancellationToken);

        // Assert no errors
        var compilationErrors = outputCompilation.GetDiagnostics(cancellationToken).Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        await Assert.That(compilationErrors).IsEmpty();

        await Verify(driver);
    }

    public static CSharpCompilation CreateCompilation(string source, CancellationToken cancellationToken = default)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, cancellationToken: cancellationToken);

        // Reference standard assemblies.
        // See: https://github.com/dotnet/runtime/blob/main/docs/design/features/host-runtime-information.md
        var trustedPlatformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
        if (string.IsNullOrEmpty(trustedPlatformAssemblies))
        {
            throw new InvalidOperationException("Missing platform assemblies.");
        }

        // Add core and custom dependencies
        var references = trustedPlatformAssemblies.Split(Path.PathSeparator).Select(static path => MetadataReference.CreateFromFile(path)).ToList();
        references.Add(MetadataReference.CreateFromFile(typeof(GenerateAssemblyInfoAttribute).Assembly.Location));

        return CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
    }
}
