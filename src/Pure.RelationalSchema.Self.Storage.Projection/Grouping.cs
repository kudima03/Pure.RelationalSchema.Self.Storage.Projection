using System.Collections;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record Grouping : IGrouping<ITable, IRow>
{
    private readonly IEnumerable<IRow> _rows;

    public ITable Key { get; }

    public Grouping(ITable key, IEnumerable<IRow> rows)
    {
        _rows = rows;
        Key = key;
    }

    public IEnumerator<IRow> GetEnumerator()
    {
        return _rows.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
