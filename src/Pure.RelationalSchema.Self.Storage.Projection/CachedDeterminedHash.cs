using System.Collections;
using Pure.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record CachedDeterminedHash : IDeterminedHash
{
    private readonly IReadOnlyCollection<byte> _hash;

    public CachedDeterminedHash(IDeterminedHash hash)
    {
        _hash = [.. hash];
    }

    public IEnumerator<byte> GetEnumerator()
    {
        return _hash.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
