namespace SlusserLabs.AssemblyInfo.Infrastructure;

// This follows the suggestion by Andrew Lock to use a custom type in place of ImmutableArray for item equality support.
// See: https://andrewlock.net/creating-a-source-generator-part-9-avoiding-performance-pitfalls-in-incremental-generators/
// See: https://github.com/CommunityToolkit/dotnet/blob/main/src/CommunityToolkit.Mvvm.SourceGenerators/Helpers/EquatableArray%7BT%7D.cs
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>> where T : IEquatable<T>
{
    private readonly T[]? _items;

    public EquatableArray(T[] items)
    {
        _items = items;
    }

    public int Count => _items?.Length ?? 0;

    public T this[int index] => _items![index];

    public bool Equals(EquatableArray<T> other)
    {
        // Sequence equality lets Roslyn reuse downstream work when the contents have not changed
        return _items.AsSpan().SequenceEqual(other._items.AsSpan());
    }

    public override bool Equals(object? obj)
    {
        return obj is EquatableArray<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            // Mix each item with a prime before XOR so ordered values produce stable hashes
            var hashCode = 17;

            if (_items is null)
            {
                return 0;
            }

            foreach (var item in _items)
            {
                hashCode = (hashCode * 397) ^ item.GetHashCode();
            }

            return hashCode;
        }
    }
}
