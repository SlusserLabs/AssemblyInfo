using SlusserLabs.AssemblyInfo.Tests.Infrastructure;

namespace SlusserLabs.AssemblyInfo.Tests;

public sealed class AssemblyInfoDataTests
{
    [Test]
    public async Task Create_WithAssemblyAttributes_ReturnsAllValues(CancellationToken cancellationToken = default)
    {
        // Arrange
        const string source = """
            using System.Reflection;

            [assembly: AssemblyConfiguration("Release")]
            [assembly: AssemblyCompany("SlusserLabs")]
            [assembly: AssemblyTitle("AssemblyInfo Tests")]
            [assembly: AssemblyDescription("Test description")]
            [assembly: AssemblyProduct("AssemblyInfo")]
            [assembly: AssemblyCopyright("Copyright 2026")]
            [assembly: AssemblyVersion("1.2.3.4")]
            [assembly: AssemblyInformationalVersion("1.2.3+abcdef")]
            [assembly: AssemblyFileVersion("1.2.3.4")]
            [assembly: AssemblyMetadata("RepositoryUrl", "https://example.test/repository")]
            [assembly: AssemblyMetadata("Commit", "abcdef")]
            """;

        var compilation = GeneratorTestHelper.CreateCompilation(source, cancellationToken);

        // Act
        var result = AssemblyInfoData.Create(compilation);
        var equivalentResult = AssemblyInfoData.Create(GeneratorTestHelper.CreateCompilation(source, cancellationToken));

        // Assert
        await Assert.That(result.Configuration).IsEqualTo("Release");
        await Assert.That(result.Company).IsEqualTo("SlusserLabs");
        await Assert.That(result.Title).IsEqualTo("AssemblyInfo Tests");
        await Assert.That(result.Description).IsEqualTo("Test description");
        await Assert.That(result.Product).IsEqualTo("AssemblyInfo");
        await Assert.That(result.Copyright).IsEqualTo("Copyright 2026");
        await Assert.That(result.Version).IsEqualTo("1.2.3.4");
        await Assert.That(result.InformationalVersion).IsEqualTo("1.2.3+abcdef");
        await Assert.That(result.FileVersion).IsEqualTo("1.2.3.4");
        await Assert.That(result.Metadata.Count).IsEqualTo(2);
        await Assert.That(result.Metadata[0].Key).IsEqualTo("RepositoryUrl");
        await Assert.That(result.Metadata[0].Value).IsEqualTo("https://example.test/repository");
        await Assert.That(result.Metadata[1].Key).IsEqualTo("Commit");
        await Assert.That(result.Metadata[1].Value).IsEqualTo("abcdef");
        await Assert.That(result).IsEqualTo(equivalentResult);
        await Assert.That(result.GetHashCode()).IsEqualTo(equivalentResult.GetHashCode());
    }
}
