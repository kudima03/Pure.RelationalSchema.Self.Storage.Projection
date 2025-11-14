using Pure.Collections.Generic;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record TableProjection : IRow
{
    private readonly ITable _entity;

    private readonly IEnumerable<IColumn> _columns;

    private readonly IReadOnlyDictionary<IColumn, IDeterminedHash> _columnsCache;

    private readonly IReadOnlyDictionary<IIndex, IDeterminedHash> _indexCache;

    public TableProjection(
        ITable entity,
        IReadOnlyDictionary<IColumn, IDeterminedHash> columnsCache,
        IReadOnlyDictionary<IIndex, IDeterminedHash> indexCache
    )
        : this(entity, new TablesTable().Columns, columnsCache, indexCache) { }

    public TableProjection(
        ITable entity,
        IEnumerable<IColumn> columns,
        IReadOnlyDictionary<IColumn, IDeterminedHash> columnsCache,
        IReadOnlyDictionary<IIndex, IDeterminedHash> indexCache
    )
    {
        _entity = entity;
        _columns = columns;
        _columnsCache = columnsCache;
        _indexCache = indexCache;
    }

    public IReadOnlyDictionary<IColumn, ICell> Cells =>
        new Dictionary<IColumn, IColumn, ICell>(
            _columns,
            column => column,
            column => new CellSwitch<IColumn>(
                column,
                [
                    new KeyValuePair<IColumn, ICell>(
                        new NameColumn(),
                        new Cell(_entity.Name)
                    ),
                    new KeyValuePair<IColumn, ICell>(
                        new CompositionHashColumn(),
                        new Cell(
                            new HexString(
                                new DeterminedHash(
                                    [
                                        new DeterminedHash(
                                            _entity.Columns.Select(x => _columnsCache[x])
                                        ),
                                        new DeterminedHash(
                                            _entity.Indexes.Select(x => _indexCache[x])
                                        ),
                                    ]
                                )
                            )
                        )
                    ),
                ],
                column => new ColumnHash(column)
            ),
            column => new ColumnHash(column)
        );
}
