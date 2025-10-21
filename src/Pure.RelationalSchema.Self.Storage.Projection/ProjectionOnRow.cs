using Pure.Collections.Generic;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record ProjectionOnRow<T> : IRow
{
    private readonly IEnumerable<IColumn> _columns;

    private readonly T _entity;

    private readonly Func<IColumn, T, ICell> _cellSwitchFactory;

    public ProjectionOnRow(
        IEnumerable<IColumn> columns,
        T entity,
        Func<IColumn, T, ICell> cellSwitchFactory
    )
    {
        _columns = columns;
        _entity = entity;
        _cellSwitchFactory = cellSwitchFactory;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells =>
        new Dictionary<IColumn, IColumn, ICell>(
            _columns,
            x => x,
            x => _cellSwitchFactory(x, _entity),
            x => new ColumnHash(x)
        );

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }
}
