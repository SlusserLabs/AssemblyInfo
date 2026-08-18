using System.Runtime.CompilerServices;

namespace SlusserLabs.AssemblyInfo.Infrastructure;

// NOTE: This follows the common practice to use a custom type in place of ImmutableArray for item equality support.
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

    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _items![index];
    }

    public bool Equals(EquatableArray<T> other)
    {
        // The bread and butter of why we have this type
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
            if (_items is null)
            {
                return 0;
            }

            // Mix each item with a prime before XOR so ordered values produce stable hashes
            var hashCode = 17;

            foreach (var item in _items)
            {
                hashCode = (hashCode * 397) ^ item.GetHashCode();
            }

            return hashCode;
        }
    }
}
