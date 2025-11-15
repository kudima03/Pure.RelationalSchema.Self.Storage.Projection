using System.Collections;
using Pure.HashCodes.Abstractions;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.Column;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Self.Storage.Projection.Caches;
using Pure.RelationalSchema.Storage.Abstractions;
using Pure.RelationalSchema.Storage.HashCodes;

namespace Pure.RelationalSchema.Self.Storage.Projection;

public sealed record SchemaProjection : IEnumerable<IGrouping<ITable, IRow>>
{
    private readonly ISchema _schema;

    public SchemaProjection(ISchema schema)
    {
        _schema = schema;
    }

    public IEnumerator<IGrouping<ITable, IRow>> GetEnumerator()
    {
        IReadOnlyDictionary<IColumn, IDeterminedHash> columnsCache =
            new ColumnsPrecomputedCache(
                _schema
                    .Tables.SelectMany(x => x.Columns)
                    .Prepend(new RowDeterminedHashColumn())
                    .DistinctBy(x => new HexString(new ColumnHash(x)).TextValue)
            );

        IReadOnlyDictionary<IIndex, IDeterminedHash> indexesCache =
            new IndexesPrecomputedCache(
                _schema
                    .Tables.SelectMany(x => x.Indexes)
                    .DistinctBy(x => new HexString(new IndexHash(x)).TextValue),
                columnsCache
            );

        IReadOnlyDictionary<ITable, IDeterminedHash> tablesCache =
            new TablesPrecomputedCache(
                _schema.Tables.DistinctBy(x => new HexString(new TableHash(x)).TextValue),
                columnsCache,
                indexesCache
            );

        IReadOnlyDictionary<IForeignKey, IDeterminedHash> foreignKeysCache =
            new ForeignKeysPrecomputedCache(
                _schema.ForeignKeys.DistinctBy(x =>
                    new HexString(new ForeignKeyHash(x)).TextValue
                ),
                columnsCache,
                tablesCache
            );

        IDeterminedHash cachedSchemaHash = new CachedDeterminedHash(
            new RowHash(
                new SchemaEntityProjection(_schema, tablesCache, foreignKeysCache)
            )
        );

        yield return new Grouping(
            new ColumnTypesTable(),
            _schema
                .Tables.SelectMany(x => x.Columns.Select(c => c.Type))
                .Select(x => new ColumnTypeProjection(x))
        );

        yield return new Grouping(
            new ColumnsTable(),
            _schema
                .Tables.SelectMany(x => x.Columns)
                .Prepend(new RowDeterminedHashColumn())
                .Select(x => new ColumnProjection(x))
        );

        yield return new Grouping(
            new TablesTable(),
            _schema.Tables.Select(x => new TableProjection(x, columnsCache, indexesCache))
        );

        yield return new Grouping(
            new TablesToColumnsTable(),
            _schema.Tables.SelectMany(x =>
                x.Columns.Select(c => new TableToColumnProjection(
                    tablesCache[x],
                    columnsCache[c],
                    new TablesToColumnsTable().Columns
                ))
            )
        );

        yield return new Grouping(
            new IndexesTable(),
            _schema
                .Tables.SelectMany(x => x.Indexes)
                .Select(x => new IndexProjection(x, columnsCache))
        );

        yield return new Grouping(
            new IndexesToColumnsTable(),
            _schema.Tables.SelectMany(table =>
                table.Indexes.SelectMany(x =>
                    x.Columns.Select(c => new IndexToColumnsProjection(
                        indexesCache[x],
                        columnsCache[c],
                        new IndexesToColumnsTable().Columns
                    ))
                )
            )
        );

        yield return new Grouping(
            new TablesToIndexesTable(),
            _schema.Tables.SelectMany(x =>
                x.Indexes.Select(c => new TableToIndexProjection(
                    tablesCache[x],
                    indexesCache[c],
                    new TablesToIndexesTable().Columns
                ))
            )
        );

        yield return new Grouping(
            new ForeignKeysTable(),
            _schema.ForeignKeys.Select(x => new ForeignKeyProjection(
                x,
                columnsCache,
                tablesCache
            ))
        );

        yield return new Grouping(
            new ForeignKeysToReferencingColumnsTable(),
            _schema.ForeignKeys.SelectMany(fk =>
                fk.ReferencingColumns.Select(
                    x => new ForeignKeyToReferencingColumnProjection(
                        foreignKeysCache[fk],
                        columnsCache[x],
                        new ForeignKeysToReferencingColumnsTable().Columns
                    )
                )
            )
        );

        yield return new Grouping(
            new ForeignKeysToReferencedColumnsTable(),
            _schema.ForeignKeys.SelectMany(fk =>
                fk.ReferencedColumns.Select(
                    x => new ForeignKeyToReferencingColumnProjection(
                        foreignKeysCache[fk],
                        columnsCache[x],
                        new ForeignKeysToReferencedColumnsTable().Columns
                    )
                )
            )
        );

        yield return new Grouping(
            new SchemasTable(),
            [new SchemaEntityProjection(_schema, tablesCache, foreignKeysCache)]
        );

        yield return new Grouping(
            new SchemasToTablesTable(),
            _schema.Tables.Select(x => new SchemaToTablesProjection(
                cachedSchemaHash,
                tablesCache[x],
                new SchemasToTablesTable().Columns
            ))
        );

        yield return new Grouping(
            new SchemasToForeignKeysTable(),
            _schema.ForeignKeys.Select(x => new SchemaToForeignKeysProjection(
                cachedSchemaHash,
                foreignKeysCache[x],
                new SchemasToForeignKeysTable().Columns
            ))
        );
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        throw new NotSupportedException();
    }
}
