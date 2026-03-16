using System.Collections;
using Pure.Primitives.String.Operations;
using Pure.RelationalSchema.Abstractions.Column;
using Pure.RelationalSchema.Abstractions.ColumnType;
using Pure.RelationalSchema.Abstractions.ForeignKey;
using Pure.RelationalSchema.Abstractions.Index;
using Pure.RelationalSchema.Abstractions.Schema;
using Pure.RelationalSchema.Abstractions.Table;
using Pure.RelationalSchema.HashCodes;
using Pure.RelationalSchema.Self.Schema.Columns;
using Pure.RelationalSchema.Self.Schema.Tables;
using Pure.RelationalSchema.Storage.Abstractions;

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
        IColumn pkColumn = new UuidColumn();

        IReadOnlyDictionary<IColumnType, IRow> columnTypes =
            new Collections.Generic.Dictionary<IColumnType, IColumnType, IRow>(
                _schema
                    .Tables.SelectMany(x => x.Columns.Select(c => c.Type))
                    .DistinctBy(x => new HexString(new ColumnTypeHash(x)).TextValue),
                x => x,
                x => new ColumnTypeProjection(x),
                x => new ColumnTypeHash(x)
            );

        yield return new Grouping(new ColumnTypesTable(), columnTypes.Values);

        IReadOnlyDictionary<IColumn, IRow> columns = new Collections.Generic.Dictionary<
            IColumn,
            IColumn,
            IRow
        >(
            _schema
                .Tables.SelectMany(x => x.Columns)
                .DistinctBy(x => new HexString(new ColumnHash(x)).TextValue),
            x => x,
            x => new ColumnProjection(x, columnTypes[x.Type].Cells[pkColumn].Value),
            x => new ColumnHash(x)
        );

        yield return new Grouping(new ColumnsTable(), columns.Values);

        IReadOnlyDictionary<IIndex, IRow> indexes = new Collections.Generic.Dictionary<
            IIndex,
            IIndex,
            IRow
        >(
            _schema
                .Tables.SelectMany(x => x.Indexes)
                .DistinctBy(x => new HexString(new IndexHash(x)).TextValue),
            x => x,
            x => new IndexProjection(x),
            x => new IndexHash(x)
        );

        yield return new Grouping(new IndexesTable(), indexes.Values);

        yield return new Grouping(
            new IndexesToColumnsTable(),
            _schema.Tables.SelectMany(table =>
                table.Indexes.SelectMany(x =>
                    x.Columns.Select(c => new IndexToColumnsProjection(
                        indexes[x].Cells[pkColumn].Value,
                        columns[c].Cells[pkColumn].Value,
                        new IndexesToColumnsTable().Columns
                    ))
                )
            )
        );

        IReadOnlyDictionary<ITable, IRow> tables = new Collections.Generic.Dictionary<
            ITable,
            ITable,
            IRow
        >(
            _schema.Tables.DistinctBy(x => new HexString(new TableHash(x)).TextValue),
            x => x,
            x => new TableProjection(x),
            x => new TableHash(x)
        );

        yield return new Grouping(new TablesTable(), tables.Values);

        yield return new Grouping(
            new TablesToColumnsTable(),
            _schema.Tables.SelectMany(x =>
                x.Columns.Select(c => new TableToColumnProjection(
                    tables[x].Cells[pkColumn].Value,
                    columns[c].Cells[pkColumn].Value
                ))
            )
        );

        yield return new Grouping(
            new TablesToIndexesTable(),
            _schema.Tables.SelectMany(x =>
                x.Indexes.Select(c => new TableToIndexProjection(
                    tables[x].Cells[pkColumn].Value,
                    indexes[c].Cells[pkColumn].Value,
                    new TablesToIndexesTable().Columns
                ))
            )
        );

        IReadOnlyDictionary<IForeignKey, IRow> foreignKeys =
            new Collections.Generic.Dictionary<IForeignKey, IForeignKey, IRow>(
                _schema.ForeignKeys.DistinctBy(x =>
                    new HexString(new ForeignKeyHash(x)).TextValue
                ),
                x => x,
                x => new ForeignKeyProjection(
                    tables[x.ReferencingTable].Cells[pkColumn].Value,
                    tables[x.ReferencedTable].Cells[pkColumn].Value
                ),
                x => new ForeignKeyHash(x)
            );

        yield return new Grouping(new ForeignKeysTable(), foreignKeys.Values);

        yield return new Grouping(
            new ForeignKeysToReferencingColumnsTable(),
            _schema.ForeignKeys.SelectMany(fk =>
                fk.ReferencingColumns.Select(
                    x => new ForeignKeyToReferencingColumnProjection(
                        foreignKeys[fk].Cells[pkColumn].Value,
                        columns[x].Cells[pkColumn].Value,
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
                        foreignKeys[fk].Cells[pkColumn].Value,
                        columns[x].Cells[pkColumn].Value,
                        new ForeignKeysToReferencedColumnsTable().Columns
                    )
                )
            )
        );

        IRow schemaProjection = new SchemaEntityProjection(_schema);

        yield return new Grouping(new SchemasTable(), [schemaProjection]);

        yield return new Grouping(
            new SchemasToTablesTable(),
            _schema.Tables.Select(x => new SchemaToTablesProjection(
                schemaProjection.Cells[pkColumn].Value,
                tables[x].Cells[pkColumn].Value,
                new SchemasToTablesTable().Columns
            ))
        );

        yield return new Grouping(
            new SchemasToForeignKeysTable(),
            _schema.ForeignKeys.Select(x => new SchemaToForeignKeysProjection(
                schemaProjection.Cells[pkColumn].Value,
                foreignKeys[x].Cells[pkColumn].Value,
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
