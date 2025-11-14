using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Pure.Collections.Generic;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection.Caches;

internal sealed record TablesPrecomputedCache
    : IReadOnlyDictionary<ITable, IDeterminedHash>
{
    private readonly IReadOnlyDictionary<ITable, IDeterminedHash> _cache;

    public TablesPrecomputedCache(
        IEnumerable<ITable> entities,
        IReadOnlyDictionary<IColumn, IDeterminedHash> columnsCache,
        IReadOnlyDictionary<IIndex, IDeterminedHash> indexCache
    )
        : this(
            new Dictionary<ITable, ITable, IDeterminedHash>(
                entities,
                x => x,
                x => new RowHash(new TableProjection(x, columnsCache, indexCache)),
                x => new TableHash(x)
            )
        )
    { }

    private TablesPrecomputedCache(IReadOnlyDictionary<ITable, IDeterminedHash> cache)
    {
        _cache = cache;
    }

    public IDeterminedHash this[ITable key] => _cache[key];

    public IEnumerable<ITable> Keys => _cache.Keys;

    public IEnumerable<IDeterminedHash> Values => _cache.Values;

    public int Count => _cache.Count;

    public bool ContainsKey(ITable key)
    {
        return _cache.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<ITable, IDeterminedHash>> GetEnumerator()
    {
        return _cache.GetEnumerator();
    }

    public bool TryGetValue(ITable key, [MaybeNullWhen(false)] out IDeterminedHash value)
    {
        return _cache.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
