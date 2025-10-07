using System.Collections;
using Pure.Collections.Generic;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record ProjectionOnRows<T> : IEnumerable<IRow>
{
    private readonly ITable _table;

    private readonly IEnumerable<T> _entities;

    private readonly Func<IColumn, T, ICell> _cellSwitchFactory;

    public ProjectionOnRows(
        ITable table,
        IEnumerable<T> entities,
        Func<IColumn, T, ICell> cellSwitchFactory
    )
    {
        _table = table;
        _entities = entities;
        _cellSwitchFactory = cellSwitchFactory;
    }

    public IEnumerator<IRow> GetEnumerator()
    {
        return _entities
            .Select(x => new Row(
                new Dictionary<IColumn, IColumn, ICell>(
                    _table.Columns,
                    c => c,
                    c => _cellSwitchFactory(c, x),
                    c => new ColumnHash(c)
                )
            ))
            .GetEnumerator();
    }

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
