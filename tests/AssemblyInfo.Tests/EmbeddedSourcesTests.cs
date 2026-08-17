namespace SlusserLabs.AssemblyInfo.Tests;

public sealed class EmbeddedSourcesTests
{
    [Test]
    [Arguments("Repository-Url", "Repository_Url")]
    [Arguments("class", "@class")]
    [Arguments("1st", "_1st")]
    [Arguments("", "_")]
    public async Task CreateIdentifier_WithMetadataKey_ReturnsValidIdentifierAsync(string key, string expected, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = EmbeddedSources.CreateIdentifier(key);

        await Assert.That(result).IsEqualTo(expected);
    }
}
