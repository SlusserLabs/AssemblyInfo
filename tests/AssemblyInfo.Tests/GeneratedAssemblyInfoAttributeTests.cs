namespace SlusserLabs.AssemblyInfo.Tests;

public sealed class GeneratedAssemblyInfoAttributeTests
{
    [Test]
    public async Task Ctor_WithoutOptions_DefaultsToAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var attribute = new GeneratedAssemblyInfoAttribute();

        await Assert.That(attribute.Options).IsEqualTo(AssemblyInfoOptions.All);
    }

    [Test]
    [Arguments(
        AssemblyInfoOptions.AllAssemblyAttributes,
        AssemblyInfoOptions.AssemblyConfiguration |
        AssemblyInfoOptions.AssemblyCompany |
        AssemblyInfoOptions.AssemblyTitle |
        AssemblyInfoOptions.AssemblyDescription |
        AssemblyInfoOptions.AssemblyProduct |
        AssemblyInfoOptions.AssemblyCopyright |
        AssemblyInfoOptions.AssemblyVersion |
        AssemblyInfoOptions.AssemblyInformationalVersion |
        AssemblyInfoOptions.AssemblyFileVersion)]
    [Arguments(AssemblyInfoOptions.All, AssemblyInfoOptions.AllAssemblyAttributes | AssemblyInfoOptions.AssemblyMetadata)]
    public async Task Ctor_WithCompositeOptions_ContainsExpectedFlagsAsync(
        AssemblyInfoOptions options,
        AssemblyInfoOptions expected,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var attribute = new GeneratedAssemblyInfoAttribute(options);

        await Assert.That(attribute.Options).IsEqualTo(expected);
    }
}
