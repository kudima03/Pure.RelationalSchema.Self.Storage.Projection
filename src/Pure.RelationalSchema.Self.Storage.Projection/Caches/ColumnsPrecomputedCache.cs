using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Pure.Collections.Generic;
using Pure.HashCodes.Abstractions;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection.Caches;

internal sealed record ColumnsPrecomputedCache
    : IReadOnlyDictionary<IColumn, IDeterminedHash>
{
    private readonly IReadOnlyDictionary<IColumn, IDeterminedHash> _cache;

    public ColumnsPrecomputedCache(IEnumerable<IColumn> entities)
        : this(
            new Dictionary<IColumn, IColumn, IDeterminedHash>(
                entities,
                x => x,
                x => new RowHash(new ColumnProjection(x)),
                x => new ColumnHash(x)
            )
        )
    { }

    private ColumnsPrecomputedCache(IReadOnlyDictionary<IColumn, IDeterminedHash> cache)
    {
        _cache = cache;
    }

    public IDeterminedHash this[IColumn key] => _cache[key];

    public IEnumerable<IColumn> Keys => _cache.Keys;

    public IEnumerable<IDeterminedHash> Values => _cache.Values;

    public int Count => _cache.Count;

    public bool ContainsKey(IColumn key)
    {
        return _cache.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<IColumn, IDeterminedHash>> GetEnumerator()
    {
        return _cache.GetEnumerator();
    }

    public bool TryGetValue(IColumn key, [MaybeNullWhen(false)] out IDeterminedHash value)
    {
        return _cache.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
