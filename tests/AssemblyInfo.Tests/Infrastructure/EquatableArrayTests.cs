using SlusserLabs.AssemblyInfo.Infrastructure;

namespace SlusserLabs.AssemblyInfo.Tests.Infrastructure;

public sealed class EquatableArrayTests
{
    [Test]
    public async Task Equals_WithEqualItems_ReturnsTrue()
    {
        // Arrange
        var first = new EquatableArray<string>(["one", "two"]);
        var second = new EquatableArray<string>(["one", "two"]);
        object equalItems = second;

        // Act
        var result = first.Equals(second);
        var objectResult = first.Equals(equalItems);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(objectResult).IsTrue();
        await Assert.That(first.Count).IsEqualTo(2);
    }

    [Test]
    public async Task Equals_WithUnequalItems_ReturnsFalse()
    {
        // Arrange
        var first = new EquatableArray<string>(["one", "two"]);
        var second = new EquatableArray<string>(["one", "three"]);
        object unequalItems = second;

        // Act
        var result = first.Equals(second);
        var objectResult = first.Equals(unequalItems);

        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(objectResult).IsFalse();
    }

    [Test]
    public async Task Equals_WithEmptyArrays_ReturnsTrue()
    {
        // Arrange
        var first = new EquatableArray<string>([]);
        var second = new EquatableArray<string>([]);
        object emptyItems = second;

        // Act
        var result = first.Equals(second);
        var objectResult = first.Equals(emptyItems);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(objectResult).IsTrue();
    }
}
