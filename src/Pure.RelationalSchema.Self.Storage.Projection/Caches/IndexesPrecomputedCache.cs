using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Pure.Collections.Generic;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection.Caches;

internal sealed record IndexesPrecomputedCache
    : IReadOnlyDictionary<IIndex, IDeterminedHash>
{
    private readonly IReadOnlyDictionary<IIndex, IDeterminedHash> _cache;

    public IndexesPrecomputedCache(
        IEnumerable<IIndex> entities,
        IReadOnlyDictionary<IColumn, IDeterminedHash> columnsCache
    )
        : this(
            new Dictionary<IIndex, IIndex, IDeterminedHash>(
                entities,
                x => x,
                x => new RowHash(new IndexProjection(x, columnsCache)),
                x => new IndexHash(x)
            )
        )
    { }

    private IndexesPrecomputedCache(IReadOnlyDictionary<IIndex, IDeterminedHash> cache)
    {
        _cache = cache;
    }

    public IDeterminedHash this[IIndex key] => _cache[key];

    IDeterminedHash IReadOnlyDictionary<IIndex, IDeterminedHash>.this[IIndex key] =>
        throw new NotImplementedException();

    public IEnumerable<IIndex> Keys => _cache.Keys;

    public IEnumerable<IDeterminedHash> Values => _cache.Values;

    public int Count => _cache.Count;

    IEnumerable<IDeterminedHash> IReadOnlyDictionary<IIndex, IDeterminedHash>.Values =>
        throw new NotImplementedException();

    public bool ContainsKey(IIndex key)
    {
        return _cache.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<IIndex, IDeterminedHash>> GetEnumerator()
    {
        return _cache.GetEnumerator();
    }

    public bool TryGetValue(IIndex key, [MaybeNullWhen(false)] out IDeterminedHash value)
    {
        return _cache.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator<KeyValuePair<IIndex, IDeterminedHash>> IEnumerable<
        KeyValuePair<IIndex, IDeterminedHash>
    >.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
