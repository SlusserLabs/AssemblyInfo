namespace SlusserLabs.AssemblyInfo.Tests;

public sealed class AssemblyInfoGeneratorTests
{
    [Test]
    public async Task Initialize_WithAllAttributesAndTargetShapes_GeneratesSnapshotAsync(CancellationToken cancellationToken = default)
    {
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

            [GeneratedAssemblyInfo]
            public static partial class ThisAssembly;

            [GeneratedAssemblyInfo]
            public partial record AssemblyRecord(string Value);

            public partial class Outer<T>
                where T : class
            {
                [GeneratedAssemblyInfo]
                public partial class Inner<TValue>;
            }
            """;

        await GeneratorTestHelper.VerifyAsync(source, cancellationToken);
    }

    [Test]
    public async Task Initialize_WithOptionSelections_GeneratesSnapshotAsync(CancellationToken cancellationToken = default)
    {
        const string source = """
            using System.Reflection;
            using SlusserLabs.AssemblyInfo;

            [assembly: AssemblyCompany("SlusserLabs")]
            [assembly: AssemblyTitle("AssemblyInfo Tests")]
            [assembly: AssemblyVersion("1.2.3.4")]
            [assembly: AssemblyMetadata("BuildDate", "2026-08-17")]
            [assembly: AssemblyMetadata("Commit", "abcdef")]

            [GeneratedAssemblyInfo(AssemblyInfoOptions.AssemblyCompany | AssemblyInfoOptions.AssemblyVersion)]
            public partial class Selected;

            [GeneratedAssemblyInfo(AssemblyInfoOptions.AllAssemblyAttributes)]
            public partial class AssemblyAttributesOnly;

            [GeneratedAssemblyInfo(AssemblyInfoOptions.AssemblyMetadata)]
            public partial class MetadataOnly;

            [GeneratedAssemblyInfo(AssemblyInfoOptions.None)]
            public partial class Empty;
            """;

        await GeneratorTestHelper.VerifyAsync(source, cancellationToken);
    }

    [Test]
    public async Task Initialize_WithMissingSelectedAttributes_GeneratesNullConstantsSnapshotAsync(CancellationToken cancellationToken = default)
    {
        const string source = """
            using SlusserLabs.AssemblyInfo;

            [GeneratedAssemblyInfo(AssemblyInfoOptions.AssemblyTitle | AssemblyInfoOptions.AssemblyDescription)]
            public partial class MissingValues;
            """;

        await GeneratorTestHelper.VerifyAsync(source, cancellationToken);
    }

    [Test]
    public async Task Initialize_WithInvalidTargetsAndOptions_ReportsDiagnosticsSnapshotAsync(CancellationToken cancellationToken = default)
    {
        const string source = """
            using SlusserLabs.AssemblyInfo;

            [GeneratedAssemblyInfo]
            public class NotPartial;

            public class Outer
            {
                [GeneratedAssemblyInfo]
                public partial class Nested;
            }

            [GeneratedAssemblyInfo]
            file partial class FileLocal;

            [GeneratedAssemblyInfo((AssemblyInfoOptions)1024)]
            public partial class UnknownOptions;
            """;

        await GeneratorTestHelper.VerifyAsync(source, cancellationToken);
    }

    [Test]
    public async Task Initialize_WithMetadataCollisions_ReportsDiagnosticsSnapshotAsync(CancellationToken cancellationToken = default)
    {
        const string source = """
            using System.Reflection;
            using SlusserLabs.AssemblyInfo;

            [assembly: AssemblyMetadata("Company", "reserved")]
            [assembly: AssemblyMetadata("Repository-Url", "first")]
            [assembly: AssemblyMetadata("Repository.Url", "second")]

            [GeneratedAssemblyInfo(AssemblyInfoOptions.AssemblyMetadata)]
            public partial class Conflicts;
            """;

        await GeneratorTestHelper.VerifyAsync(source, cancellationToken);
    }
}
