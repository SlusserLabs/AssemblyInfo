using SlusserLabs.AssemblyInfo.Tests.Infrastructure;

namespace SlusserLabs.AssemblyInfo.Tests;

public sealed class AssemblyInfoGeneratorTests
{
    [Test]
    public async Task Initialize_WithAllOptions_GeneratesSnapshot(CancellationToken cancellationToken = default)
    {
        // Arrange
        const string source = """
            using System.Reflection;
            using SlusserLabs.AssemblyInfo;

            [assembly: AssemblyConfiguration("Release")]
            [assembly: AssemblyCompany("SlusserLabs")]
            [assembly: AssemblyTitle("AssemblyInfo Tests")]
            [assembly: AssemblyDescription("Snapshot tests")]
            [assembly: AssemblyProduct("AssemblyInfo")]
            [assembly: AssemblyCopyright("SlusserLabs, Jacob Slusser")]
            [assembly: AssemblyVersion("1.2.3.4")]
            [assembly: AssemblyInformationalVersion("1.2.3+abcdef")]
            [assembly: AssemblyFileVersion("1.2.3.4")]
            [assembly: AssemblyMetadata("BuildDate", "2026-08-17")]
            [assembly: AssemblyMetadata("Repository-Url", "https://example.test/repository")]

            namespace Example;

            [GenerateAssemblyInfo]
            public static partial class ThisAssembly;

            [GenerateAssemblyInfo]
            public partial record AssemblyRecord(string Value);

            public partial class Outer<T> where T : class
            {
                [GenerateAssemblyInfo]
                public partial class Inner<TValue>;
            }
            """;

        // Act & Assert
        await GeneratorTestHelper.VerifyAsync(source, cancellationToken);
    }

    [Test]
    public async Task Initialize_WithOptionSelections_GeneratesSnapshot(CancellationToken cancellationToken = default)
    {
        // Arrange
        const string source = """
            using System.Reflection;
            using SlusserLabs.AssemblyInfo;

            [assembly: AssemblyCompany("SlusserLabs")]
            [assembly: AssemblyTitle("AssemblyInfo Tests")]
            [assembly: AssemblyVersion("1.2.3.4")]
            [assembly: AssemblyMetadata("BuildDate", "2026-08-17")]
            [assembly: AssemblyMetadata("Commit", "abcdef")]

            [GenerateAssemblyInfo(GenerateAssemblyInfoOptions.AssemblyCompany | GenerateAssemblyInfoOptions.AssemblyVersion)]
            public partial class Selected;

            [GenerateAssemblyInfo(GenerateAssemblyInfoOptions.AllAssemblyAttributes)]
            public partial class AssemblyAttributesOnly;

            [GenerateAssemblyInfo(GenerateAssemblyInfoOptions.AssemblyMetadata)]
            public partial class MetadataOnly;

            [GenerateAssemblyInfo(GenerateAssemblyInfoOptions.None)]
            public partial class Empty;
            """;

        // Act & Assert
        await GeneratorTestHelper.VerifyAsync(source, cancellationToken);
    }

    [Test]
    public async Task Initialize_WithMissingSelectedAttributes_GeneratesNullConstantsSnapshot(CancellationToken cancellationToken = default)
    {
        // Arrange
        const string source = """
            using SlusserLabs.AssemblyInfo;

            [GenerateAssemblyInfo(GenerateAssemblyInfoOptions.AssemblyTitle | GenerateAssemblyInfoOptions.AssemblyDescription)]
            public partial class MissingValues;
            """;

        // Act & Assert
        await GeneratorTestHelper.VerifyAsync(source, cancellationToken);
    }

    [Test]
    public async Task Initialize_WithInvalidOptions_ReportsDiagnosticsSnapshot(CancellationToken cancellationToken = default)
    {
        // Arrange
        const string source = """
            using SlusserLabs.AssemblyInfo;

            [GenerateAssemblyInfo]
            public class NotPartial;

            public class Outer
            {
                [GenerateAssemblyInfo]
                public partial class Nested;
            }

            [GenerateAssemblyInfo]
            file partial class FileLocal;

            [GenerateAssemblyInfo((GenerateAssemblyInfoOptions)1024)]
            public partial class UnknownOptions;
            """;

        // Act & Assert
        await GeneratorTestHelper.VerifyAsync(source, cancellationToken);
    }

    [Test]
    public async Task Initialize_WithMetadataCollisions_ReportsDiagnosticsSnapshot(CancellationToken cancellationToken = default)
    {
        // Arrange
        const string source = """
            using System.Reflection;
            using SlusserLabs.AssemblyInfo;

            [assembly: AssemblyMetadata("Company", "reserved")]
            [assembly: AssemblyMetadata("Repository-Url", "first")]
            [assembly: AssemblyMetadata("Repository.Url", "second")]

            [GenerateAssemblyInfo(GenerateAssemblyInfoOptions.AssemblyMetadata)]
            public partial class Conflicts;
            """;

        // Act & Assert
        await GeneratorTestHelper.VerifyAsync(source, cancellationToken);
    }
}
