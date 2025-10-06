using Pure.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record EqualityComparerByDeterminedHash<T> : IEqualityComparer<T>
{
    private readonly Func<T, IDeterminedHash> _determinedHashFactory;

    public EqualityComparerByDeterminedHash(
        Func<T, IDeterminedHash> determinedHashFactory
    )
    {
        _determinedHashFactory = determinedHashFactory;
    }

    public bool Equals(T? x, T? y)
    {
        return _determinedHashFactory(x!).SequenceEqual(_determinedHashFactory(y!));
    }

    public int GetHashCode(T obj)
    {
        HashCode hash = new();
        hash.AddBytes(_determinedHashFactory(obj).ToArray());
        return hash.ToHashCode();
    }

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }
}
