using Pure.Collections.Generic;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection;

internal sealed record ForeignKeyProjection : IRow
{
    private readonly IForeignKey _entity;

    private readonly IEnumerable<IColumn> _columns;

    private readonly IReadOnlyDictionary<ITable, IDeterminedHash> _tablesCache;

    private readonly IReadOnlyDictionary<IColumn, IDeterminedHash> _columnsCache;

    public ForeignKeyProjection(
        IForeignKey entity,
        IReadOnlyDictionary<IColumn, IDeterminedHash> columnsCache,
        IReadOnlyDictionary<ITable, IDeterminedHash> tablesCache
    )
        : this(entity, new ForeignKeysTable().Columns, columnsCache, tablesCache) { }

    public ForeignKeyProjection(
        IForeignKey entity,
        IEnumerable<IColumn> columns,
        IReadOnlyDictionary<IColumn, IDeterminedHash> columnsCache,
        IReadOnlyDictionary<ITable, IDeterminedHash> tablesCache
    )
    {
        _entity = entity;
        _columns = columns;
        _tablesCache = tablesCache;
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
                        new ReferencingTableColumn(),
                        new Cell(new HexString(_tablesCache[_entity.ReferencingTable]))
                    ),
                    new KeyValuePair<IColumn, ICell>(
                        new ReferencedTableColumn(),
                        new Cell(new HexString(_tablesCache[_entity.ReferencedTable]))
                    ),
                    new KeyValuePair<IColumn, ICell>(
                        new CompositionHashColumn(),
                        new Cell(
                            new HexString(
                                new DeterminedHash(
                                    [
                                        new DeterminedHash(
                                            _entity.ReferencingColumns.Select(x =>
                                                _columnsCache[x]
                                            )
                                        ),
                                        new DeterminedHash(
                                            _entity.ReferencedColumns.Select(x =>
                                                _columnsCache[x]
                                            )
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
