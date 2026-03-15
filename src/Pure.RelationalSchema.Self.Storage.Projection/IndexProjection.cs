using Pure.Collections.Generic;
using Pure.HashCodes.Abstractions;
using Pure.Primitives.Guid;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;
using String = Pure.Primitives.String.String;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record IndexProjection : IRow
{
    private readonly IIndex _entity;

    private readonly IEnumerable<IColumn> _columns;

    private readonly IReadOnlyDictionary<IColumn, IDeterminedHash> _columnsCache;

    public IndexProjection(
        IIndex entity,
        IReadOnlyDictionary<IColumn, IDeterminedHash> columnsCache
    )
        : this(entity, new IndexesTable().Columns, columnsCache) { }

    public IndexProjection(
        IIndex entity,
        IEnumerable<IColumn> columns,
        IReadOnlyDictionary<IColumn, IDeterminedHash> columnsCache
    )
    {
        _entity = entity;
        _columns = columns;
        _columnsCache = columnsCache;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells =>
        new Dictionary<IColumn, IColumn, ICell>(
            _columns,
            column => column,
            column => new CellSwitch<IColumn>(
                column,
                [
                    new KeyValuePair<IColumn, ICell>(
                        new UuidColumn(),
                        new Cell(new String(new Ulid()))
                    ),
                    new KeyValuePair<IColumn, ICell>(
                        new IsUniqueColumn(),
                        new Cell(new String(_entity.IsUnique))
                    ),
                ],
                x => new ColumnHash(x)
            ),
            column => new ColumnHash(column)
        );
}
