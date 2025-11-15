using Pure.Collections.Generic;
using Pure.HashCodes;
using Pure.HashCodes.Abstractions;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Self.Storage.Projection.Caches;
using Pure.RelationalSchema.Storage;
using Pure.RelationalSchema.Storage.Abstractions;

namespace Pure.RelationalSchema.Self.Storage.Projection;

public sealed record SchemaEntityProjection : IRow
{
    private readonly ISchema _entity;

    private readonly IEnumerable<IColumn> _columns;

    private readonly IReadOnlyDictionary<ITable, IDeterminedHash> _tablesCache;

    private readonly IReadOnlyDictionary<IForeignKey, IDeterminedHash> _foreignKeysCache;

    public SchemaEntityProjection(ISchema entity)
    {
        IReadOnlyDictionary<IColumn, IDeterminedHash> columnsCache =
            new ColumnsPrecomputedCache(
                entity
                    .Tables.SelectMany(x => x.Columns)
                    .Prepend(new RowDeterminedHashColumn())
                    .DistinctBy(x => new HexString(new ColumnHash(x)).TextValue)
            );

        IReadOnlyDictionary<IIndex, IDeterminedHash> indexesCache =
            new IndexesPrecomputedCache(
                entity
                    .Tables.SelectMany(x => x.Indexes)
                    .DistinctBy(x => new HexString(new IndexHash(x)).TextValue),
                columnsCache
            );

        _tablesCache = new TablesPrecomputedCache(
            entity.Tables.DistinctBy(x => new HexString(new TableHash(x)).TextValue),
            columnsCache,
            indexesCache
        );

        _foreignKeysCache = new ForeignKeysPrecomputedCache(
            entity.ForeignKeys.DistinctBy(x =>
                new HexString(new ForeignKeyHash(x)).TextValue
            ),
            columnsCache,
            _tablesCache
        );

        _columns = new SchemasTable().Columns;
        _entity = entity;
    }

    internal SchemaEntityProjection(
        ISchema entity,
        IReadOnlyDictionary<ITable, IDeterminedHash> tablesCache,
        IReadOnlyDictionary<IForeignKey, IDeterminedHash> foreignKeysCache
    )
        : this(entity, new SchemasTable().Columns, tablesCache, foreignKeysCache) { }

    internal SchemaEntityProjection(
        ISchema entity,
        IEnumerable<IColumn> columns,
        IReadOnlyDictionary<ITable, IDeterminedHash> tablesCache,
        IReadOnlyDictionary<IForeignKey, IDeterminedHash> foreignKeysCache
    )
    {
        _entity = entity;
        _columns = columns;
        _tablesCache = tablesCache;
        _foreignKeysCache = foreignKeysCache;
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
                                            _entity.Tables.Select(x => _tablesCache[x])
                                        ),
                                        new DeterminedHash(
                                            _entity.ForeignKeys.Select(x =>
                                                _foreignKeysCache[x]
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
