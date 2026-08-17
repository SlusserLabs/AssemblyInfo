namespace SlusserLabs.AssemblyInfo;

internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>
    where T : IEquatable<T>
{
    private readonly T[]? _items;

    internal EquatableArray(T[] items)
    {
        _items = items;
    }

    internal int Count => _items?.Length ?? 0;

    internal T this[int index] => _items![index];

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
