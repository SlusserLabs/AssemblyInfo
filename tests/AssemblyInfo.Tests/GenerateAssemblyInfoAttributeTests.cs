namespace SlusserLabs.AssemblyInfo.Tests;

// Not exactly life-changing tests, but okay....
public sealed class GenerateAssemblyInfoAttributeTests
{
    [Test]
    public async Task Ctor_WithoutOptions_DefaultsToAll(CancellationToken cancellationToken = default)
    {
        // Act
        var attribute = new GenerateAssemblyInfoAttribute();

        // Assert
        await Assert.That(attribute.Options).IsEqualTo(GenerateAssemblyInfoOptions.All);
    }

    [Test]
    [Arguments(GenerateAssemblyInfoOptions.AllAssemblyAttributes,
        GenerateAssemblyInfoOptions.AssemblyConfiguration
        | GenerateAssemblyInfoOptions.AssemblyCompany
        | GenerateAssemblyInfoOptions.AssemblyTitle
        | GenerateAssemblyInfoOptions.AssemblyDescription
        | GenerateAssemblyInfoOptions.AssemblyProduct
        | GenerateAssemblyInfoOptions.AssemblyCopyright
        | GenerateAssemblyInfoOptions.AssemblyVersion
        | GenerateAssemblyInfoOptions.AssemblyInformationalVersion
        | GenerateAssemblyInfoOptions.AssemblyFileVersion)]
    [Arguments(GenerateAssemblyInfoOptions.All, GenerateAssemblyInfoOptions.AllAssemblyAttributes | GenerateAssemblyInfoOptions.AssemblyMetadata)]
    public async Task Ctor_WithCompositeOptions_ContainsExpectedFlags(GenerateAssemblyInfoOptions options, GenerateAssemblyInfoOptions expected, CancellationToken cancellationToken = default)
    {
        // Act
        var attribute = new GenerateAssemblyInfoAttribute(options);

        // Assert
        await Assert.That(attribute.Options).IsEqualTo(expected);
    }
}
