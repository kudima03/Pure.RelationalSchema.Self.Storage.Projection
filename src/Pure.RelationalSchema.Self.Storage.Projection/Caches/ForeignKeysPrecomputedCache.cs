using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Pure.Collections.Generic;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection.Caches;

internal sealed record ForeignKeysPrecomputedCache
    : IReadOnlyDictionary<IForeignKey, IDeterminedHash>
{
    private readonly IReadOnlyDictionary<IForeignKey, IDeterminedHash> _cache;

    public ForeignKeysPrecomputedCache(
        IEnumerable<IForeignKey> entities,
        IReadOnlyDictionary<IColumn, IDeterminedHash> columnsCache,
        IReadOnlyDictionary<ITable, IDeterminedHash> tablesCache
    )
        : this(
            new Dictionary<IForeignKey, IForeignKey, IDeterminedHash>(
                entities,
                x => x,
                x => new RowHash(new ForeignKeyProjection(x, columnsCache, tablesCache)),
                x => new ForeignKeyHash(x)
            )
        )
    { }

    private ForeignKeysPrecomputedCache(
        IReadOnlyDictionary<IForeignKey, IDeterminedHash> cache
    )
    {
        _cache = cache;
    }

    public IDeterminedHash this[IForeignKey key] => _cache[key];

    public IEnumerable<IForeignKey> Keys => _cache.Keys;

    public IEnumerable<IDeterminedHash> Values => _cache.Values;

    public int Count => _cache.Count;

    public bool ContainsKey(IForeignKey key)
    {
        return _cache.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<IForeignKey, IDeterminedHash>> GetEnumerator()
    {
        return _cache.GetEnumerator();
    }

    public bool TryGetValue(
        IForeignKey key,
        [MaybeNullWhen(false)] out IDeterminedHash value
    )
    {
        return _cache.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
